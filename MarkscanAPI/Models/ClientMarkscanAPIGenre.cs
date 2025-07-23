using Dapper;
using MarkscanAPI.Common;
using MySqlConnector;
using System.ComponentModel.DataAnnotations.Schema;

namespace MarkscanAPI.Models
{
    [Table("ClientMarkscanAPIGenre")]
    public class ClientMarkscanAPIGenre  : BaseFields
    {
        [Column("ClientMarkscanAPIId")]
        public string? ClientMarkscanAPIId { get; set; }
        [Column("GenreId")]
        public string? GenreId { get; set; }

        public static async Task<IEnumerable<ClientMarkscanAPIGenre>> GetAllGenres(MySqlConnection? conn, MySqlTransaction? transaction = null)
        {
            return await conn.QueryAsync<ClientMarkscanAPIGenre>(@"select * from ClientMarkscanAPIGenre where Active=1;", transaction: transaction);
        }
        public static async Task<IEnumerable<ClientMarkscanAPIGenre>> GetActiveInactiveGenresForClient(MySqlConnection? conn, MySqlTransaction? transaction, string? ClientId)
        {
            return await conn.QueryAsync<ClientMarkscanAPIGenre>(@"select * from ClientMarkscanAPIGenre where ClientMarkscanAPIId=@ClientId;", new { ClientId }, transaction: transaction);
        }
        public static async Task<IEnumerable<ClientMarkscanAPIGenre>> GetGenresForClient(MySqlConnection? conn, string? ClientId)
        {
            return await conn.QueryAsync<ClientMarkscanAPIGenre>(@"select * from ClientMarkscanAPIGenre where Active=1 and ClientMarkscanAPIId=@ClientId;", new { ClientId });
        }
        public static async Task<ClientMarkscanAPIGenre> GetGenreForClientByGenreId(MySqlConnection? conn, string? ClientId, string? GenreId)
        {
            return await conn.QueryFirstOrDefaultAsync<ClientMarkscanAPIGenre>(@"select * from ClientMarkscanAPIGenre where Active=1 and ClientMarkscanAPIId=@ClientId and GenreId=@GenreId;", new { ClientId, GenreId });
        }
    }
}
