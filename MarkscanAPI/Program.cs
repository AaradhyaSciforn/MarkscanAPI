using Dapper;
using DbAccess;
using MarkscanAPI.AdminRoleEndpoints;
using MarkscanAPI.Common;
using MarkscanAPI.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//builder.Services.AddOpenApi();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Description = "Bearer Authentication with JWT Token",
        Type = SecuritySchemeType.Http
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme
                }
            },
            new List<string>()
        }
    });
});
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateActor = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),

       
    };
});
builder.Services.AddAuthorization(/*options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
    .RequireAuthenticatedUser()
    .Build();
}*/);

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "IP-House API",
        Description = "An ASP.NET Core Web API ",
    });
    options.EnableAnnotations();
});

/*builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
});*/
builder.Services.AddDistributedMemoryCache();
var MediaScanConnection = builder.Configuration.GetConnectionString("MediaScanConnection");
builder.Services.AddTransient<IDatabaseConnection>(db => new DatabaseConnection(MediaScanConnection));
IDatabaseConnection databaseConnection = new DatabaseConnection(MediaScanConnection);


var app = builder.Build();

app.UseDeveloperExceptionPage();
app.UseSwagger();

app.UseSwaggerUI(
    config =>
    {
        config.ConfigObject.AdditionalItems.Add("syntaxHighlight", false);
        config.ConfigObject.AdditionalItems.Add("theme", "agate");
        config.ConfigObject.DefaultModelRendering = ModelRendering.Model;
        config.ConfigObject.DisplayRequestDuration = false;
        config.ConfigObject.ShowExtensions = false;
        config.ConfigObject.AdditionalItems.Add("defaultModelExpandDepth", 1);
        config.DocExpansion(DocExpansion.None);

    }
);

app.Use(async (context, next) =>
{
    var localTime = TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
    context.Response.Headers.Add("X-Local-Time", localTime.ToString("yyyy-MM-dd HH:mm:ss zzz"));
    await next.Invoke();
});

/*app.UseCors();*/
app.UseAuthorization();
app.UseAuthentication();

app.MapPost("/Login", async ([FromBody] Login_DTO user, IDistributedCache cache) =>
{

    try
    {
        if (!string.IsNullOrEmpty(user.UserName))
        {
            var loggedInUser = await Credentials.GetUserDetails(databaseConnection, user.UserName);

            if (loggedInUser != null)
            {

                var decryptedPassword = CommonFunctions.DecryptString(loggedInUser.Passwordhash!);

                if (decryptedPassword == user.Password)
                {
                    var claims = new List<Claim>()
                    {
                        new Claim(ClaimTypes.Name, loggedInUser.UserName!),
                        new Claim(ClaimTypes.Role, loggedInUser.Role!.ToLower()),
                        //new Claim(ClaimTypes.Sid, loggedInUser.UserId),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
                    };

                    var token = new JwtSecurityToken
                    (
                        issuer: builder.Configuration["Jwt:Issuer"],
                        audience: builder.Configuration["Jwt:Audience"],
                        claims: claims,
                        expires: DateTime.Now.AddMonths(1),
                        notBefore: DateTime.Now,
                        signingCredentials: new SigningCredentials(
                            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
                            SecurityAlgorithms.HmacSha256)
                    );

                    var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

                    var cacheKey = token.Id;
                    var cacheOptions = new DistributedCacheEntryOptions()
                    {
                        AbsoluteExpiration = token.ValidTo,
                    };
                    using var conn = databaseConnection.GetConnection();
                    if (loggedInUser.Role == "Client")
                    {
                        loggedInUser.ClientId = await conn.QueryFirstOrDefaultAsync<string>(@"select Id from ClientMaster where trim(lower(CompanyName))=trim(lower(@name));", new { name = loggedInUser.UserName });
                    }
                    var userData = JsonSerializer.Serialize(loggedInUser);
                    await cache.SetStringAsync(cacheKey, userData, cacheOptions);
                    return Results.Ok(tokenString);
                }
                else
                {
                    return Results.BadRequest("Invalid password! Please retry with correct credentials.");
                }
            }
            else
            {
                return Results.BadRequest("Invalid user credentials!");
            }

        }

        return Results.BadRequest("Invalid user credentials!");
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.Message.ToString());
    }

}).WithTags("1. Authentication").WithMetadata(new SwaggerOperationAttribute("Authentication.", "On valid login user will get a JWT token which can be used to access other requests."));


/*app.MapPost("/GetInfringements/AllPlatforms", [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "client")] async (
    [FromBody] Request_DTO req,
    ClaimsPrincipal user,
    IDistributedCache cache) =>
{
    try
    {
        var jti = user.FindFirstValue(JwtRegisteredClaimNames.Jti);
        if (string.IsNullOrEmpty(jti)) return Results.Unauthorized();

        var userData = await cache.GetStringAsync(jti);
        if (userData == null) return Results.Unauthorized();

        var loggedInUser = JsonSerializer.Deserialize<Credentials>(userData);
        if (req.StartDate == null) return Results.BadRequest("Start Date must be present!");
        req.EndDate ??= CommonFunctions.ConvertUtcToIst(DateTime.UtcNow);

        var ytTask = YoutubeURLs.GetURLsForClient(databaseConnection, loggedInUser.ClientId, (DateTime)req.StartDate, req.EndDate, req.AssetName);
        var fbTask = FacebookURLs.GetURLsForClient(databaseConnection, loggedInUser.ClientId, (DateTime)req.StartDate, req.EndDate, req.AssetName);
        var igTask = InstagramUrls.GetURLsForClient(databaseConnection, loggedInUser.ClientId, (DateTime)req.StartDate, req.EndDate, req.AssetName);
        var tgTask = TelegramUrls.GetURLsForClient(databaseConnection, loggedInUser.ClientId, (DateTime)req.StartDate, req.EndDate, req.AssetName);
        var twTask = TwitterURLsNEW.GetURLsForClient(databaseConnection, loggedInUser.ClientId, (DateTime)req.StartDate, req.EndDate, req.AssetName);
        var ugcTask = UGCAndOtherSocialMediaURLs.GetURLsForClient(databaseConnection, loggedInUser.ClientId, (DateTime)req.StartDate, req.EndDate, req.AssetName);

        await Task.WhenAll(ytTask, fbTask, igTask, tgTask, twTask, ugcTask);

        var result = new List<InfringementDTO>();

        //  var langcheck = ytTask.Result.FirstOrDefault();

        result.AddRange(ytTask.Result.Select(x => InfringementDTO.MapToDTO(x, "YouTube")).ToList());
        result.AddRange(fbTask.Result.Select(x => InfringementDTO.MapToDTO(x, "Facebook")).ToList());
        result.AddRange(igTask.Result.Select(x => InfringementDTO.MapToDTO(x, "Instagram")).ToList());
        result.AddRange(tgTask.Result.Select(x => InfringementDTO.MapToDTO(x, "Telegram")).ToList());
        result.AddRange(twTask.Result.Select(x => InfringementDTO.MapToDTO(x, "Twitter")).ToList());
        result.AddRange(ugcTask.Result.Select(x => InfringementDTO.MapToDTO(x, "UGC")).ToList());

        return Results.Ok(result);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.Message.ToString());
    }
}).WithTags("2. Client")
  .WithMetadata(new SwaggerOperationAttribute("Get all the Infringements (All Platforms)", "Gets the list of all the infringements across all platforms for the client."));

*/

app.MapPost("/GetInfringements/AllPlatforms", [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "client")] async ([FromBody] Request_DTO req,
    ClaimsPrincipal user, // Automatically populated from JWT
    IDistributedCache cache) =>
{
    try
    {
        var jti = user.FindFirstValue(JwtRegisteredClaimNames.Jti);
        if (string.IsNullOrEmpty(jti))
        {
            return Results.Unauthorized();
        }
        var userData = await cache.GetStringAsync(jti);
        if (userData == null)
        {
            return Results.Unauthorized();
        }

        var loggedInUser = JsonSerializer.Deserialize<Credentials>(userData);
        if (req.StartDate != null)
        {
            if (req.EndDate == null)
            {
                req.EndDate = CommonFunctions.ConvertUtcToIst(DateTime.UtcNow);
            }
            var urls = (await AllInOne.GetURLsForClient(databaseConnection, loggedInUser.ClientId, (DateTime)req.StartDate, req.EndDate, req.AssetName)).ToList();
            return Results.Ok(urls);
        }
        return Results.BadRequest("Start Date must be present!");
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.Message.ToString());
    }

}).WithTags("2. Get Infringements").WithMetadata(new SwaggerOperationAttribute("Get all the Infringements.", "Gets the list of all the infringements present on ALL Social Medias for the client."));


app.MapPost("/GetInfringements/Youtube", [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "client")] async ([FromBody] Request_DTO req, 
    ClaimsPrincipal user, // Automatically populated from JWT
    IDistributedCache cache) =>
{
    try
    {
        var jti = user.FindFirstValue(JwtRegisteredClaimNames.Jti);
        if (string.IsNullOrEmpty(jti))
        {
            return Results.Unauthorized();
        }
        var userData = await cache.GetStringAsync(jti);
        if (userData == null)
        {
            return Results.Unauthorized();
        }

        var loggedInUser = JsonSerializer.Deserialize<Credentials>(userData);
        if (req.StartDate != null)
        {
            if(req.EndDate == null)
            {
                req.EndDate = CommonFunctions.ConvertUtcToIst(DateTime.UtcNow);
            }
            var ytURLs = (await YoutubeURLs.GetURLsForClient(databaseConnection, loggedInUser.ClientId, (DateTime)req.StartDate, req.EndDate, req.AssetName)).ToList();
            return Results.Ok(ytURLs);
        }
        return Results.BadRequest("Start Date must be present!");
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.Message.ToString());
    }

}).WithTags("2. Get Infringements").WithMetadata(new SwaggerOperationAttribute("Get all the Infringements.", "Gets the list of all the infringements present on YouTube for the client.")); //yout

app.MapPost("/GetInfringements/Facebook", [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "client")] async ([FromBody] Request_DTO req,
    ClaimsPrincipal user, // Automatically populated from JWT
    IDistributedCache cache) =>
{
    try
    {
        var jti = user.FindFirstValue(JwtRegisteredClaimNames.Jti);
        if (string.IsNullOrEmpty(jti))
        {
            return Results.Unauthorized();
        }
        var userData = await cache.GetStringAsync(jti);
        if (userData == null)
        {
            return Results.Unauthorized();
        }

        var loggedInUser = JsonSerializer.Deserialize<Credentials>(userData);
        if (req.StartDate != null)
        {
            if (req.EndDate == null)
            {
                req.EndDate = CommonFunctions.ConvertUtcToIst(DateTime.UtcNow);
            }
            var FBURLs = (await FacebookURLs.GetURLsForClient(databaseConnection, loggedInUser.ClientId, (DateTime)req.StartDate, req.EndDate, req.AssetName)).ToList();
            return Results.Ok(FBURLs);
        }
        return Results.BadRequest("Start Date must be present!");
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.Message.ToString());
    }

}).WithTags("2. Get Infringements").WithMetadata(new SwaggerOperationAttribute("Get all the Infringements.", "Gets the list of all the infringements present on FaceBook for the client.")); // face


app.MapPost("/GetInfringements/Instagram", [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "client")] async ([FromBody] Request_DTO req,
    ClaimsPrincipal user, // Automatically populated from JWT
    IDistributedCache cache) =>
{
    try
    {
        var jti = user.FindFirstValue(JwtRegisteredClaimNames.Jti);
        if (string.IsNullOrEmpty(jti))
        {
            return Results.Unauthorized();
        }
        var userData = await cache.GetStringAsync(jti);
        if (userData == null)
        {
            return Results.Unauthorized();
        }

        var loggedInUser = JsonSerializer.Deserialize<Credentials>(userData);
        if (req.StartDate != null)
        {
            if (req.EndDate == null)
            {
                req.EndDate = CommonFunctions.ConvertUtcToIst(DateTime.UtcNow);
            }
            var IGURLs = (await InstagramUrls.GetURLsForClient(databaseConnection, loggedInUser.ClientId, (DateTime)req.StartDate, req.EndDate, req.AssetName)).ToList();
            return Results.Ok(IGURLs);
        }
        return Results.BadRequest("Start Date must be present!");
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.Message.ToString());
    }

}).WithTags("2. Get Infringements").WithMetadata(new SwaggerOperationAttribute("Get all the Infringements.", "Gets the list of all the infringements present on Instagram for the client.")); //insta

app.MapPost("/GetInfringements/Telegram", [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "client")] async ([FromBody] Request_DTO req,
    ClaimsPrincipal user, // Automatically populated from JWT
    IDistributedCache cache) =>
{
    try
    {
        var jti = user.FindFirstValue(JwtRegisteredClaimNames.Jti);
        if (string.IsNullOrEmpty(jti))
        {
            return Results.Unauthorized();
        }
        var userData = await cache.GetStringAsync(jti);
        if (userData == null)
        {
            return Results.Unauthorized();
        }

        var loggedInUser = JsonSerializer.Deserialize<Credentials>(userData);
        if (req.StartDate != null)
        {
            if (req.EndDate == null)
            {
                req.EndDate = CommonFunctions.ConvertUtcToIst(DateTime.UtcNow);
            }
            var TGURLs = (await TelegramUrls.GetURLsForClient(databaseConnection, loggedInUser.ClientId, (DateTime)req.StartDate, req.EndDate, req.AssetName)).ToList(); // already in ist
            return Results.Ok(TGURLs);
        }
        return Results.BadRequest("Start Date must be present!");
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.Message.ToString());
    }

}).WithTags("2. Get Infringements").WithMetadata(new SwaggerOperationAttribute("Get all the Infringements.", "Gets the list of all the infringements present on Telegram for the client.")); // tele

app.MapPost("/GetInfringements/Twitter", [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "client")] async ([FromBody] Request_DTO req,
    ClaimsPrincipal user, // Automatically populated from JWT
    IDistributedCache cache) =>
{
    try
    {
        var jti = user.FindFirstValue(JwtRegisteredClaimNames.Jti);
        if (string.IsNullOrEmpty(jti))
        {
            return Results.Unauthorized();
        }
        var userData = await cache.GetStringAsync(jti);
        if (userData == null)
        {
            return Results.Unauthorized();
        }

        var loggedInUser = JsonSerializer.Deserialize<Credentials>(userData);
        if (req.StartDate != null)
        {
            if (req.EndDate == null)
            {
                req.EndDate = CommonFunctions.ConvertUtcToIst(DateTime.UtcNow);
            }
            var IGURLs = (await TwitterURLsNEW.GetURLsForClient(databaseConnection, loggedInUser.ClientId, (DateTime)req.StartDate, req.EndDate, req.AssetName)).ToList();
            return Results.Ok(IGURLs);
        }
        return Results.BadRequest("Start Date must be present!");
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.Message.ToString());
    }

}).WithTags("2. Get Infringements").WithMetadata(new SwaggerOperationAttribute("Get all the Infringements.", "Gets the list of all the infringements present on Twitter for the client.")); // tweet

app.MapPost("/GetInfringements/UGCAndOtherSocialMedia", [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "client")] async ([FromBody] Request_DTO req,
    ClaimsPrincipal user, // Automatically populated from JWT
    IDistributedCache cache) =>
{
    try
    {
        var jti = user.FindFirstValue(JwtRegisteredClaimNames.Jti);
        if (string.IsNullOrEmpty(jti))
        {
            return Results.Unauthorized();
        }
        var userData = await cache.GetStringAsync(jti);
        if (userData == null)
        {
            return Results.Unauthorized();
        }

        var loggedInUser = JsonSerializer.Deserialize<Credentials>(userData);
        if (req.StartDate != null)
        {
            if (req.EndDate == null)
            {
                req.EndDate = CommonFunctions.ConvertUtcToIst(DateTime.UtcNow);
            }
            var IGURLs = (await UGCAndOtherSocialMediaURLs.GetURLsForClient(databaseConnection, loggedInUser.ClientId, (DateTime)req.StartDate, req.EndDate, req.AssetName)).ToList();
            return Results.Ok(IGURLs);
        }
        return Results.BadRequest("Start Date must be present!");
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.Message.ToString());
    }

}).WithTags("2. Get Infringements").WithMetadata(new SwaggerOperationAttribute("Get all the Infringements.", "Gets the list of all the infringements present on other Social Medias for the client.")); //UGC

app.MapClientEndpoints();
app.MapAssetEndpoints();

var authenticationTag = new OpenApiTag { Name = "Authentication", Description = "Methods related to authentication" };



app.UseHttpsRedirection();


app.MapControllers();

app.Run();


