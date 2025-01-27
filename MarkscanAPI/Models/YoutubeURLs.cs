using Dapper;
using Dapper.Contrib.Extensions;
using DbAccess;
using System.ComponentModel.DataAnnotations.Schema;

namespace MarkscanAPI.Models
{
    [System.ComponentModel.DataAnnotations.Schema.Table("YoutubeURLs")]
    public class YoutubeURLs
    {

        [Column("SourceURL")]
        public string? SourceURL { get; set; }
        [Computed]
        [Write(false)]
        public string? AssetName { get; set; }
        [Column("RemovalStatus")]
        public string? RemovalStatus { get; set; }

        [Column("IsChannelSuspended")]
        public bool IsChannelSuspended { get; set; }
        [Column("UploadDate")]
        public DateTime? UploadDate { get; set; }
        [Column("ViewCount")]
        public string? ViewCount { get; set; }
        [Column("LikeCount")]
        public string? LikeCount { get; set; }


        public static async Task<IEnumerable<YoutubeURLs>> GetURLsForClient(IDatabaseConnection databaseConnection, string? ClientId, DateTime StartDate, DateTime? EndDate)
        {
            using var conn = databaseConnection.GetConnection();
            return await conn.QueryAsync<YoutubeURLs>(@"Select i.SourceURL,A.AssetName AssetName, i.UploadDate, i.ViewCount, i.LikeCount, i.RemovalStatus, i.IsChannelSuspended from YoutubeURLs i
                        inner join Asset A on A.id = i.AssetId and A.Active=1 and i.Active=1
                        join ClientMaster cl on cl.Id=A.ClientMasterId and cl.Active=1 and cl.Id=@ClientId
                        where i.URLUploadDate >= @YTStartDate and i.URLUploadDate<= @YTEndDate and  i.IsInvalidURL = 0;"
                        , new { ClientId, YTStartDate = StartDate.AddDays(-1).ToString("yyyy-MM-dd") + " 18:30:00", YTEndDate = EndDate?.ToString("yyyy-MM-dd") + " 18:30:00", commandTimeout = 3000 });
        }
    }
}
