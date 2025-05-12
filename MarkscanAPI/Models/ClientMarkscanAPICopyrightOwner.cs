using Dapper;
using MarkscanAPI.Common;
using MySqlConnector;
using System.ComponentModel.DataAnnotations.Schema;

namespace MarkscanAPI.Models
{
    [Table("ClientMarkscanAPICopyrightOwner")]
    public class ClientMarkscanAPICopyrightOwner : BaseFields
    {
        [Column("ClientMarkscanAPIId")]
        public string? ClientMarkscanAPIId { get; set; }
        [Column("Name")]
        public string? Name { get; set; }

        public static async Task<IEnumerable<ClientMarkscanAPICopyrightOwner>> GetActiveInactiveCopyrightOwnersForClient(MySqlConnection? conn, MySqlTransaction? transaction, string? ClientId)
        {
            return await conn.QueryAsync<ClientMarkscanAPICopyrightOwner>(@"select * from ClientMarkscanAPICopyrightOwner where ClientMarkscanAPIId=@ClientId;", new { ClientId }, transaction: transaction);
        }
        public static async Task<IEnumerable<ClientMarkscanAPICopyrightOwner>> GetCopyrightOwnersForClient(MySqlConnection? conn, string? ClientId)
        {
            return await conn.QueryAsync<ClientMarkscanAPICopyrightOwner>(@"select * from ClientMarkscanAPICopyrightOwner where Active=1 and ClientMarkscanAPIId=@ClientId;", new { ClientId });
        }
        public static async Task<ClientMarkscanAPICopyrightOwner> GetCopyrightOwnersForClientByCopyrightOwnerName(MySqlConnection? conn, string? ClientId, string? CopyrightOwnerName)
        {
            return await conn.QueryFirstOrDefaultAsync<ClientMarkscanAPICopyrightOwner>(@"select * from ClientMarkscanAPICopyrightOwner where Active=1 and ClientMarkscanAPIId=@ClientId and Name=@CopyrightOwnerName;", new { ClientId, CopyrightOwnerName });
        }
        public static async Task<ClientMarkscanAPICopyrightOwner> GetCopyrightOwnersForClientByCopyrightOwnerId(MySqlConnection? conn, string? ClientId, string? CopyrightOwnerId)
        {
            return await conn.QueryFirstOrDefaultAsync<ClientMarkscanAPICopyrightOwner>(@"select * from ClientMarkscanAPICopyrightOwner where Active=1 and ClientMarkscanAPIId=@ClientId and Id=@CopyrightOwnerId;", new { ClientId, CopyrightOwnerId });
        }
    }
}
