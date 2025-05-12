using Dapper;
using Dapper.Contrib.Extensions;
using MarkscanAPI.Common;
using MySqlConnector;
using System.ComponentModel.DataAnnotations.Schema;

namespace MarkscanAPI.Models
{
    [System.ComponentModel.DataAnnotations.Schema.Table("AssetMarkscanAPIContentLanguage")]
    public class AssetMarkscanAPIContentLanguage : BaseFields
    {
        [Column("AssetMarkscanAPIId")]
        public string? AssetMarkscanAPIId { get; set; }

        [Column("LanguageId")]
        public string? LanguageId { get; set; }
        [Computed]
        public string? Name { get; set; }

        public static async Task<IEnumerable<AssetMarkscanAPIContentLanguage>> GetActiveInactiveContentLanguageByAssetId(MySqlConnection? conn, string? AssetId, MySqlTransaction? transaction = null)
        {
            return await conn.QueryAsync<AssetMarkscanAPIContentLanguage>(@"select * from AssetMarkscanAPIContentLanguage where AssetMarkscanAPIId=@AssetId", new { AssetId }, transaction: transaction);
        }
        public static async Task<IEnumerable<AssetMarkscanAPIContentLanguage>> GetContentLanguagesByAssetId(MySqlConnection? conn, string? AssetId, MySqlTransaction? transaction = null)
        {
            return await conn.QueryAsync<AssetMarkscanAPIContentLanguage>(@"select *, l.Name Name from AssetMarkscanAPIContentLanguage apil
                join Language l on l.Id=apil.LanguageId and l.Active=1 and apil.Active=1 and apil.AssetMarkscanAPIId=@AssetId", new { AssetId }, transaction: transaction);
        }
    }
}
