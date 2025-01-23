using DbAccess;
using MarkscanAPI.Common;
using MarkscanAPI.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Annotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

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
        Title = "MarkScan API",
        Description = "An ASP.NET Core Web API ",
    });
    options.EnableAnnotations();
});


var MediaScanConnection = builder.Configuration.GetConnectionString("MediaScanConnection");
IDatabaseConnection databaseConnection = new DatabaseConnection(MediaScanConnection);


var app = builder.Build();

app.UseDeveloperExceptionPage();
app.UseSwagger();

app.UseSwaggerUI(/*c =>
{
    c.SwaggerEndpoint("/stage/swagger/v1/swagger.json", "Api");
}*/
    config =>
    {
        config.ConfigObject.AdditionalItems["syntaxHighlight"] = new Dictionary<string, object>
        {
            ["activated"] = false
        };
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

app.MapPost("/Login", async ([FromBody] Login_DTO user) =>
{

    try
    {
        if (!string.IsNullOrEmpty(user.UserName))
        {
            var loggedInUser = await Credentials.GetUserDetails(databaseConnection, user.UserName);

            if (loggedInUser != null)
            {

                var decryptedPassword = CommonFunctions.DecryptString(loggedInUser.Passwordhash);

                if (decryptedPassword == user.Password)
                {
                    var claims = new List<Claim>()
                    {
                        new Claim(ClaimTypes.Name, loggedInUser.UserName),
                        new Claim(ClaimTypes.Role, "client"),
                        //new Claim(ClaimTypes.Sid, loggedInUser.UserId),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
                    };

                    var token = new JwtSecurityToken
                    (
                        issuer: builder.Configuration["Jwt:Issuer"],
                        audience: builder.Configuration["Jwt:Audience"],
                        claims: claims,
                        expires: DateTime.Now.AddDays(1),
                        notBefore: DateTime.Now,
                        signingCredentials: new SigningCredentials(
                            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
                            SecurityAlgorithms.HmacSha256)
                    );

                    var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
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

app.MapPost("/GetInfringements", [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "client")] async ([FromBody] Request_DTO req) =>
{

    try
    {
        if (req.StartDate != null)
        {
            if(req.EndDate == null)
            {
                req.EndDate = await CommonFunctions.ConvertUtcToIst(DateTime.UtcNow);
            }
            var ytURLs = await YoutubeURLs.GetURLsForClient(databaseConnection, "83C95897-D4DF-4B51-9C95-1EE42DAA34C8", (DateTime)req.StartDate, req.EndDate);
            return Results.Ok(ytURLs);
        }
        return Results.BadRequest("Start Date must be present!");
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.Message.ToString());
    }

}).WithTags("2. Client").WithMetadata(new SwaggerOperationAttribute("Get all the Infringements.", "Gets the list of all the infringements present for the client."));


var authenticationTag = new OpenApiTag { Name = "Authentication", Description = "Methods related to authentication" };



app.UseHttpsRedirection();


app.MapControllers();

app.Run();
