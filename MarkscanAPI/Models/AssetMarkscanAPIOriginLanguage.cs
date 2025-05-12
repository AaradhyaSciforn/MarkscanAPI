using Dapper;
using Dapper.Contrib.Extensions;
using MarkscanAPI.Common;
using MySqlConnector;
using System.ComponentModel.DataAnnotations.Schema;

namespace MarkscanAPI.Models
{
    [System.ComponentModel.DataAnnotations.Schema.Table("AssetMarkscanAPIOriginLanguage")]
    public class AssetMarkscanAPIOriginLanguage : BaseFields
    {
        [Column("AssetMarkscanAPIId")]
        public string? AssetMarkscanAPIId { get; set; }

        [Column("LanguageId")]
        public string? LanguageId { get; set; }
        [Computed]
        public string? Name { get; set; }

        public static async Task<IEnumerable<AssetMarkscanAPIOriginLanguage>> GetActiveInactiveOriginLanguageByAssetId(MySqlConnection? conn, string? AssetId, MySqlTransaction? transaction = null)
        {
            return await conn.QueryAsync<AssetMarkscanAPIOriginLanguage>(@"select * from AssetMarkscanAPIOriginLanguage where AssetMarkscanAPIId=@AssetId", new { AssetId }, transaction: transaction);
        }
        public static async Task<IEnumerable<AssetMarkscanAPIOriginLanguage>> GetOriginLanguagesByAssetId(MySqlConnection? conn, string? AssetId, MySqlTransaction? transaction = null)
        {
            return await conn.QueryAsync<AssetMarkscanAPIOriginLanguage>(@"select *, l.Name Name from AssetMarkscanAPIOriginLanguage apil
                join Language l on l.Id=apil.LanguageId and l.Active=1 and apil.Active=1 and apil.AssetMarkscanAPIId=@AssetId", new { AssetId }, transaction: transaction);
        }
    }
}
