using Dapper;
using Dapper.Contrib.Extensions;
using DbAccess;
using MarkscanAPI.Models;
using Microsoft.IdentityModel.Tokens;
using MySqlConnector;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Transactions;
using System.Web;



namespace MarkscanAPI.Common
{
    public class CommonFunctions : BaseFields
    {
        public static async Task AddOriginLanguage(List<string>? list, AssetMarkscanAPI asset, string? UserName, MySqlConnection? conn, MySqlTransaction? transaction)
        {
            try
            {
                if (list == null || !list.Any())
                {
                    return;
                }
                var MapOriginLanguage = (await conn.QueryAsync<IdNameClass>(@"select Id, Name from Language where Active=1;", transaction: transaction)).ToDictionary(x => x.Name, x => x.Id);
                foreach (var x in list)
                {
                    if (!MapOriginLanguage.ContainsKey(x))
                    {
                        throw new Exception($"OriginLanguage: {x} does not exist.");
                    }
                    AssetMarkscanAPIOriginLanguage obj = new();
                    SetBaseFields(obj, UserName);
                    obj.AssetMarkscanAPIId = asset.Id;
                    obj.LanguageId = MapOriginLanguage[x];
                    await conn.InsertAsync(obj, transaction);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public static async Task UpdateOriginLanguage(List<string>? list, AssetMarkscanAPI asset, string? UserName, MySqlConnection? conn, MySqlTransaction? transaction)
        {
            try
            {
                if (list == null || !list.Any())
                {
                    return;
                }
                var MapLanguage = (await conn.QueryAsync<IdNameClass>(@"select Id, Name from Language where Active=1;", transaction: transaction)).ToDictionary(x => x.Name, x => x.Id);
                var MapAddedLanguage = (await AssetMarkscanAPIOriginLanguage.GetActiveInactiveOriginLanguageByAssetId(conn, asset.Id, transaction))?.ToDictionary(x => x.LanguageId);
                List<string> IdList = new();
                foreach (var language in list)
                {
                    if (!MapLanguage.ContainsKey(language))
                    {
                        throw new Exception($"Language: {language} does not exist.");
                    }
                    AssetMarkscanAPIOriginLanguage _language = new();
                    SetBaseFields(_language, UserName);
                    _language.AssetMarkscanAPIId = asset.Id;
                    _language.LanguageId = MapLanguage[language];
                    IdList.Add(_language.LanguageId);
                    var alreadyAdded = MapAddedLanguage?.GetValueOrDefault(_language.LanguageId);
                    if (alreadyAdded != null)
                    {
                        alreadyAdded.UpdatedOn = DateTime.UtcNow;
                        alreadyAdded.Active = true;
                        await conn.UpdateAsync(alreadyAdded, transaction);
                    }
                    else
                    {
                        await conn.InsertAsync(_language, transaction);
                    }
                }
                if (MapAddedLanguage != null)
                {
                    foreach (var presentLangs in MapAddedLanguage)
                    {
                        if (!IdList.Where(x => x == presentLangs.Key).Any())
                        {
                            var langToDelete = presentLangs.Value;
                            langToDelete.Active = false;
                            langToDelete.UpdatedOn = DateTime.UtcNow;
                            await conn.UpdateAsync(langToDelete, transaction);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public static async Task AddContentLanguage(List<string>? list, AssetMarkscanAPI asset, string? UserName, MySqlConnection? conn, MySqlTransaction? transaction)
        {
            try
            {
                if (list == null || !list.Any())
                {
                    return;
                }
                var MapContentLanguage = (await conn.QueryAsync<IdNameClass>(@"select Id, Name from Language where Active=1;", transaction: transaction)).ToDictionary(x => x.Name, x => x.Id);
                foreach (var x in list)
                {
                    if (!MapContentLanguage.ContainsKey(x))
                    {
                        throw new Exception($"ContentLanguage: {x} does not exist.");
                    }
                    AssetMarkscanAPIContentLanguage obj = new();
                    SetBaseFields(obj, UserName);
                    obj.AssetMarkscanAPIId = asset.Id;
                    obj.LanguageId = MapContentLanguage[x];
                    await conn.InsertAsync(obj, transaction);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public static async Task UpdateContentLanguage(List<string>? list, AssetMarkscanAPI asset, string? UserName, MySqlConnection? conn, MySqlTransaction? transaction)
        {
            try
            {
                if (list == null || !list.Any())
                {
                    return;
                }
                var MapLanguage = (await conn.QueryAsync<IdNameClass>(@"select Id, Name from Language where Active=1;", transaction: transaction)).ToDictionary(x => x.Name, x => x.Id);
                var MapAddedLanguage = (await AssetMarkscanAPIContentLanguage.GetActiveInactiveContentLanguageByAssetId(conn, asset.Id, transaction))?.ToDictionary(x => x.LanguageId);
                List<string> IdList = new();
                foreach (var language in list)
                {
                    if (!MapLanguage.ContainsKey(language))
                    {
                        throw new Exception($"Language: {language} does not exist.");
                    }
                    AssetMarkscanAPIContentLanguage _language = new();
                    SetBaseFields(_language, UserName);
                    _language.AssetMarkscanAPIId = asset.Id;
                    _language.LanguageId = MapLanguage[language];
                    IdList.Add(_language.LanguageId);
                    var alreadyAdded = MapAddedLanguage?.GetValueOrDefault(_language.LanguageId);
                    if (alreadyAdded != null)
                    {
                        alreadyAdded.UpdatedOn = DateTime.UtcNow;
                        alreadyAdded.Active = true;
                        await conn.UpdateAsync(alreadyAdded, transaction);
                    }
                    else
                    {
                        await conn.InsertAsync(_language, transaction);
                    }
                }
                if (MapAddedLanguage != null)
                {
                    foreach (var presentLangs in MapAddedLanguage)
                    {
                        if (!IdList.Where(x => x == presentLangs.Key).Any())
                        {
                            var langToDelete = presentLangs.Value;
                            langToDelete.Active = false;
                            langToDelete.UpdatedOn = DateTime.UtcNow;
                            await conn.UpdateAsync(langToDelete, transaction);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public static async Task AddCountries(List<string>? list, AssetMarkscanAPI asset, string? UserName, MySqlConnection? conn, MySqlTransaction? transaction)
        {
            try
            {
                if (list == null || !list.Any())
                {
                    return;
                }
                var MapCountries = (await conn.QueryAsync<IdNameClass>(@"select Id, Name from Countries where Active=1;", transaction: transaction)).GroupBy(x => x.Name).ToDictionary(x => x.Key, x => x.First().Id);
                foreach (var x in list)
                {
                    if (!MapCountries.ContainsKey(x))
                    {
                        throw new Exception($"Country: {x} does not exist.");
                    }
                    AssetMarkscanAPICountries obj = new();
                    SetBaseFields(obj, UserName);
                    obj.AssetMarkscanAPIId = asset.Id;
                    obj.CountryId = MapCountries[x];
                    await conn.InsertAsync(obj, transaction);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public static async Task UpdateCountries(List<string>? list, AssetMarkscanAPI asset, string? UserName, MySqlConnection? conn, MySqlTransaction? transaction)
        {
            try
            {
                if (list == null || !list.Any())
                {
                    return;
                }
                var MapCountries = (await conn.QueryAsync<IdNameClass>(@"select Id, Name from Countries where Active=1;", transaction: transaction)).GroupBy(x => x.Name).ToDictionary(x => x.Key, x => x.First().Id);
                var MapAddedCountries = (await AssetMarkscanAPICountries.GetActiveInactiveCountriesByAssetId(conn, asset.Id, transaction))?.ToDictionary(x => x.CountryId);
                List<string> IdList = new();
                foreach (var country in list)
                {
                    if (!MapCountries.ContainsKey(country))
                    {
                        throw new Exception($"Country: {country} does not exist.");
                    }
                    AssetMarkscanAPICountries _country = new();
                    SetBaseFields(_country, UserName);
                    _country.AssetMarkscanAPIId = asset.Id;
                    _country.CountryId = MapCountries[country];
                    IdList.Add(_country.CountryId);
                    var alreadyAdded = MapAddedCountries?.GetValueOrDefault(_country.CountryId);
                    if (alreadyAdded != null)
                    {
                        alreadyAdded.UpdatedOn = DateTime.UtcNow;
                        alreadyAdded.Active = true;
                        await conn.UpdateAsync(alreadyAdded, transaction);
                    }
                    else
                    {
                        await conn.InsertAsync(_country, transaction);
                    }
                }
                if (MapAddedCountries != null)
                {
                    foreach (var presentCountry in MapAddedCountries)
                    {
                        if (!IdList.Where(x => x == presentCountry.Key).Any())
                        {
                            var countryToDelete = presentCountry.Value;
                            countryToDelete.Active = false;
                            countryToDelete.UpdatedOn = DateTime.UtcNow;
                            await conn.UpdateAsync(countryToDelete, transaction);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public static async Task AddExclusiveCountries(List<string>? list, AssetMarkscanAPI asset, string? UserName, MySqlConnection? conn, MySqlTransaction? transaction)
        {
            try
            {
                if (list == null || !list.Any())
                {
                    return;
                }
                var MapCountries = (await conn.QueryAsync<IdNameClass>(@"select Id, Name from Countries where Active=1;", transaction: transaction)).GroupBy(x => x.Name).ToDictionary(x => x.Key, x => x.First().Id);
                foreach (var x in list)
                {
                    if (!MapCountries.ContainsKey(x))
                    {
                        throw new Exception($"Country: {x} does not exist.");
                    }
                    AssetMarkscanAPIExclusiveCountries obj = new();
                    SetBaseFields(obj, UserName);
                    obj.AssetMarkscanAPIId = asset.Id;
                    obj.CountryId = MapCountries[x];
                    await conn.InsertAsync(obj, transaction);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public static async Task UpdateExclusiveCountries(List<string>? list, AssetMarkscanAPI asset, string? UserName, MySqlConnection? conn, MySqlTransaction? transaction)
        {
            try
            {
                if (list == null || !list.Any())
                {
                    return;
                }
                var MapCountries = (await conn.QueryAsync<IdNameClass>(@"select Id, Name from Countries where Active=1;", transaction: transaction)).GroupBy(x => x.Name).ToDictionary(x => x.Key, x => x.First().Id);
                var MapAddedCountries = (await AssetMarkscanAPIExclusiveCountries.GetActiveInactiveExclusiveCountriesByAssetId(conn, asset.Id, transaction))?.ToDictionary(x => x.CountryId);
                List<string> IdList = new();
                foreach (var country in list)
                {
                    if (!MapCountries.ContainsKey(country))
                    {
                        throw new Exception($"Country: {country} does not exist.");
                    }
                    AssetMarkscanAPIExclusiveCountries _country = new();
                    SetBaseFields(_country, UserName);
                    _country.AssetMarkscanAPIId = asset.Id;
                    _country.CountryId = MapCountries[country];
                    IdList.Add(_country.CountryId);
                    var alreadyAdded = MapAddedCountries?.GetValueOrDefault(_country.CountryId);
                    if (alreadyAdded != null)
                    {
                        alreadyAdded.UpdatedOn = DateTime.UtcNow;
                        alreadyAdded.Active = true;
                        await conn.UpdateAsync(alreadyAdded, transaction);
                    }
                    else
                    {
                        await conn.InsertAsync(_country, transaction);
                    }
                }
                if (MapAddedCountries != null)
                {
                    foreach (var presentCountry in MapAddedCountries)
                    {
                        if (!IdList.Where(x => x == presentCountry.Key).Any())
                        {
                            var countryToDelete = presentCountry.Value;
                            countryToDelete.Active = false;
                            countryToDelete.UpdatedOn = DateTime.UtcNow;
                            await conn.UpdateAsync(countryToDelete, transaction);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public static async Task AddTypeOfClient(string? TypeOfClient, ClientMarkscanAPI client, MySqlConnection? conn, MySqlTransaction? transaction)
        {
            try
            {
                var typeOfClientPresentId = await conn.QueryFirstOrDefaultAsync<string>(@"select Id from ClientType where Active=1 and Name=@Name", new { Name = TypeOfClient }, transaction: transaction);
                if (typeOfClientPresentId == null)
                {
                    throw new Exception($"Type of Client: {TypeOfClient} does not exist.");
                }
                client.ClientTypeId = typeOfClientPresentId;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public static async Task AddGenres(List<string> GenreList, ClientMarkscanAPI client, string? UserName, MySqlConnection? conn, MySqlTransaction? transaction)
        {
            if (GenreList == null || !GenreList.Any())
            {
                return;
            }
            try
            {
                var MapGenre = (await conn.QueryAsync<IdNameClass>(@"select Id, Name from GenreMS where Active=1;", transaction: transaction)).ToDictionary(x => x.Name, x => x.Id);
                //var MapAddedGenre = (await ClientMarkscanAPIGenre.GetActiveInactiveGenresForClient(conn, client.Id))?.ToDictionary(x => x.GenreId);
                foreach (var genre in GenreList)
                {
                    if (!MapGenre.ContainsKey(genre))
                    {
                        throw new Exception($"Genre: {genre} does not exist.");
                    }
                    ClientMarkscanAPIGenre _genre = new();
                    SetBaseFields(_genre, UserName);
                    _genre.ClientMarkscanAPIId = client.Id;
                    _genre.GenreId = MapGenre[genre];
                    await conn.InsertAsync(_genre, transaction);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public static async Task UpdateGenres(IDatabaseConnection databaseConnection, List<string> GenreList, ClientMarkscanAPI client, string? UserName, MySqlConnection? conn, MySqlTransaction? transaction)
        {
            try
            {
                var MapGenre = (await conn.QueryAsync<IdNameClass>(@"select Id, Name from GenreMS where Active=1;", transaction: transaction)).ToDictionary(x => x.Name, x => x.Id);
                var MapAddedGenre = (await ClientMarkscanAPIGenre.GetActiveInactiveGenresForClient(conn, transaction, client.Id))?.ToDictionary(x => x.GenreId);
                List<string> GenreIdList = new();
                foreach (var genre in GenreList)
                {
                    if (!MapGenre.ContainsKey(genre))
                    {
                        throw new Exception($"Genre: {genre} does not exist.");
                    }
                    ClientMarkscanAPIGenre _genre = new();
                    SetBaseFields(_genre, UserName);
                    _genre.ClientMarkscanAPIId = client.Id;
                    _genre.GenreId = MapGenre[genre];
                    GenreIdList.Add(_genre.GenreId);
                    var alreadyAdded = MapAddedGenre?.GetValueOrDefault(_genre.GenreId);
                    if (alreadyAdded != null)
                    {
                        alreadyAdded.UpdatedOn = DateTime.UtcNow;
                        alreadyAdded.Active = true;
                        await conn.UpdateAsync(alreadyAdded, transaction);
                    }
                    else
                    {
                        await conn.InsertAsync(_genre, transaction);
                    }
                }
                if (MapAddedGenre != null)
                {
                    foreach (var presentGenres in MapAddedGenre)
                    {
                        if (!GenreIdList.Where(x => x == presentGenres.Key).Any())
                        {
                            var genreToDelete = presentGenres.Value;
                            genreToDelete.Active = false;
                            genreToDelete.UpdatedOn = DateTime.UtcNow;
                            await conn.UpdateAsync(genreToDelete, transaction);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public static async Task UpdateCopyrightOwners(IDatabaseConnection databaseConnection, List<string> CopyrightOwnerNames, ClientMarkscanAPI client, string? UserName, MySqlConnection? conn, MySqlTransaction? transaction)
        {
            try
            {
                var MapAddedOwners = (await ClientMarkscanAPICopyrightOwner.GetActiveInactiveCopyrightOwnersForClient(conn, transaction, client.Id))?.ToDictionary(x => x.Name);
                foreach (var owner in CopyrightOwnerNames)
                {
                    ClientMarkscanAPICopyrightOwner _genre = new();
                    SetBaseFields(_genre, UserName);
                    _genre.ClientMarkscanAPIId = client.Id;
                    _genre.Name = owner;
                    var alreadyAdded = MapAddedOwners?.GetValueOrDefault(owner);
                    if (alreadyAdded != null)
                    {
                        alreadyAdded.UpdatedOn = DateTime.UtcNow;
                        alreadyAdded.Active = true;
                        await conn.UpdateAsync(alreadyAdded, transaction);
                    }
                    else
                    {
                        await conn.InsertAsync(_genre, transaction);
                    }
                }
                if (MapAddedOwners != null)
                {
                    foreach (var presentOwners in MapAddedOwners)
                    {
                        if (!CopyrightOwnerNames.Where(x => x == presentOwners.Key).Any())
                        {
                            var ownerToDelete = presentOwners.Value;
                            ownerToDelete.Active = false;
                            ownerToDelete.UpdatedOn = DateTime.UtcNow;
                            await makeAssetsInactiveByCopyrightOwner(ownerToDelete, conn, transaction);
                            await conn.UpdateAsync(ownerToDelete, transaction);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public static async Task makeAssetsInactiveByCopyrightOwner(ClientMarkscanAPICopyrightOwner? owner, MySqlConnection? conn, MySqlTransaction? transaction)
        {
            try
            {
                var assetList = await AssetMarkscanAPI.GetAssetsByCopyrightId(owner.Id, conn, transaction);
                if (assetList != null && assetList.Any())
                {
                    foreach (var asset in assetList)
                    {
                        asset.Active = false;
                        asset.UpdatedOn = DateTime.UtcNow;
                        await conn.UpdateAsync(asset, transaction);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public static async Task AddCopyrightOwners(List<string> CopyrightOwnerNames, ClientMarkscanAPI client, string? UserName, MySqlConnection? conn, MySqlTransaction? transaction)
        {
            try
            {
                List<ClientMarkscanAPICopyrightOwner> IdList = new();
                foreach (var owner in CopyrightOwnerNames)
                {
                    ClientMarkscanAPICopyrightOwner _owner = new();
                    SetBaseFields(_owner, UserName);
                    _owner.ClientMarkscanAPIId = client.Id;
                    _owner.Name = owner;
                    await conn.InsertAsync(_owner, transaction);
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public static DateTime ConvertUtcToIst(DateTime? dt)
        {
            DateTime utcdate = (dt == null) ? DateTime.UtcNow : dt.Value;
            var istdate = TimeZoneInfo.ConvertTimeFromUtc(utcdate,
            TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
            return istdate;
        }
        public static void SetBaseFields<T>(T obj, string? _user) where T : BaseFields
        {
            if (obj == null)
            {
                obj = (T)Activator.CreateInstance(typeof(T));
            }
            obj.Id = Guid.NewGuid().ToString().ToUpper();
            obj.Active = true;
            obj.UpdatedOn = DateTime.UtcNow;
            obj.UpdatedBy = _user;
        }
        public static string EncryptString(string text, string keyString = "F326C8D3278C45431069B5727695D4F1")
        {
            var key = Encoding.UTF8.GetBytes(keyString);

            using var aesAlg = Aes.Create();
            using var encryptor = aesAlg.CreateEncryptor(key, aesAlg.IV);
            using var msEncrypt = new MemoryStream();
            using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
            using (var swEncrypt = new StreamWriter(csEncrypt))
            {
                swEncrypt.Write(text);

            }

            var iv = aesAlg.IV;
            var decryptedContent = msEncrypt.ToArray();

            var result = new byte[iv.Length + decryptedContent.Length];

            Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
            Buffer.BlockCopy(decryptedContent, 0, result, iv.Length, decryptedContent.Length);

            return Convert.ToBase64String(result);
        }

        public static string DecryptString(string cipherText, string keyString = "F326C8D3278C45431069B5727695D4F1")
        {
            var fullCipher = Convert.FromBase64String(cipherText);

            var iv = new byte[16];
            var cipher = new byte[fullCipher.Length - iv.Length];

            Buffer.BlockCopy(fullCipher, 0, iv, 0, iv.Length);
            Buffer.BlockCopy(fullCipher, iv.Length, cipher, 0, cipher.Length);
            var key = Encoding.UTF8.GetBytes(keyString);

            using var aesAlg = Aes.Create();
            using var decryptor = aesAlg.CreateDecryptor(key, iv);
            string result;
            using (var msDecrypt = new MemoryStream(cipher))
            {
                using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
                using var srDecrypt = new StreamReader(csDecrypt);
                result = srDecrypt.ReadToEnd();
            }
            return result;
        }

    }
    public class IdNameClass
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
    public class SubGenreNameClass
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string GenreMSId { get; set; }
    }
    public class MonitoringStatusClass
    {
        public bool IsMonitoring { get; set; }
    }
    public class DetailsClass
    {
        public List<string>? ClientTypes { get; set; }
        public List<GenreDetails>? GenreDetails { get; set; }
        public List<string>? Languages { get; set; }
        public List<string>? Countries { get; set; }
    }
    public class GenreDetails
    {
        public string? GenreName { get; set; }
        public List<string>? SubGenreNames { get; set; }
    }
    public class GenreSubGenreName
    {
        public string? GenreName { get; set; }
        public string? SubGenreName { get; set; }
    }

}
