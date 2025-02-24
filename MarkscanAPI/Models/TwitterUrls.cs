using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;
using System.Xml.Linq;
using System;
using DbAccess;
using Dapper;
using Dapper.Contrib.Extensions;
using System.ComponentModel.DataAnnotations.Schema;

namespace MarkscanAPI.Models
{
    [System.ComponentModel.DataAnnotations.Schema.Table("TwitterURLsNEW")]
    public class TwitterURLsNEW
    {
        [Computed]
        [Write(false)]
        public string? AssetName { get; set; }
        [Column("SourceURLLink")]
        public string? SourceURLLink { get; set; }
        [Column("ProfileURL")]
        public string? ProfileURL { get; set; }
        [Column("ViewCount")]
        public string? ViewCount { get; set; }
        [Column("VideoDuration")]
        public string? VideoDuration { get; set; }
        [Column("RetweetCount")]
        public string? RetweetCount { get; set; }
        [Column("LikeCount")]
        public string? LikeCount { get; set; }
        [Column("PublishedOn")]
        public DateTime? PublishedOn { get; set; }
        [Column("Username")]
        public string? Username { get; set; }
        [Column("UserFullName")]
        public string? UserFullName { get; set; }
        [Column("Title")]
        public string? Title { get; set; }
        [Computed]
        [Write(false)]
        public string? SignPostURLs { get; set; }
        [Computed]
        [Write(false)]
        public string? Country { get; set; }
        [Computed]
        [Write(false)]
        public string? InfringementType { get; set; }
        [Computed]
        [Write(false)]
        public string? QualityOfPrint { get; set; }
        [Computed]
        [Write(false)]
        public string? AudioLanguage { get; set; }
        [Column("Keywords")]
        public string? Keywords { get; set; }
        [Column("Season")]
        public string? Season { get; set; }
        [Column("Episode")]
        public string? Episode { get; set; }

        public static async Task<IEnumerable<TwitterURLsNEW>> GetURLsForClient(IDatabaseConnection databaseConnection, string? ClientId, DateTime StartDate, DateTime? EndDate, string? AssetName)
        {
            try
            {
                using var conn = databaseConnection.GetConnection();
                if (string.IsNullOrEmpty(AssetName))
                {
                    return await conn.QueryAsync<TwitterURLsNEW>(@"Select i.source_url_link SourceURLLink,A.AssetName AssetName,it.Name InfringementType, convert_tz(i.upload_date,'+00:00','+05:30') PublishedOn, i.view_count ViewCount, i.like_count LikeCount,i.retweet_count RetweetCount,i.Title,
                            i.UserName,i.UserFullName,i.ProfileURL,i.VideoLength VideoDuration,qp.Name QualityOfPrint,pus.SignPostURL,lng.Name AudioLanguage,i.Keywords, cn.Name Country,i.Season,i.Episode from TwitterURLsNEW i
                            inner join Asset A on A.id = i.AssetId and A.Active=1 and i.Active=1
                            join ClientMaster cl on cl.Id=A.ClientMasterId and cl.Active=1 and cl.Id=@ClientId
                            left join InfringmentType it on i.InfringementTypeId  =it.Id and it.Active=1
                            left join Countries cn on i.CountryId=cn.Id and cn.Active
                            left join Language lng on i.LanguageId=lng.Id and lng.Active=1
                            left join QualityOfPrint qp on i.QualityOfPrintId=qp.Id and qp.Active=1
                            Left Join PlatformUrlSignPostURLs pus on pus.UrlId=i.Id and pus.PlatformId='0265483E-B288-11ED-A6F5-00155D03A4B9' and pus.Active =1
                            where i.upload_date >= @TWStartDate and i.upload_date<= @TWEndDate and  i.IsInvalidURL = 0;"
                                , new { ClientId, TWStartDate = StartDate.AddDays(-1).ToString("yyyy-MM-dd") + " 18:30:00", TWEndDate = EndDate?.ToString("yyyy-MM-dd") + " 18:30:00", commandTimeout = 3000 });
                }
                else
                {
                    var assetId = await conn.QueryFirstOrDefaultAsync<string>(@"select Id from Asset where lower(AssetName)=lower(@AssetName)", new { AssetName });
                    return await conn.QueryAsync<TwitterURLsNEW>(@"Select i.source_url_link SourceURLLink,A.AssetName AssetName,it.Name InfringementType, convert_tz(i.upload_date,'+00:00','+05:30') PublishedOn, i.view_count ViewCount, i.like_count LikeCount,i.retweet_count RetweetCount,i.Title,
                            i.UserName,i.UserFullName,i.ProfileURL,i.VideoLength VideoDuration,qp.Name QualityOfPrint,pus.SignPostURL,lng.Name AudioLanguage,i.Keywords, cn.Name Country,i.Season,i.Episode from TwitterURLsNEW i
                            inner join Asset A on A.id = i.AssetId and A.Active=1 and i.Active=1 and AssetId=@assetId
                            join ClientMaster cl on cl.Id=A.ClientMasterId and cl.Active=1 and cl.Id=@ClientId
                            left join InfringmentType it on i.InfringementTypeId  =it.Id and it.Active=1
                            left join Countries cn on i.CountryId=cn.Id and cn.Active
                            left join Language lng on i.LanguageId=lng.Id and lng.Active=1
                            left join QualityOfPrint qp on i.QualityOfPrintId=qp.Id and qp.Active=1
                            Left Join PlatformUrlSignPostURLs pus on pus.UrlId=i.Id and pus.PlatformId='0265483E-B288-11ED-A6F5-00155D03A4B9' and pus.Active =1
                            where i.upload_date >= @TWStartDate and i.upload_date<= @TWEndDate and  i.IsInvalidURL = 0;"
                                , new { ClientId, TWStartDate = StartDate.AddDays(-1).ToString("yyyy-MM-dd") + " 18:30:00", TWEndDate = EndDate?.ToString("yyyy-MM-dd") + " 18:30:00", assetId, commandTimeout = 3000 });
                }
            }
            catch (Exception ex)// inner join Asset A on A.id = i.AssetId and A.Active=1 and i.Active=1 and AssetId=@assetId
            {
                return null;
            }
        }
    }
}
