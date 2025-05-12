using Dapper;
using Dapper.Contrib.Extensions;
using DbAccess;
using MarkscanAPI.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace MarkscanAPI.Models
{
    [System.ComponentModel.DataAnnotations.Schema.Table("Credentials")]
    public class Credentials : BaseFields
    {

        [Column("UserName")]
        public string? UserName { get; set; }

        [Column("Passwordhash")]
        public string? Passwordhash { get; set; }

        [Column("CredentialTypeId")]
        public string? CredentialTypeId { get; set; }
        [Column("Role")]
        public string? Role { get; set; }
        [Computed]
        public string? ClientId { get; set; }

        [Computed]
        [Write(false)]
        public string? Password { get; set; }

        [Computed]
        [Write(false)]
        public string? CredentialTypeName { get; set; }

        public static async Task<IEnumerable<Credentials>> GetallCredentials(IDatabaseConnection databaseConnection)
        {
            using var connection = databaseConnection.GetConnection();
            return await connection.QueryAsync<Credentials>(@"Select c.*, ct.Name CredentialTypeName from Credentials c inner join CredentialType ct on ct.id = c.CredentialTypeId and  c.Active = 1 and ct.Active=1;");
        }

        public static async Task<IEnumerable<Credentials>> GetallCredentialsByCredentialTypeId(IDatabaseConnection databaseConnection, string? CredentialTypeId)
        {
            using var connection = databaseConnection.GetConnection();
            return await connection.QueryAsync<Credentials>(@"Select * from Credentials where CredentialTypeId=@CredentialTypeId and  Active = 1;", new { CredentialTypeId });
        }

        public static async Task<Credentials> GetCredentialsById(IDatabaseConnection databaseConnection, string? Id)
        {
            using var connection = databaseConnection.GetConnection();
            return await connection.QueryFirstOrDefaultAsync<Credentials>(@"Select 
             Ip.* From Credentials Ip 
             where Ip.Id = @Id and Ip.Active=1"
            , new { Id });
        }

        public static async Task<IEnumerable<Credentials>> GetGoogleServiceCredentials(IDatabaseConnection databaseConnection)
        {
            try
            {
                using var connection = databaseConnection.GetConnection();
                return await connection.QueryAsync<Credentials>(@"Select * from Credentials WHERE CredentialTypeId = 'A906A214-2266-11EF-A3F0-00155DA69640' and active = 1;");
            }

            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static async Task<Credentials> GetCompleteCredentialsDetailById(IDatabaseConnection databaseConnection, string? Id)
        {
            try
            {
                using var connection = databaseConnection.GetConnection();
                var credentialDetail = await connection.QueryFirstOrDefaultAsync<Credentials>(@"Select Ip.* From Credentials Ip where Ip.Id = @Id and Ip.Active=1"
                , new { Id });
                if (credentialDetail != null && !string.IsNullOrWhiteSpace(credentialDetail.Passwordhash))
                {
                    credentialDetail.Password = CommonFunctions.DecryptString(credentialDetail.Passwordhash);

                }

                return credentialDetail;
            }
            catch(Exception ex)
            {
                throw new Exception("Something Went Wrong in Credentials Table. " + ex.Message);
            }
        }

        public static async Task<IEnumerable<Credentials>> GetallInactiveCredentials(IDatabaseConnection databaseConnection)
        {
            using var connection = databaseConnection.GetConnection();
            return await connection.QueryAsync<Credentials>(@"Select g.* from Credentials g where g.Active=0;");
        }

        public static async Task<IEnumerable<Credentials>> GetallInactiveGoogleServiceCredentials(IDatabaseConnection databaseConnection)
        {
            using var connection = databaseConnection.GetConnection();
            return await connection.QueryAsync<Credentials>(@"Select g.* from Credentials g where g.Active=0 and g.CredentialTypeId = 'A906A214-2266-11EF-A3F0-00155DA69640';");
        }

        public static async Task<Credentials> GetUserDetails(IDatabaseConnection databaseConnection, string? UserName)
        {
            using var conn = databaseConnection.GetConnection();
            return await conn.QueryFirstOrDefaultAsync<Credentials>(@"select * from Credentials where UserName=@UserName;", new { UserName });
        }

    }
}
