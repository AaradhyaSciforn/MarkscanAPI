using Dapper;
using DbAccess;
using MarkscanAPI.Common;
using MySqlConnector;
using System.ComponentModel.DataAnnotations.Schema;

namespace MarkscanAPI.Models
{
    [Table("AssetMarkscanAPI")]
    public class AssetMarkscanAPI : BaseFields
    {
        [Column("ClientMarkscanAPIId")]
        public string? ClientMarkscanAPIId { get; set; }

        [Column("AssetName")]
        public string? AssetName { get; set; }

        [Column("CopyrightOwnerClientId")]
        public string? CopyrightOwnerClientId { get; set; }

        [Column("GenreId")]
        public string? GenreId { get; set; }
        [Column("SubGenreId")]
        public string? SubGenreId { get; set; }

        [Column("OfficialURL")]
        public string? OfficialURL { get; set; }

        [Column("StartDate")]
        public DateTime? StartDate { get; set; }

        [Column("EndDate")]
        public DateTime? EndDate { get; set; }

        [Column("ReleaseDate")]
        public DateTime? ReleaseDate { get; set; }

        [Column("RightsExpiryDate")]
        public DateTime? RightsExpiryDate { get; set; }

        [Column("OttBroadcastReleaseDateTime")]
        public DateTime? OttBroadcastReleaseDateTime { get; set; }

        [Column("OttReleaseDateTime")]
        public DateTime? OttReleaseDateTime { get; set; }

        [Column("ImdbId")]
        public string? ImdbId { get; set; }

        [Column("ReleaseYear")]
        public int? ReleaseYear { get; set; }

        [Column("IsAssetExclusive")]
        public bool IsAssetExclusive { get; set; }

        [Column("IsMonitoringOn")]
        public bool IsMonitoringOn { get; set; }
        [Column("IsApproved")]
        public bool IsApproved { get; set; }
        [Column("Timezonecode")]
        public string? Timezonecode { get; set; }
        [Column("BroadCastDay")]
        public string? BroadCastDay { get; set; }

        public static async Task<IEnumerable<AssetMarkscanAPI>> GetAssetsByClientId(IDatabaseConnection databaseConnection, string? ClientId)
        {
            using var conn = databaseConnection.GetConnection();
            return await conn.QueryAsync<AssetMarkscanAPI>(@"select * from AssetMarkscanAPI where Active=1 and ClientMarkscanAPIId=@ClientId", new { ClientId });
        }
        public static async Task<IEnumerable<AssetMarkscanAPI>> GetAssetsByCopyrightId(string? CopyrightOwnerClientId, MySqlConnection? conn, MySqlTransaction? transacation = null)
        {
            return await conn.QueryAsync<AssetMarkscanAPI>(@"select * from AssetMarkscanAPI where Active=1 and CopyrightOwnerClientId=@CopyrightOwnerClientId", new { CopyrightOwnerClientId }, transaction: transacation);
        }
        public static async Task<AssetMarkscanAPI> GetAssetByAssetNameAndClientId(IDatabaseConnection databaseConnection, string? AssetName, string? ClientId)
        {
            using var conn = databaseConnection.GetConnection();
            return await conn.QueryFirstOrDefaultAsync<AssetMarkscanAPI>(@"select * from AssetMarkscanAPI where Active=1 and ClientMarkscanAPIId=@ClientId and AssetName=@AssetName;", new { ClientId, AssetName });
        }
        public static async Task<AssetMarkscanAPI> GetActiveInactiveAssetByAssetNameAndClientId(IDatabaseConnection databaseConnection, string? AssetName, string? ClientId)
        {
            using var conn = databaseConnection.GetConnection();
            return await conn.QueryFirstOrDefaultAsync<AssetMarkscanAPI>(@"select * from AssetMarkscanAPI where ClientMarkscanAPIId=@ClientId and AssetName=@AssetName;", new { ClientId, AssetName });
        }
    }
}
