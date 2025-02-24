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

        [Computed]
        [Write(false)]
        public string? InfringementType { get; set; }

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

        [Column("dislikeCount")]
        public int? dislikeCount { get; set; }

        [Column("SubscriberCount")]
        public int? SubscriberCount { get; set; }

        [Column("CommentCount")]
        public int? CommentCount { get; set; }

        [Column("FavouriteCount")]
        public int? FavouriteCount { get; set; }

        [Column("VideoId")]
        public string? VideoId { get; set; }

        [Column("VideoName")]
        public string? VideoName { get; set; }

        [Column("VideoDuration")]
        public string? VideoDuration { get; set; }

        [Computed]
        [Write(false)]
        public string? QualityOfPrint { get; set; }

        [Column("ChannelName")]
        public string? ChannelName { get; set; }

        [Column("ChannelId")]
        public string? ChannelId { get; set; }


        [Computed]
        [Write(false)]
        public string? Language { get; set; }

        [Column("Keywords")]
        public string? Keywords { get; set; }

        [Computed]
        [Write(false)]
        public string? Country { get; set; }

        [Column("Season")]
        public string? Season { get; set; }

        [Column("Episode")]
        public string? Episode { get; set; }





        public static async Task<IEnumerable<YoutubeURLs>> GetURLsForClient(IDatabaseConnection databaseConnection, string? ClientId, DateTime StartDate, DateTime? EndDate, string? AssetName)
        {
            using var conn = databaseConnection.GetConnection();
            if (string.IsNullOrEmpty(AssetName))
            {
                return await conn.QueryAsync<YoutubeURLs>(@"Select i.SourceURL,A.AssetName AssetName,it.Name InfringementType, convert_tz(i.UploadDate,'+00:00','+05:30') UploadDate, i.ViewCount, i.LikeCount, i.RemovalStatus, i.IsChannelSuspended,i.dislikeCount,i.SubscriberCount,i.CommentCount,
                            i.FavouriteCount,i.VideoId,i.VideoName,i.VideoDuration,qp.Name QualityOfPrint,i.ChannelName,lng.Name Language,i.Keywords, cn.Name Country,i.Season,i.Episode from YoutubeURLs i
                            inner join Asset A on A.id = i.AssetId and A.Active=1 and i.Active=1
                            join ClientMaster cl on cl.Id=A.ClientMasterId and cl.Active=1 and cl.Id=@ClientId
                            left join InfringmentType it on i.InfringmentTypeId  =it.Id and it.Active=1
                            left join Countries cn on i.CountryId=cn.Id and cn.Active
                            left join Language lng on i.LanguageId=lng.Id and lng.Active=1
                            left join QualityOfPrint qp on i.QualityOfPrintId=qp.Id and qp.Active=1
                            where i.UploadDate >= @YTStartDate and i.UploadDate<= @YTEndDate and  i.IsInvalidURL = 0;"
                            , new { ClientId, YTStartDate = StartDate.AddDays(-1).ToString("yyyy-MM-dd") + " 18:30:00", YTEndDate = EndDate?.ToString("yyyy-MM-dd") + " 18:30:00", commandTimeout = 3000 });
            }
            else
            {
                var assetId = await conn.QueryFirstOrDefaultAsync<string>(@"select Id from Asset where lower(AssetName)=lower(@AssetName)", new { AssetName });
                return await conn.QueryAsync<YoutubeURLs>(@"Select i.SourceURL,A.AssetName AssetName,it.Name InfringementType, convert_tz(i.UploadDate,'+00:00','+05:30') UploadDate, i.ViewCount, i.LikeCount, i.RemovalStatus, i.IsChannelSuspended,i.dislikeCount,i.SubscriberCount,i.CommentCount,
                            i.FavouriteCount,i.VideoId,i.VideoName,i.VideoDuration,qp.Name QualityOfPrint,i.ChannelName,lng.Name Language,i.Keywords, cn.Name Country,i.Season,i.Episode from YoutubeURLs i
                            inner join Asset A on A.id = i.AssetId and A.Active=1 and i.Active=1 and AssetId=@assetId
                            join ClientMaster cl on cl.Id=A.ClientMasterId and cl.Active=1 and cl.Id=@ClientId
                            left join InfringmentType it on i.InfringmentTypeId  =it.Id and it.Active=1
                            left join Countries cn on i.CountryId=cn.Id and cn.Active
                            left join Language lng on i.LanguageId=lng.Id and lng.Active=1
                            left join QualityOfPrint qp on i.QualityOfPrintId=qp.Id and qp.Active=1
                            where i.UploadDate >= @YTStartDate and i.UploadDate<= @YTEndDate and  i.IsInvalidURL = 0;"
                            , new { ClientId, YTStartDate = StartDate.AddDays(-1).ToString("yyyy-MM-dd") + " 18:30:00", YTEndDate = EndDate?.ToString("yyyy-MM-dd") + " 18:30:00", assetId, commandTimeout = 3000 });
            }
        }
    }
}
