using Dapper;
using Dapper.Contrib.Extensions;
using DbAccess;
using MarkscanAPI.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace MarkscanAPI.Models
{
    [System.ComponentModel.DataAnnotations.Schema.Table("ClientMarkscanAPI")]
    public class ClientMarkscanAPI : BaseFields
    {
        [Column("CompanyName")]
        public string? CompanyName { get; set; }
        [Column("ClientTypeId")]
        public string? ClientTypeId { get; set; }
        /*[Computed]
        public string? CopyrightOwnerNames { get; set; }
        [Computed]
        public string? GenreIds { get; set; }*/
        [Column("UserName")]
        public string? UserName { get; set; }
        [Column("IsApproved")]
        public bool IsApproved { get; set; }


        public static async Task<IEnumerable<ClientMarkscanAPI>> GetAllClients(IDatabaseConnection databaseConnection)
        {
            using var conn = databaseConnection.GetConnection();
            return await conn.QueryAsync<ClientMarkscanAPI>(@"select * from ClientMarkscanAPI where Active=1;");
        }
        public static async Task<IEnumerable<ClientMarkscanAPI>> GetAllClientsByUserName(IDatabaseConnection databaseConnection, string? UserName)
        {
            using var conn = databaseConnection.GetConnection();
            return await conn.QueryAsync<ClientMarkscanAPI>(@"select * from ClientMarkscanAPI where Active=1 and UserName=@UserName;", new { UserName });
        }
        public static async Task<ClientMarkscanAPI> GetClientByCompanyNameAndUserName(IDatabaseConnection databaseConnection, string? CompanyName, string? UserName)
        {
            using var conn = databaseConnection.GetConnection();
            return await conn.QueryFirstOrDefaultAsync<ClientMarkscanAPI>(@"select * from ClientMarkscanAPI where Active=1 and UserName=@UserName and CompanyName=@CompanyName;", new { CompanyName, UserName });
        }
        public static async Task<ClientMarkscanAPI> GetActiveInactiveClientByCompanyNameAndUserName(IDatabaseConnection databaseConnection, string? CompanyName, string? UserName)
        {
            using var conn = databaseConnection.GetConnection();
            return await conn.QueryFirstOrDefaultAsync<ClientMarkscanAPI>(@"select * from ClientMarkscanAPI where UserName=@UserName and CompanyName=@CompanyName;", new { CompanyName, UserName });
        }
    }
}
