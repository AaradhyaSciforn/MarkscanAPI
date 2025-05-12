using Dapper;
using Dapper.Contrib.Extensions;
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
    public static class AssetEndpoints
    {
        public static IEndpointRouteBuilder MapAssetEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapGet("/GetAllAssetsForClient/{CompanyName}", async ([Required] string? CompanyName,
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
                    var Client = await ClientMarkscanAPI.GetClientByCompanyNameAndUserName(DatabaseConnection, CompanyName, loggedInUser.UserName);
                    if (Client == null)
                    {
                        return Results.BadRequest($"No Client present with CompanyName: {CompanyName}!");
                    }

                    var AssetList = await AssetMarkscanAPI.GetAssetsByClientId(DatabaseConnection, Client.Id);

                    using var conn = DatabaseConnection.GetConnection();
                    var MapGenre = (await conn.QueryAsync<IdNameClass>(@"select Id, Name from GenreMS where Active=1;")).ToDictionary(x => x.Id, x => x.Name);
                    var MapCopyrightOwner = (await conn.QueryAsync<IdNameClass>(@"select Id, Name from ClientMarkscanAPICopyrightOwner where Active=1;")).ToDictionary(x => x.Id, x => x.Name);
                    var MapOriginLanguage = (await conn.QueryAsync<IdNameClass>(@"select AssetMarkscanAPIId Id, l.Name Name from AssetMarkscanAPIOriginLanguage apil
                        join Language l on l.Id=apil.LanguageId and l.Active=1 and apil.Active=1;")).GroupBy(x => x.Id).ToDictionary(x => x.Key, x=> x.ToList());
                    var MapContentLanguage = (await conn.QueryAsync<IdNameClass>(@"select AssetMarkscanAPIId Id, l.Name Name from AssetMarkscanAPIContentLanguage apil
                        join Language l on l.Id=apil.LanguageId and l.Active=1 and apil.Active=1;")).GroupBy(x => x.Id).ToDictionary(x => x.Key, x => x.ToList());
                    var MapCountries = (await conn.QueryAsync<IdNameClass>(@"select AssetMarkscanAPIId Id, c.Name Name from AssetMarkscanAPICountries apic
                        join Countries c on c.Id=apic.CountryId and c.Active=1 and apic.Active=1;")).GroupBy(x => x.Id).ToDictionary(x => x.Key, x=> x.ToList());
                    var MapExclusiveCountries = (await conn.QueryAsync<IdNameClass>(@"select AssetMarkscanAPIId Id, c.Name Name from AssetMarkscanAPIExclusiveCountries apic
                        join Countries c on c.Id=apic.CountryId and c.Active=1 and apic.Active=1;")).GroupBy(x => x.Id).ToDictionary(x => x.Key, x => x.ToList());
                    List<AssetRequest_DTO> AssetToShowList = new();

                    if (AssetList != null && AssetList.Any())
                    {
                        foreach (var asset in AssetList)
                        {
                            AssetRequest_DTO _asset = new();
                            _asset.AssetName = asset.AssetName;
                            _asset.CompanyName = Client.CompanyName;
                            _asset.CopyrightOwnerName = MapCopyrightOwner[asset.CopyrightOwnerClientId!];
                            _asset.GenreName = MapGenre[asset.GenreId!];
                            _asset.OfficialURL = asset.OfficialURL;
                            _asset.StartDate = asset.StartDate;
                            _asset.EndDate = asset.EndDate;
                            _asset.ReleaseDate = asset.ReleaseDate;
                            _asset.RightsExpiryDate = asset.RightsExpiryDate;
                            _asset.OttBroadcastReleaseDateTime = asset.OttBroadcastReleaseDateTime;
                            _asset.OttReleaseDateTime = asset.OttReleaseDateTime;
                            _asset.ImdbId = asset.ImdbId;
                            _asset.ReleaseYear = asset.ReleaseYear;
                            _asset.IsAssetExclusive = asset.IsAssetExclusive;
                            _asset.IsMonitoringOn = asset.IsMonitoringOn;
                            _asset.IsApproved = asset.IsApproved;
                            //lists
                            _asset.OriginLanguageList = MapOriginLanguage[asset.Id!].Select(x => x.Name).ToList();
                            _asset.ContentLanguageList = MapContentLanguage[asset.Id!].Select(x => x.Name).ToList();
                            _asset.CountryList = MapCountries[asset.Id!].Select(x => x.Name).ToList();
                            _asset.ExclusiveCountryList = MapExclusiveCountries[asset.Id!].Select(x => x.Name).ToList();
                            AssetToShowList.Add(_asset);
                        }

                    }
                    return Results.Ok(AssetToShowList);
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
            .WithMetadata(new SwaggerOperationAttribute("Get all Assets.", "Fetches all the clients, previously sent through API."));

            app.MapGet("/GetAsset/{CompanyName}/{AssetName}", async ([Required] string? CompanyName, [Required] string? AssetName,
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
                    var client = await ClientMarkscanAPI.GetClientByCompanyNameAndUserName(DatabaseConnection, CompanyName, loggedInUser.UserName);
                    if (client == null)
                    {
                        return Results.BadRequest($"No Client with CompanyName: {CompanyName} exists!");
                    }
                    using var conn = DatabaseConnection.GetConnection();
                    var asset = await AssetMarkscanAPI.GetAssetByAssetNameAndClientId(DatabaseConnection, AssetName, client.Id);
                    if (asset == null)
                    {
                        return Results.BadRequest($"No AssetName: {AssetName} found for the Client: {CompanyName}!");
                    }

                    var owner = await ClientMarkscanAPICopyrightOwner.GetCopyrightOwnersForClientByCopyrightOwnerId(conn, client.Id, asset.CopyrightOwnerClientId);
                    var genre = await conn.QueryFirstOrDefaultAsync<IdNameClass>(@"select Id,Name from GenreMS where Active=1 and Id=@GenreId", new { GenreId = asset.GenreId });
                    var originLangList = await AssetMarkscanAPIOriginLanguage.GetOriginLanguagesByAssetId(conn, asset.Id);
                    var contentLangList = await AssetMarkscanAPIContentLanguage.GetContentLanguagesByAssetId(conn, asset.Id);
                    var countryList = await AssetMarkscanAPICountries.GetCountriesByAssetId(conn, asset.Id);
                    var exclusivecountryList = await AssetMarkscanAPIExclusiveCountries.GetExclusiveCountriesByAssetId(conn, asset.Id);

                    AssetRequest_DTO _asset = new();
                    _asset.AssetName = asset.AssetName;
                    _asset.CompanyName = client.CompanyName;
                    _asset.CopyrightOwnerName = owner.Name;
                    _asset.GenreName = genre.Name;
                    _asset.OfficialURL = asset.OfficialURL;
                    _asset.StartDate = asset.StartDate;
                    _asset.EndDate = asset.EndDate;
                    _asset.ReleaseDate = asset.ReleaseDate;
                    _asset.RightsExpiryDate = asset.RightsExpiryDate;
                    _asset.OttBroadcastReleaseDateTime = asset.OttBroadcastReleaseDateTime;
                    _asset.OttReleaseDateTime = asset.OttReleaseDateTime;
                    _asset.ImdbId = asset.ImdbId;
                    _asset.ReleaseYear = asset.ReleaseYear;
                    _asset.IsAssetExclusive = asset.IsAssetExclusive;
                    _asset.IsMonitoringOn = asset.IsMonitoringOn;
                    _asset.IsApproved = asset.IsApproved;
                    //lists
                    _asset.OriginLanguageList = originLangList.Select(x => x.Name).ToList();
                    _asset.ContentLanguageList = contentLangList.Select(x => x.Name).ToList();
                    _asset.CountryList = countryList.Select(x => x.Name).ToList();
                    _asset.ExclusiveCountryList = exclusivecountryList.Select(x => x.Name).ToList();
                        
                    return Results.Ok(_asset);
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
            .WithMetadata(new SwaggerOperationAttribute("Get all Assets.", "Fetches all the clients, previously sent through API."));

            app.MapPost("/AddAsset", async ([FromBody] AssetRequest_DTO req,
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
                    if (string.IsNullOrWhiteSpace(req.AssetName))
                    {
                        return Results.BadRequest("AssetName cannot be empty!");
                    }
                    if (string.IsNullOrWhiteSpace(req.CopyrightOwnerName))
                    {
                        return Results.BadRequest("CopyrightOwnerName cannot be empty!");
                    }
                    if (string.IsNullOrWhiteSpace(req.GenreName))
                    {
                        return Results.BadRequest("GenreName cannot be empty!");
                    }
                    if (string.IsNullOrWhiteSpace(req.OfficialURL))
                    {
                        return Results.BadRequest("OfficialURL cannot be empty!");
                    }
                    var client = await ClientMarkscanAPI.GetClientByCompanyNameAndUserName(DatabaseConnection, req.CompanyName, loggedInUser.UserName);
                    if (client == null)
                    {
                        return Results.BadRequest($"No Client with CompanyName: {req.CompanyName} exists!");
                    }
                    using var conn = DatabaseConnection.GetConnection();
                    var owner = await ClientMarkscanAPICopyrightOwner.GetCopyrightOwnersForClientByCopyrightOwnerName(conn, client.Id, req.CopyrightOwnerName);
                    if (owner == null)
                    {
                        return Results.BadRequest($"No CopyrightOwner with Name: {req.CopyrightOwnerName} exists for the Client: {client.CompanyName}!");
                    }
                    var Genre = await conn.QueryFirstOrDefaultAsync<IdNameClass>(@"select * from GenreMS where Name=@GenreName", new { GenreName = req.GenreName });
                    if (Genre == null)
                    {
                        return Results.BadRequest($"No Genre with name: {req.GenreName} exists!");
                    }
                    var genre = await ClientMarkscanAPIGenre.GetGenreForClientByGenreId(conn, client.Id, Genre.Id);
                    if (genre == null)
                    {
                        return Results.BadRequest($"No Genre with name: {req.GenreName} present for the mentioned Client!");
                    }
                    var asset = await AssetMarkscanAPI.GetAssetByAssetNameAndClientId(DatabaseConnection, req.AssetName, client.Id);
                    if (asset != null)
                    {
                        return Results.BadRequest($"Asset Already present and is {((asset.IsApproved) ? "Approved" : "Pending for Approval")}!");
                    }

                    await conn.OpenAsync();
                    using var transaction = await conn.BeginTransactionAsync();

                    AssetMarkscanAPI _asset = new();
                    CommonFunctions.SetBaseFields(_asset, loggedInUser?.UserName);
                    _asset.ClientMarkscanAPIId = client.Id;
                    _asset.AssetName = req.AssetName;
                    _asset.CopyrightOwnerClientId = owner.Id;
                    _asset.GenreId = genre.GenreId;
                    _asset.OfficialURL = req.OfficialURL;
                    _asset.StartDate = req.StartDate;
                    _asset.EndDate = req.EndDate;
                    _asset.ReleaseDate = req.ReleaseDate;
                    _asset.RightsExpiryDate = req.RightsExpiryDate;
                    _asset.OttBroadcastReleaseDateTime = req.OttBroadcastReleaseDateTime;
                    _asset.OttReleaseDateTime = req.OttReleaseDateTime;
                    _asset.ImdbId = ((string.IsNullOrWhiteSpace(req.ImdbId) || req.ImdbId == "string") ? null : req.ImdbId);
                    _asset.ReleaseYear = req.ReleaseYear;
                    _asset.IsAssetExclusive = req.IsAssetExclusive;
                    _asset.IsMonitoringOn = req.IsMonitoringOn;
                    await conn.InsertAsync(_asset, transaction);
                    await CommonFunctions.AddOriginLanguage(req.OriginLanguageList, _asset, loggedInUser?.UserName, conn, transaction);
                    await CommonFunctions.AddContentLanguage(req.ContentLanguageList, _asset, loggedInUser?.UserName, conn, transaction);
                    await CommonFunctions.AddCountries(req.CountryList, _asset, loggedInUser?.UserName, conn, transaction);
                    await CommonFunctions.AddExclusiveCountries(req.ExclusiveCountryList, _asset, loggedInUser?.UserName, conn, transaction);
                    await transaction.CommitAsync();
                    return Results.Ok("Asset successfully created!");
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
            .WithMetadata(new SwaggerOperationAttribute("Add Assets.", "Sends a post request to add client."));

            app.MapPut("/UpdateAssetDetails", async ([FromBody] AssetRequest_DTO req,
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
                    if (string.IsNullOrWhiteSpace(req.AssetName))
                    {
                        return Results.BadRequest("AssetName cannot be empty!");
                    }
                    if (string.IsNullOrWhiteSpace(req.CopyrightOwnerName))
                    {
                        return Results.BadRequest("CopyrightOwnerName cannot be empty!");
                    }
                    if (string.IsNullOrWhiteSpace(req.GenreName))
                    {
                        return Results.BadRequest("GenreName cannot be empty!");
                    }
                    if (string.IsNullOrWhiteSpace(req.OfficialURL))
                    {
                        return Results.BadRequest("OfficialURL cannot be empty!");
                    }
                    var client = await ClientMarkscanAPI.GetClientByCompanyNameAndUserName(DatabaseConnection, req.CompanyName, loggedInUser.UserName);
                    if (client == null)
                    {
                        return Results.BadRequest($"No Client with CompanyName: {req.CompanyName} exists!");
                    }
                    using var conn = DatabaseConnection.GetConnection();
                    var owner = await ClientMarkscanAPICopyrightOwner.GetCopyrightOwnersForClientByCopyrightOwnerName(conn, client.Id, req.CopyrightOwnerName);
                    if (owner == null)
                    {
                        return Results.BadRequest($"No CopyrightOwner with Name: {req.CopyrightOwnerName} exists for the Client: {client.CompanyName}!");
                    }
                    var Genre = await conn.QueryFirstOrDefaultAsync<IdNameClass>(@"select * from GenreMS where Name=@GenreName", new { GenreName = req.GenreName });
                    if (Genre == null)
                    {
                        return Results.BadRequest($"No Genre with name: {req.GenreName} exists!");
                    }
                    var genre = await ClientMarkscanAPIGenre.GetGenreForClientByGenreId(conn, client.Id, Genre.Id);
                    if (genre == null)
                    {
                        return Results.BadRequest($"No Genre with name: {req.GenreName} present for the Client: {req.CompanyName}!");
                    }
                    var asset = await AssetMarkscanAPI.GetActiveInactiveAssetByAssetNameAndClientId(DatabaseConnection, req.AssetName, client.Id);
                    if (asset == null)
                    {
                        return Results.BadRequest($"No AssetName: {req.AssetName} found to update for the Client: {req.CompanyName}!");
                    }

                    await conn.OpenAsync();
                    using var transaction = await conn.BeginTransactionAsync();

                    asset.Active = true;
                    asset.IsApproved = false;
                    asset.UpdatedOn = DateTime.UtcNow;
                    asset.ClientMarkscanAPIId = client.Id;
                    asset.AssetName = req.AssetName;
                    asset.CopyrightOwnerClientId = owner.Id;
                    asset.GenreId = genre.GenreId;
                    asset.OfficialURL = req.OfficialURL;
                    asset.StartDate = req.StartDate;
                    asset.EndDate = req.EndDate;
                    asset.ReleaseDate = req.ReleaseDate;
                    asset.RightsExpiryDate = req.RightsExpiryDate;
                    asset.OttBroadcastReleaseDateTime = req.OttBroadcastReleaseDateTime;
                    asset.OttReleaseDateTime = req.OttReleaseDateTime;
                    asset.ImdbId = ((string.IsNullOrWhiteSpace(req.ImdbId) || req.ImdbId == "string") ? null : req.ImdbId);
                    asset.ReleaseYear = req.ReleaseYear;
                    asset.IsAssetExclusive = req.IsAssetExclusive;
                    asset.IsMonitoringOn = req.IsMonitoringOn;
                    await conn.UpdateAsync(asset, transaction);
                    await CommonFunctions.UpdateOriginLanguage(req.OriginLanguageList, asset, loggedInUser?.UserName, conn, transaction);
                    await CommonFunctions.UpdateContentLanguage(req.ContentLanguageList, asset, loggedInUser?.UserName, conn, transaction);
                    await CommonFunctions.UpdateCountries(req.CountryList, asset, loggedInUser?.UserName, conn, transaction);
                    await CommonFunctions.UpdateExclusiveCountries(req.ExclusiveCountryList, asset, loggedInUser?.UserName, conn, transaction);
                    await transaction.CommitAsync();
                    return Results.Ok("Asset successfully updated!");
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
            .WithMetadata(new SwaggerOperationAttribute("Update Assets.", "Updates a previously added client."));


            app.MapPut("/UpdateAssetDetails/{CompanyName}/{AssetName}", async ([Required] string CompanyName, [Required] string AssetName, [FromBody] MonitoringStatusClass req,
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
                        return Results.BadRequest("Company Name cannot be empty!");
                    }
                    if (string.IsNullOrWhiteSpace(AssetName))
                    {
                        return Results.BadRequest("AssetName cannot be empty!");
                    }
                    var client = await ClientMarkscanAPI.GetClientByCompanyNameAndUserName(DatabaseConnection, CompanyName, loggedInUser.UserName);
                    if (client == null)
                    {
                        return Results.BadRequest($"No Client with CompanyName: {CompanyName} exists!");
                    }
                    using var conn = DatabaseConnection.GetConnection();
                    var asset = await AssetMarkscanAPI.GetAssetByAssetNameAndClientId(DatabaseConnection, AssetName, client.Id);
                    if (asset == null)
                    {
                        return Results.BadRequest($"No AssetName: {AssetName} found to update for the Client: {CompanyName}!");
                    }
                    asset.IsMonitoringOn = req.IsMonitoring;
                    await conn.UpdateAsync(asset);
                    return Results.Ok("Monitoring Status Updated successfully!");
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
            .WithMetadata(new SwaggerOperationAttribute("Update Assets.", "Updates a previously added client."));

            return app;
        }
    }
}
