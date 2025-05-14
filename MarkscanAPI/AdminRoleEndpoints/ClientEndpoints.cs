using MarkscanAPI.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Caching.Distributed;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MarkscanAPI.Common;
using System.Text.Json;
using System.IdentityModel.Tokens.Jwt;
using DbAccess;
using Dapper.Contrib.Extensions;
using Dapper;
using System.ComponentModel.DataAnnotations;

namespace MarkscanAPI.AdminRoleEndpoints
{
    public static class ClientEndpoints
    {
        public static IEndpointRouteBuilder MapClientEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapGet("/GetDetails", async (
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
                    if (loggedInUser == null)
                    {
                        return Results.BadRequest("User is not logged in!");
                    }
                    var alreadyPresentClients = await ClientMarkscanAPI.GetAllClientsByUserName(DatabaseConnection, loggedInUser.UserName);

                    using var conn = DatabaseConnection.GetConnection();
                    var ClientTypeList = (await conn.QueryAsync<string>(@"select Name from ClientType where Active=1;")).ToList();
                    var GenreList = (await conn.QueryAsync<string>(@"select Name from GenreMS where Active=1;")).ToList();
                    var LanguageList = (await conn.QueryAsync<string>(@"select Name from Language where Active=1;")).Distinct().ToList();
                    var CountryList = (await conn.QueryAsync<string>(@"select Name from Countries where Active=1;")).Distinct().ToList();

                    var details = new DetailsClass()
                    {
                        ClientTypes = ClientTypeList,
                        Genres = GenreList,
                        Languages = LanguageList,
                        Countries = CountryList
                    };
                    
                    return Results.Ok(details);
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
            .WithTags("3. Add/Update Clients and Assets")
            .WithMetadata(new SwaggerOperationAttribute("Get all Clients.", "Fetches all the clients, previously sent through API."));

            app.MapGet("/GetAllClients", async (
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
                    if (loggedInUser == null)
                    {
                        return Results.BadRequest("User is not logged in!");
                    }
                    var alreadyPresentClients = await ClientMarkscanAPI.GetAllClientsByUserName(DatabaseConnection, loggedInUser.UserName);

                    using var conn = DatabaseConnection.GetConnection();
                    var MapGenre = (await conn.QueryAsync<IdNameClass>(@"select Id, Name from GenreMS where Active=1;")).ToDictionary(x => x.Id, x => x.Name);
                    var MapClientType = (await conn.QueryAsync<IdNameClass>(@"select Id, Name from ClientType where Active=1;")).ToDictionary(x => x.Id, x => x.Name);
                    List<ClientRequest_DTO> clientList = new();

                    if (alreadyPresentClients != null && alreadyPresentClients.Any())
                    {
                        foreach (var client in alreadyPresentClients)
                        {
                            ClientRequest_DTO _client = new();
                            _client.CompanyName = client.CompanyName;
                            _client.CopyrightOwnerNameList = (await ClientMarkscanAPICopyrightOwner.GetCopyrightOwnersForClient(conn, client.Id))?.Select(x => x.Name).ToList();
                            _client.GenreList = (await ClientMarkscanAPIGenre.GetGenresForClient(conn, client.Id))?.Select(x => MapGenre[x.GenreId]).ToList();
                            _client.TypeOfClient = MapClientType[client.ClientTypeId];
                            clientList.Add(_client);
                        }

                    }
                    return Results.Ok(clientList);
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
            .WithTags("3. Add/Update Clients and Assets")
            .WithMetadata(new SwaggerOperationAttribute("Get all Clients.", "Fetches all the clients, previously sent through API."));

            app.MapGet("/GetClient/{CompanyName}", async ([Required] string? CompanyName,
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
                    if (loggedInUser == null)
                    {
                        return Results.BadRequest("User is not logged in!");
                    }
                    if (string.IsNullOrWhiteSpace(CompanyName))
                    {
                        return Results.BadRequest("CompanyName cannot be empty!");
                    }
                    var client = await ClientMarkscanAPI.GetClientByCompanyNameAndUserName(DatabaseConnection, CompanyName, loggedInUser.UserName);
                    if (client != null)
                    {
                        using var conn = DatabaseConnection.GetConnection();
                        var MapGenre = (await conn.QueryAsync<IdNameClass>(@"select Id, Name from GenreMS where Active=1;")).ToDictionary(x => x.Id, x => x.Name);
                        var MapClientType = (await conn.QueryAsync<IdNameClass>(@"select Id, Name from ClientType where Active=1;")).ToDictionary(x => x.Id, x => x.Name);

                        ClientRequest_DTO _client = new();
                        _client.CompanyName = client.CompanyName;
                        _client.CopyrightOwnerNameList = (await ClientMarkscanAPICopyrightOwner.GetCopyrightOwnersForClient(conn, client.Id))?.Select(x => x.Name).ToList();
                        _client.GenreList = (await ClientMarkscanAPIGenre.GetGenresForClient(conn, client.Id))?.Select(x => MapGenre[x.GenreId]).ToList();
                        _client.TypeOfClient = MapClientType[client.ClientTypeId];
                        return Results.Ok(_client);
                    }
                    return Results.BadRequest($"No Client found by the CompanyName: {CompanyName}!");
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
            .WithTags("3. Add/Update Clients and Assets")
            .WithMetadata(new SwaggerOperationAttribute("Get Client by Company Name.", "Fetches the clients with the Company Name provided."));

            app.MapPost("/AddClient", async ([FromBody] ClientRequest_DTO req,
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
                    if (loggedInUser == null)
                    {
                        return Results.BadRequest("User is not logged in!");
                    }
                    if (string.IsNullOrWhiteSpace(req.CompanyName))
                    {
                        return Results.BadRequest("Company Name cannot be empty!");
                    }
                    if (string.IsNullOrWhiteSpace(req.TypeOfClient))
                    {
                        return Results.BadRequest("Type Of Client cannot be empty!");
                    }
                    if(req.GenreList == null || !req.GenreList.Any())
                    {
                        return Results.BadRequest("Genre List cannot be empty!");
                    }
                    if(req.CopyrightOwnerNameList == null || !req.CopyrightOwnerNameList.Any())
                    {
                        return Results.BadRequest("Copyright Owner Name List cannot be empty!");
                    }
                    var alreadyPresentClients = await ClientMarkscanAPI.GetAllClients(DatabaseConnection);
                    var checkIfAlreadyPresent = alreadyPresentClients.Where(x => x.CompanyName == req.CompanyName).FirstOrDefault();
                    if (checkIfAlreadyPresent != null)
                    {
                        return Results.BadRequest($"Client Already present and is {((checkIfAlreadyPresent.IsApproved) ? "Approved" : "Pending for Approval") }!");
                    }

                    using var conn = DatabaseConnection.GetConnection();
                    await conn.OpenAsync();
                    using var transaction = await conn.BeginTransactionAsync();
                    ClientMarkscanAPI client = new();
                    CommonFunctions.SetBaseFields(client, loggedInUser?.UserName);
                    client.CompanyName = req.CompanyName;
                    client.CompanyName = req.CompanyName;
                    client.UserName = loggedInUser?.UserName;
                    await CommonFunctions.AddTypeOfClient(req.TypeOfClient, client, conn, transaction);

                    await conn.InsertAsync(client, transaction);

                    await CommonFunctions.AddGenres(req.GenreList, client, loggedInUser?.UserName, conn, transaction);
                    await CommonFunctions.AddCopyrightOwners(req.CopyrightOwnerNameList, client, loggedInUser?.UserName, conn, transaction);
                    await transaction.CommitAsync();
                    return Results.Ok("Client successfully created!");
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
            .WithTags("3. Add/Update Clients and Assets")
            .WithMetadata(new SwaggerOperationAttribute("Add Clients.", "Sends a post request to add client."));

            app.MapPut("/UpdateClient", async ([FromBody] ClientRequest_DTO req,
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
                    if (loggedInUser == null)
                    {
                        return Results.BadRequest("User is not logged in!");
                    }
                    if (string.IsNullOrWhiteSpace(req.CompanyName))
                    {
                        return Results.BadRequest("Company Name cannot be empty!");
                    }
                    if (string.IsNullOrWhiteSpace(req.TypeOfClient))
                    {
                        return Results.BadRequest("Type Of Client cannot be empty!");
                    }
                    if (req.GenreList == null || !req.GenreList.Any())
                    {
                        return Results.BadRequest("Genre List cannot be empty!");
                    }
                    if (req.CopyrightOwnerNameList == null || !req.CopyrightOwnerNameList.Any())
                    {
                        return Results.BadRequest("Copyright Owner Name List cannot be empty!");
                    }
                    var client = await ClientMarkscanAPI.GetActiveInactiveClientByCompanyNameAndUserName(DatabaseConnection, req.CompanyName, loggedInUser.UserName);
                    if (client == null)
                    {
                        return Results.BadRequest($"No Client with CompanyName: {req.CompanyName} found to update!");
                    }

                    using var conn = DatabaseConnection.GetConnection();
                    await conn.OpenAsync();
                    using var transaction = await conn.BeginTransactionAsync();

                    await CommonFunctions.UpdateGenres(DatabaseConnection, req.GenreList, client, loggedInUser?.UserName, conn, transaction);
                    await CommonFunctions.UpdateCopyrightOwners(DatabaseConnection, req.CopyrightOwnerNameList, client, loggedInUser?.UserName, conn, transaction);
                    await CommonFunctions.AddTypeOfClient(req.TypeOfClient, client, conn, transaction);
                    client.Active = true;
                    client.IsApproved = false;
                    client.UpdatedOn = DateTime.UtcNow;
                    await conn.UpdateAsync(client, transaction);
                    await transaction.CommitAsync();
                    return Results.Ok("Client successfully Updated!");
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
            .WithTags("3. Add/Update Clients and Assets")
            .WithMetadata(new SwaggerOperationAttribute("Update Clients.", "Updates a previously added client."));

            return app;
        }
    }
}
