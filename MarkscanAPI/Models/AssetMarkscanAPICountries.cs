using Dapper;
using Dapper.Contrib.Extensions;
using MarkscanAPI.Common;
using MySqlConnector;
using System.ComponentModel.DataAnnotations.Schema;

namespace MarkscanAPI.Models
{
    [System.ComponentModel.DataAnnotations.Schema.Table("AssetMarkscanAPICountries")]
    public class AssetMarkscanAPICountries : BaseFields
    {
        [Column("AssetMarkscanAPIId")]
        public string? AssetMarkscanAPIId { get; set; }

        [Column("CountryId")]
        public string? CountryId { get; set; }
        [Computed]
        public string? Name { get; set; }

        public static async Task<IEnumerable<AssetMarkscanAPICountries>> GetActiveInactiveCountriesByAssetId(MySqlConnection? conn, string? AssetId, MySqlTransaction? transaction = null)
        {
            return await conn.QueryAsync<AssetMarkscanAPICountries>(@"select * from AssetMarkscanAPICountries where AssetMarkscanAPIId=@AssetId", new { AssetId }, transaction: transaction);
        }
        public static async Task<IEnumerable<AssetMarkscanAPICountries>> GetCountriesByAssetId(MySqlConnection? conn, string? AssetId, MySqlTransaction? transaction = null)
        {
            return await conn.QueryAsync<AssetMarkscanAPICountries>(@"select *, c.Name Name from AssetMarkscanAPICountries apic
                join Countries c on c.Id=apic.CountryId and c.Active=1 and apic.Active=1 and apic.AssetMarkscanAPIId=@AssetId", new { AssetId }, transaction: transaction);
        }
    }
}
