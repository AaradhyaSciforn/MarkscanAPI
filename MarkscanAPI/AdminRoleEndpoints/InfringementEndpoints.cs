using Dapper;
using DbAccess;
using MarkscanAPI.Common;
using MarkscanAPI.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;

namespace MarkscanAPI.AdminRoleEndpoints
{
    public static class InfringementEndpoints
    {
        public static IEndpointRouteBuilder MapInfringementEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapPost("/GetInfringements/Facebook/{CompanyName}", async ([Required] string? CompanyName, [FromBody] Request_DTO req,
                ClaimsPrincipal user, // Automatically populated from JWT
                IDistributedCache cache,
                IDatabaseConnection DatabaseConnection) =>
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

                    var client = await ClientMarkscanAPI.GetClientByCompanyNameAndUserName(DatabaseConnection, CompanyName, loggedInUser.UserName);
                    if (client == null)
                    {
                        return Results.BadRequest($"No Client present for the CompanyName: {CompanyName}!");
                    }
                    if (req.StartDate != null)
                    {
                        if (req.EndDate == null)
                        {
                            req.EndDate = CommonFunctions.ConvertUtcToIst(DateTime.UtcNow);
                        }
                        var URLs = (await FacebookURLs.GetURLsForClient(DatabaseConnection, client.Id, (DateTime)req.StartDate, req.EndDate, req.AssetName)).ToList();
                        return Results.Ok(URLs);
                    }
                    return Results.BadRequest("Start Date must be present!");
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(ex.Message.ToString());
                }
            })
            .RequireAuthorization(
                new AuthorizeAttribute
                {
                    AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
                    Roles = "partner"
                }
            )
            .WithTags("4. Get Infringements for Partner")
            .WithMetadata(new SwaggerOperationAttribute("Get all the Infringements.", "Gets the list of all the infringements present on FaceBook for the client."));

            app.MapPost("/GetInfringements/Youtube/{CompanyName}", async ([Required] string? CompanyName, [FromBody] Request_DTO req,
                ClaimsPrincipal user, // Automatically populated from JWT
                IDistributedCache cache,
                IDatabaseConnection DatabaseConnection) =>
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

                    var client = await ClientMarkscanAPI.GetClientByCompanyNameAndUserName(DatabaseConnection, CompanyName, loggedInUser.UserName);
                    if (client == null)
                    {
                        return Results.BadRequest($"No Client present for the CompanyName: {CompanyName}!");
                    }
                    if (req.StartDate != null)
                    {
                        if (req.EndDate == null)
                        {
                            req.EndDate = CommonFunctions.ConvertUtcToIst(DateTime.UtcNow);
                        }
                        var URLs = (await YoutubeURLs.GetURLsForClient(DatabaseConnection, client.Id, (DateTime)req.StartDate, req.EndDate, req.AssetName)).ToList();
                        return Results.Ok(URLs);
                    }
                    return Results.BadRequest("Start Date must be present!");
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(ex.Message.ToString());
                }
            })
            .RequireAuthorization(
                new AuthorizeAttribute
                {
                    AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
                    Roles = "partner"
                }
            )
            .WithTags("4. Get Infringements for Partner")
            .WithMetadata(new SwaggerOperationAttribute("Get all the Infringements.", "Gets the list of all the infringements present on Youtube for the client."));

            app.MapPost("/GetInfringements/Instagram/{CompanyName}", async ([Required] string? CompanyName, [FromBody] Request_DTO req,
                ClaimsPrincipal user, // Automatically populated from JWT
                IDistributedCache cache,
                IDatabaseConnection DatabaseConnection) =>
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

                    var client = await ClientMarkscanAPI.GetClientByCompanyNameAndUserName(DatabaseConnection, CompanyName, loggedInUser.UserName);
                    if (client == null)
                    {
                        return Results.BadRequest($"No Client present for the CompanyName: {CompanyName}!");
                    }
                    if (req.StartDate != null)
                    {
                        if (req.EndDate == null)
                        {
                            req.EndDate = CommonFunctions.ConvertUtcToIst(DateTime.UtcNow);
                        }
                        var URLs = (await InstagramUrls.GetURLsForClient(DatabaseConnection, client.Id, (DateTime)req.StartDate, req.EndDate, req.AssetName)).ToList();
                        return Results.Ok(URLs);
                    }
                    return Results.BadRequest("Start Date must be present!");
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(ex.Message.ToString());
                }
            })
            .RequireAuthorization(
                new AuthorizeAttribute
                {
                    AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
                    Roles = "partner"
                }
            )
            .WithTags("4. Get Infringements for Partner")
            .WithMetadata(new SwaggerOperationAttribute("Get all the Infringements.", "Gets the list of all the infringements present on Instagram for the client."));

            app.MapPost("/GetInfringements/Telegram/{CompanyName}", async ([Required] string? CompanyName, [FromBody] Request_DTO req,
                ClaimsPrincipal user, // Automatically populated from JWT
                IDistributedCache cache,
                IDatabaseConnection DatabaseConnection) =>
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

                    var client = await ClientMarkscanAPI.GetClientByCompanyNameAndUserName(DatabaseConnection, CompanyName, loggedInUser.UserName);
                    if (client == null)
                    {
                        return Results.BadRequest($"No Client present for the CompanyName: {CompanyName}!");
                    }
                    if (req.StartDate != null)
                    {
                        if (req.EndDate == null)
                        {
                            req.EndDate = CommonFunctions.ConvertUtcToIst(DateTime.UtcNow);
                        }
                        var URLs = (await TelegramUrls.GetURLsForClient(DatabaseConnection, client.Id, (DateTime)req.StartDate, req.EndDate, req.AssetName)).ToList();
                        return Results.Ok(URLs);
                    }
                    return Results.BadRequest("Start Date must be present!");
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(ex.Message.ToString());
                }
            })
            .RequireAuthorization(
                new AuthorizeAttribute
                {
                    AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
                    Roles = "partner"
                }
            )
            .WithTags("4. Get Infringements for Partner")
            .WithMetadata(new SwaggerOperationAttribute("Get all the Infringements.", "Gets the list of all the infringements present on Telegram for the client."));

            app.MapPost("/GetInfringements/Twitter/{CompanyName}", async ([Required] string? CompanyName, [FromBody] Request_DTO req,
                ClaimsPrincipal user, // Automatically populated from JWT
                IDistributedCache cache,
                IDatabaseConnection DatabaseConnection) =>
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

                    var client = await ClientMarkscanAPI.GetClientByCompanyNameAndUserName(DatabaseConnection, CompanyName, loggedInUser.UserName);
                    if (client == null)
                    {
                        return Results.BadRequest($"No Client present for the CompanyName: {CompanyName}!");
                    }
                    if (req.StartDate != null)
                    {
                        if (req.EndDate == null)
                        {
                            req.EndDate = CommonFunctions.ConvertUtcToIst(DateTime.UtcNow);
                        }
                        var URLs = (await TwitterURLsNEW.GetURLsForClient(DatabaseConnection, client.Id, (DateTime)req.StartDate, req.EndDate, req.AssetName)).ToList();
                        return Results.Ok(URLs);
                    }
                    return Results.BadRequest("Start Date must be present!");
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(ex.Message.ToString());
                }
            })
            .RequireAuthorization(
                new AuthorizeAttribute
                {
                    AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
                    Roles = "partner"
                }
            )
            .WithTags("4. Get Infringements for Partner")
            .WithMetadata(new SwaggerOperationAttribute("Get all the Infringements.", "Gets the list of all the infringements present on Twitter for the client."));

            app.MapPost("/GetInfringements/UGCAndOtherSocialMedia/{CompanyName}", async ([Required] string? CompanyName, [FromBody] Request_DTO req,
                ClaimsPrincipal user, // Automatically populated from JWT
                IDistributedCache cache,
                IDatabaseConnection DatabaseConnection) =>
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

                    var client = await ClientMarkscanAPI.GetClientByCompanyNameAndUserName(DatabaseConnection, CompanyName, loggedInUser.UserName);
                    if (client == null)
                    {
                        return Results.BadRequest($"No Client present for the CompanyName: {CompanyName}!");
                    }
                    if (req.StartDate != null)
                    {
                        if (req.EndDate == null)
                        {
                            req.EndDate = CommonFunctions.ConvertUtcToIst(DateTime.UtcNow);
                        }
                        var URLs = (await UGCAndOtherSocialMediaURLs.GetURLsForClient(DatabaseConnection, client.Id, (DateTime)req.StartDate, req.EndDate, req.AssetName)).ToList();
                        return Results.Ok(URLs);
                    }
                    return Results.BadRequest("Start Date must be present!");
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(ex.Message.ToString());
                }
            })
            .RequireAuthorization(
                new AuthorizeAttribute
                {
                    AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
                    Roles = "partner"
                }
            )
            .WithTags("4. Get Infringements for Partner")
            .WithMetadata(new SwaggerOperationAttribute("Get all the Infringements.", "Gets the list of all the infringements present on UGCAndOtherSocialMedia for the client."));

            app.MapPost("/GetInfringements/Internet/{CompanyName}", async ([Required] string? CompanyName, [FromBody] Request_DTO req,
                ClaimsPrincipal user, // Automatically populated from JWT
                IDistributedCache cache,
                IDatabaseConnection DatabaseConnection) =>
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

                    var client = await ClientMarkscanAPI.GetClientByCompanyNameAndUserName(DatabaseConnection, CompanyName, loggedInUser.UserName);
                    if (client == null)
                    {
                        return Results.BadRequest($"No Client present for the CompanyName: {CompanyName}!");
                    }
                    if (req.StartDate != null)
                    {
                        if (req.EndDate == null)
                        {
                            req.EndDate = CommonFunctions.ConvertUtcToIst(DateTime.UtcNow);
                        }
                        var URLs = (await InternetURLs.GetURLsForClient(DatabaseConnection, client.Id, (DateTime)req.StartDate, req.EndDate, req.AssetName)).ToList();
                        return Results.Ok(URLs);
                    }
                    return Results.BadRequest("Start Date must be present!");
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(ex.Message.ToString());
                }
            })
            .RequireAuthorization(
                new AuthorizeAttribute
                {
                    AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
                    Roles = "partner"
                }
            )
            .WithTags("4. Get Infringements for Partner")
            .WithMetadata(new SwaggerOperationAttribute("Get all the Infringements.", "Gets the list of all the infringements present on Internet for the client."));


            return app;
        }
    }
}
