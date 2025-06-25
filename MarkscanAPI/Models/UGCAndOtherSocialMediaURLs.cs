using Dapper;
using Dapper.Contrib.Extensions;
using DbAccess;
using System.ComponentModel.DataAnnotations.Schema;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MarkscanAPI.Models
{
    [System.ComponentModel.DataAnnotations.Schema.Table("UGCAndOtherSocialMediaURLs")]
    public class UGCAndOtherSocialMediaURLs
    {
        [Computed]
        [Write(false)]
        public string? AssetName { get; set; }

        [Column("VideoURL")]
        public string? VideoURL { get; set; }

        [Column("VideoTitle")]
        public string? VideoTitle { get; set; }
        [Column("Views")]
        public string? Views { get; set; }
        [Column("Duration")]
        public string? Duration { get; set; }
        [Computed]
        [Write(false)]
        public string? InfringementType { get; set; }
        [Column("Likes")]
        public string? Likes { get; set; }
        [Column("CommentCount")]
        public string? CommentCount { get; set; }
        [Column("PostUploadDate")]
        public DateTime? PostUploadDate { get; set; }
        [Column("URLUploadDate")]
        public DateTime? URLUploadDate { get; set; }
        [Column("Season")]
        public string? Season { get; set; }
        [Column("Episode")]
        public string? Episode { get; set; }
        [Computed]
        [Write(false)]
        public string? SignPostURL { get; set; }
        [Column("ChannelOrProfileURL")]
        public string? ChannelOrProfileURL { get; set; }
        [Column("ChannelOrProfileName")]
        public string? ChannelOrProfileName { get; set; }
        [Column("Subscriber")]
        public string? Subscriber { get; set; }
        [Computed]
        [Write(false)]
        public string? QualityOfPrint { get; set; }
        [Computed]
        [Write(false)]
        public string? Language { get; set; }
        [Computed]
        [Write(false)]
        public string? Country { get; set; }
        [Column("Keyword")]
        public string? Keyword { get; set; }

        public static async Task<IEnumerable<UGCAndOtherSocialMediaURLs>> GetURLsForClient(IDatabaseConnection databaseConnection, string? ClientId, DateTime StartDate, DateTime? EndDate, string? AssetName)
        {
            try
            {
                using var conn = databaseConnection.GetConnection();
                if (string.IsNullOrEmpty(AssetName))
                {
                    return await conn.QueryAsync<UGCAndOtherSocialMediaURLs>(@"
                            Select i.VideoURL VideoURL,i.VideoTitle VideoTitle,A.AssetName AssetName,it.Name InfringementType, i.PostUploadDate PostUploadDate, convert_tz(i.DiscoveryDoneAt,'+00:00','+05:30') URLUploadDate, i.Views Views, i.Likes Likes,i.CommentCount CommentCount,i.Subscriber,
                            i.ChannelOrProfileName,i.ChannelOrProfileURL,i.Duration Duration,qp.Name QualityOfPrint,pus.SignPostURL,lng.Name Language,i.Keyword, cn.Name Country,i.Season,i.Episode from UGCAndOtherSocialMediaURLs i
                            inner join Asset A on A.id = i.AssetId and A.Active=1 and i.Active=1 
                            join ClientMaster cl on cl.Id=A.ClientMasterId and cl.Active=1 and cl.Id=@ClientId
                            left join InfringmentType it on i.InfringmentTypeId  =it.Id and it.Active=1
                            left join Countries cn on i.CountryId=cn.Id and cn.Active
                            left join Language lng on i.LanguageId=lng.Id and lng.Active=1
                            left join QualityOfPrint qp on i.QualityOfPrintId=qp.Id and qp.Active=1
                            Left Join PlatformUrlSignPostURLs pus on pus.UrlId=i.Id and pus.PlatformId='33D60DB2-FAAA-45F2-9761-1B3882F4CB39' and pus.Active =1
                            where i.URLUploadDate >= @TWStartDate and i.URLUploadDate<= @TWEndDate and  i.IsInvalidURL = 0
                            union
                            Select i.VideoURL VideoURL,i.VideoTitle VideoTitle,A.AssetName AssetName,it.Name InfringementType, i.PostUploadDate PostUploadDate, convert_tz(i.DiscoveryDoneAt,'+00:00','+05:30') URLUploadDate, i.Views Views, i.Likes Likes,i.CommentCount CommentCount,i.Subscriber,
                            i.ChannelOrProfileName,i.ChannelOrProfileURL,i.Duration Duration,qp.Name QualityOfPrint,pus.SignPostURL,lng.Name Language,i.Keyword, cn.Name Country,i.Season,i.Episode from OkURLs i
                            inner join Asset A on A.id = i.AssetId and A.Active=1 and i.Active=1 
                            join ClientMaster cl on cl.Id=A.ClientMasterId and cl.Active=1 and cl.Id=@ClientId
                            left join InfringmentType it on i.InfringmentTypeId  =it.Id and it.Active=1
                            left join Countries cn on i.CountryId=cn.Id and cn.Active
                            left join Language lng on i.LanguageId=lng.Id and lng.Active=1
                            left join QualityOfPrint qp on i.QualityOfPrintId=qp.Id and qp.Active=1
                            Left Join PlatformUrlSignPostURLs pus on pus.UrlId=i.Id and pus.PlatformId='422EF444-8AD9-4DD6-8A41-D9F3670C735E' and pus.Active =1
                            where i.URLUploadDate >= @TWStartDate and i.URLUploadDate<= @TWEndDate and  i.IsInvalidURL = 0
                            union
                            Select i.VideoURL VideoURL,i.VideoTitle VideoTitle,A.AssetName AssetName,it.Name InfringementType, i.PostUploadDate PostUploadDate, convert_tz(i.DiscoveryDoneAt,'+00:00','+05:30') URLUploadDate, i.Views Views, i.Likes Likes,i.CommentCount CommentCount,i.Subscriber,
                            i.ChannelOrProfileName,i.ChannelOrProfileURL,i.Duration Duration,qp.Name QualityOfPrint,pus.SignPostURL,lng.Name Language,i.Keyword, cn.Name Country,i.Season,i.Episode from TiktokURLs i
                            inner join Asset A on A.id = i.AssetId and A.Active=1 and i.Active=1 
                            join ClientMaster cl on cl.Id=A.ClientMasterId and cl.Active=1 and cl.Id=@ClientId
                            left join InfringmentType it on i.InfringmentTypeId  =it.Id and it.Active=1
                            left join Countries cn on i.CountryId=cn.Id and cn.Active
                            left join Language lng on i.LanguageId=lng.Id and lng.Active=1
                            left join QualityOfPrint qp on i.QualityOfPrintId=qp.Id and qp.Active=1
                            Left Join PlatformUrlSignPostURLs pus on pus.UrlId=i.Id and pus.PlatformId='86FAC8E8-0F1F-4141-BA84-CD8711B72E39' and pus.Active =1
                            where i.URLUploadDate >= @TWStartDate and i.URLUploadDate<= @TWEndDate and  i.IsInvalidURL = 0
                            union
                            Select i.VideoURL VideoURL,i.VideoTitle VideoTitle,A.AssetName AssetName,it.Name InfringementType, i.PostUploadDate PostUploadDate, convert_tz(i.DiscoveryDoneAt,'+00:00','+05:30') URLUploadDate, i.Views Views, i.Likes Likes,i.CommentCount CommentCount,i.Subscriber,
                            i.ChannelOrProfileName,i.ChannelOrProfileURL,i.Duration Duration,qp.Name QualityOfPrint,pus.SignPostURL,lng.Name Language,i.Keyword, cn.Name Country,i.Season,i.Episode from ShareChatURLs i
                            inner join Asset A on A.id = i.AssetId and A.Active=1 and i.Active=1 
                            join ClientMaster cl on cl.Id=A.ClientMasterId and cl.Active=1 and cl.Id=@ClientId
                            left join InfringmentType it on i.InfringmentTypeId  =it.Id and it.Active=1
                            left join Countries cn on i.CountryId=cn.Id and cn.Active
                            left join Language lng on i.LanguageId=lng.Id and lng.Active=1
                            left join QualityOfPrint qp on i.QualityOfPrintId=qp.Id and qp.Active=1
                            Left Join PlatformUrlSignPostURLs pus on pus.UrlId=i.Id and pus.PlatformId='38750701-0627-48E0-B8B9-F48315DCE47F' and pus.Active =1
                            where i.URLUploadDate >= @TWStartDate and i.URLUploadDate<= @TWEndDate and  i.IsInvalidURL = 0
                            union
                            Select i.VideoURL VideoURL,i.VideoTitle VideoTitle,A.AssetName AssetName,it.Name InfringementType, i.PostUploadDate PostUploadDate, convert_tz(i.DiscoveryDoneAt,'+00:00','+05:30') URLUploadDate, i.Views Views, i.Likes Likes,i.CommentCount CommentCount,i.Subscriber,
                            i.ChannelOrProfileName,i.ChannelOrProfileURL,i.Duration Duration,qp.Name QualityOfPrint,pus.SignPostURL,lng.Name Language,i.Keyword, cn.Name Country,i.Season,i.Episode from VKURLs i
                            inner join Asset A on A.id = i.AssetId and A.Active=1 and i.Active=1 
                            join ClientMaster cl on cl.Id=A.ClientMasterId and cl.Active=1 and cl.Id=@ClientId
                            left join InfringmentType it on i.InfringmentTypeId  =it.Id and it.Active=1
                            left join Countries cn on i.CountryId=cn.Id and cn.Active
                            left join Language lng on i.LanguageId=lng.Id and lng.Active=1
                            left join QualityOfPrint qp on i.QualityOfPrintId=qp.Id and qp.Active=1
                            Left Join PlatformUrlSignPostURLs pus on pus.UrlId=i.Id and pus.PlatformId='2773562C-0B73-400F-8A4F-68C3F1B24BDE' and pus.Active =1
                            where i.URLUploadDate >= @TWStartDate and i.URLUploadDate<= @TWEndDate and  i.IsInvalidURL = 0
                            union
                            Select i.VideoURL VideoURL,i.VideoTitle VideoTitle,A.AssetName AssetName,it.Name InfringementType, i.PostUploadDate PostUploadDate, convert_tz(i.DiscoveryDoneAt,'+00:00','+05:30') URLUploadDate, i.Views Views, i.Likes Likes,i.CommentCount CommentCount,i.Subscriber,
                            i.ChannelOrProfileName,i.ChannelOrProfileURL,i.Duration Duration,qp.Name QualityOfPrint,pus.SignPostURL,lng.Name Language,i.Keyword, cn.Name Country,i.Season,i.Episode from DailymotionURLs i
                            inner join Asset A on A.id = i.AssetId and A.Active=1 and i.Active=1 
                            join ClientMaster cl on cl.Id=A.ClientMasterId and cl.Active=1 and cl.Id=@ClientId
                            left join InfringmentType it on i.InfringmentTypeId  =it.Id and it.Active=1
                            left join Countries cn on i.CountryId=cn.Id and cn.Active
                            left join Language lng on i.LanguageId=lng.Id and lng.Active=1
                            left join QualityOfPrint qp on i.QualityOfPrintId=qp.Id and qp.Active=1
                            Left Join PlatformUrlSignPostURLs pus on pus.UrlId=i.Id and pus.PlatformId='6FD30070-962C-4949-9100-F45A368039A0' and pus.Active =1
                            where i.URLUploadDate >= @TWStartDate and i.URLUploadDate<= @TWEndDate and  i.IsInvalidURL = 0
                            union
                            Select i.VideoURL VideoURL,i.VideoTitle VideoTitle,A.AssetName AssetName,it.Name InfringementType, i.PostUploadDate PostUploadDate, convert_tz(i.DiscoveryDoneAt,'+00:00','+05:30') URLUploadDate, i.Views Views, i.Likes Likes,i.CommentCount CommentCount,i.Subscriber,
                            i.ChannelOrProfileName,i.ChannelOrProfileURL,i.Duration Duration,qp.Name QualityOfPrint,pus.SignPostURL,lng.Name Language,i.Keyword, cn.Name Country,i.Season,i.Episode from ChomikujURLs i
                            inner join Asset A on A.id = i.AssetId and A.Active=1 and i.Active=1 
                            join ClientMaster cl on cl.Id=A.ClientMasterId and cl.Active=1 and cl.Id=@ClientId
                            left join InfringmentType it on i.InfringmentTypeId  =it.Id and it.Active=1
                            left join Countries cn on i.CountryId=cn.Id and cn.Active
                            left join Language lng on i.LanguageId=lng.Id and lng.Active=1
                            left join QualityOfPrint qp on i.QualityOfPrintId=qp.Id and qp.Active=1
                            Left Join PlatformUrlSignPostURLs pus on pus.UrlId=i.Id and pus.PlatformId='6D74B5F2-D086-4756-BD2E-F4C8F9993CB3' and pus.Active =1
                            where i.URLUploadDate >= @TWStartDate and i.URLUploadDate<= @TWEndDate and  i.IsInvalidURL = 0
                            union
                            Select i.VideoURL VideoURL,i.VideoTitle VideoTitle,A.AssetName AssetName,it.Name InfringementType, i.PostUploadDate PostUploadDate, convert_tz(i.DiscoveryDoneAt,'+00:00','+05:30') URLUploadDate, i.Views Views, i.Likes Likes,i.CommentCount CommentCount,i.Subscriber,
                            i.ChannelOrProfileName,i.ChannelOrProfileURL,i.Duration Duration,qp.Name QualityOfPrint,pus.SignPostURL,lng.Name Language,i.Keyword, cn.Name Country,i.Season,i.Episode from BiliBiliURLs i
                            inner join Asset A on A.id = i.AssetId and A.Active=1 and i.Active=1 
                            join ClientMaster cl on cl.Id=A.ClientMasterId and cl.Active=1 and cl.Id=@ClientId
                            left join InfringmentType it on i.InfringmentTypeId  =it.Id and it.Active=1
                            left join Countries cn on i.CountryId=cn.Id and cn.Active
                            left join Language lng on i.LanguageId=lng.Id and lng.Active=1
                            left join QualityOfPrint qp on i.QualityOfPrintId=qp.Id and qp.Active=1
                            Left Join PlatformUrlSignPostURLs pus on pus.UrlId=i.Id and pus.PlatformId='E7391B65-78D6-40EE-9723-E43BA778BDB8' and pus.Active =1
                            where i.URLUploadDate >= @TWStartDate and i.URLUploadDate<= @TWEndDate and  i.IsInvalidURL = 0;"
                                , new { ClientId, TWStartDate = StartDate.AddDays(-1).ToString("yyyy-MM-dd") + " 18:30:00", TWEndDate = EndDate?.ToString("yyyy-MM-dd") + " 18:30:00", commandTimeout = 3000 });
                }
                else
                {
                    var assetId = await conn.QueryFirstOrDefaultAsync<string>(@"select Id from Asset where lower(AssetName)=lower(@AssetName)", new { AssetName });
                    return await conn.QueryAsync<UGCAndOtherSocialMediaURLs>(@"
                            Select i.VideoURL VideoURL,i.VideoTitle VideoTitle,A.AssetName AssetName,it.Name InfringementType, i.PostUploadDate PostUploadDate, convert_tz(i.DiscoveryDoneAt,'+00:00','+05:30') URLUploadDate, i.Views Views, i.Likes Likes,i.CommentCount CommentCount,i.Subscriber,
                            i.ChannelOrProfileName,i.ChannelOrProfileURL,i.Duration Duration,qp.Name QualityOfPrint,pus.SignPostURL,lng.Name Language,i.Keyword, cn.Name Country,i.Season,i.Episode from UGCAndOtherSocialMediaURLs i
                            inner join Asset A on A.id = i.AssetId and A.Active=1 and i.Active=1 and AssetId=@assetId
                            join ClientMaster cl on cl.Id=A.ClientMasterId and cl.Active=1 and cl.Id=@ClientId
                            left join InfringmentType it on i.InfringmentTypeId  =it.Id and it.Active=1
                            left join Countries cn on i.CountryId=cn.Id and cn.Active
                            left join Language lng on i.LanguageId=lng.Id and lng.Active=1
                            left join QualityOfPrint qp on i.QualityOfPrintId=qp.Id and qp.Active=1
                            Left Join PlatformUrlSignPostURLs pus on pus.UrlId=i.Id and pus.PlatformId='33D60DB2-FAAA-45F2-9761-1B3882F4CB39' and pus.Active =1
                            where i.URLUploadDate >= @TWStartDate and i.URLUploadDate<= @TWEndDate and  i.IsInvalidURL = 0
                            union
                            Select i.VideoURL VideoURL,i.VideoTitle VideoTitle,A.AssetName AssetName,it.Name InfringementType, i.PostUploadDate PostUploadDate, convert_tz(i.DiscoveryDoneAt,'+00:00','+05:30') URLUploadDate, i.Views Views, i.Likes Likes,i.CommentCount CommentCount,i.Subscriber,
                            i.ChannelOrProfileName,i.ChannelOrProfileURL,i.Duration Duration,qp.Name QualityOfPrint,pus.SignPostURL,lng.Name Language,i.Keyword, cn.Name Country,i.Season,i.Episode from OkURLs i
                            inner join Asset A on A.id = i.AssetId and A.Active=1 and i.Active=1 and AssetId=@assetId
                            join ClientMaster cl on cl.Id=A.ClientMasterId and cl.Active=1 and cl.Id=@ClientId
                            left join InfringmentType it on i.InfringmentTypeId  =it.Id and it.Active=1
                            left join Countries cn on i.CountryId=cn.Id and cn.Active
                            left join Language lng on i.LanguageId=lng.Id and lng.Active=1
                            left join QualityOfPrint qp on i.QualityOfPrintId=qp.Id and qp.Active=1
                            Left Join PlatformUrlSignPostURLs pus on pus.UrlId=i.Id and pus.PlatformId='422EF444-8AD9-4DD6-8A41-D9F3670C735E' and pus.Active =1
                            where i.URLUploadDate >= @TWStartDate and i.URLUploadDate<= @TWEndDate and  i.IsInvalidURL = 0
                            union
                            Select i.VideoURL VideoURL,i.VideoTitle VideoTitle,A.AssetName AssetName,it.Name InfringementType, i.PostUploadDate PostUploadDate, convert_tz(i.DiscoveryDoneAt,'+00:00','+05:30') URLUploadDate, i.Views Views, i.Likes Likes,i.CommentCount CommentCount,i.Subscriber,
                            i.ChannelOrProfileName,i.ChannelOrProfileURL,i.Duration Duration,qp.Name QualityOfPrint,pus.SignPostURL,lng.Name Language,i.Keyword, cn.Name Country,i.Season,i.Episode from TiktokURLs i
                            inner join Asset A on A.id = i.AssetId and A.Active=1 and i.Active=1 and AssetId=@assetId
                            join ClientMaster cl on cl.Id=A.ClientMasterId and cl.Active=1 and cl.Id=@ClientId
                            left join InfringmentType it on i.InfringmentTypeId  =it.Id and it.Active=1
                            left join Countries cn on i.CountryId=cn.Id and cn.Active
                            left join Language lng on i.LanguageId=lng.Id and lng.Active=1
                            left join QualityOfPrint qp on i.QualityOfPrintId=qp.Id and qp.Active=1
                            Left Join PlatformUrlSignPostURLs pus on pus.UrlId=i.Id and pus.PlatformId='86FAC8E8-0F1F-4141-BA84-CD8711B72E39' and pus.Active =1
                            where i.URLUploadDate >= @TWStartDate and i.URLUploadDate<= @TWEndDate and  i.IsInvalidURL = 0
                            union
                            Select i.VideoURL VideoURL,i.VideoTitle VideoTitle,A.AssetName AssetName,it.Name InfringementType, i.PostUploadDate PostUploadDate, convert_tz(i.DiscoveryDoneAt,'+00:00','+05:30') URLUploadDate, i.Views Views, i.Likes Likes,i.CommentCount CommentCount,i.Subscriber,
                            i.ChannelOrProfileName,i.ChannelOrProfileURL,i.Duration Duration,qp.Name QualityOfPrint,pus.SignPostURL,lng.Name Language,i.Keyword, cn.Name Country,i.Season,i.Episode from ShareChatURLs i
                            inner join Asset A on A.id = i.AssetId and A.Active=1 and i.Active=1 and AssetId=@assetId
                            join ClientMaster cl on cl.Id=A.ClientMasterId and cl.Active=1 and cl.Id=@ClientId
                            left join InfringmentType it on i.InfringmentTypeId  =it.Id and it.Active=1
                            left join Countries cn on i.CountryId=cn.Id and cn.Active
                            left join Language lng on i.LanguageId=lng.Id and lng.Active=1
                            left join QualityOfPrint qp on i.QualityOfPrintId=qp.Id and qp.Active=1
                            Left Join PlatformUrlSignPostURLs pus on pus.UrlId=i.Id and pus.PlatformId='38750701-0627-48E0-B8B9-F48315DCE47F' and pus.Active =1
                            where i.URLUploadDate >= @TWStartDate and i.URLUploadDate<= @TWEndDate and  i.IsInvalidURL = 0
                            union
                            Select i.VideoURL VideoURL,i.VideoTitle VideoTitle,A.AssetName AssetName,it.Name InfringementType, i.PostUploadDate PostUploadDate, convert_tz(i.DiscoveryDoneAt,'+00:00','+05:30') URLUploadDate, i.Views Views, i.Likes Likes,i.CommentCount CommentCount,i.Subscriber,
                            i.ChannelOrProfileName,i.ChannelOrProfileURL,i.Duration Duration,qp.Name QualityOfPrint,pus.SignPostURL,lng.Name Language,i.Keyword, cn.Name Country,i.Season,i.Episode from VKURLs i
                            inner join Asset A on A.id = i.AssetId and A.Active=1 and i.Active=1 and AssetId=@assetId
                            join ClientMaster cl on cl.Id=A.ClientMasterId and cl.Active=1 and cl.Id=@ClientId
                            left join InfringmentType it on i.InfringmentTypeId  =it.Id and it.Active=1
                            left join Countries cn on i.CountryId=cn.Id and cn.Active
                            left join Language lng on i.LanguageId=lng.Id and lng.Active=1
                            left join QualityOfPrint qp on i.QualityOfPrintId=qp.Id and qp.Active=1
                            Left Join PlatformUrlSignPostURLs pus on pus.UrlId=i.Id and pus.PlatformId='2773562C-0B73-400F-8A4F-68C3F1B24BDE' and pus.Active =1
                            where i.URLUploadDate >= @TWStartDate and i.URLUploadDate<= @TWEndDate and  i.IsInvalidURL = 0
                            union
                            Select i.VideoURL VideoURL,i.VideoTitle VideoTitle,A.AssetName AssetName,it.Name InfringementType, i.PostUploadDate PostUploadDate, convert_tz(i.DiscoveryDoneAt,'+00:00','+05:30') URLUploadDate, i.Views Views, i.Likes Likes,i.CommentCount CommentCount,i.Subscriber,
                            i.ChannelOrProfileName,i.ChannelOrProfileURL,i.Duration Duration,qp.Name QualityOfPrint,pus.SignPostURL,lng.Name Language,i.Keyword, cn.Name Country,i.Season,i.Episode from DailymotionURLs i
                            inner join Asset A on A.id = i.AssetId and A.Active=1 and i.Active=1 and AssetId=@assetId
                            join ClientMaster cl on cl.Id=A.ClientMasterId and cl.Active=1 and cl.Id=@ClientId
                            left join InfringmentType it on i.InfringmentTypeId  =it.Id and it.Active=1
                            left join Countries cn on i.CountryId=cn.Id and cn.Active
                            left join Language lng on i.LanguageId=lng.Id and lng.Active=1
                            left join QualityOfPrint qp on i.QualityOfPrintId=qp.Id and qp.Active=1
                            Left Join PlatformUrlSignPostURLs pus on pus.UrlId=i.Id and pus.PlatformId='6FD30070-962C-4949-9100-F45A368039A0' and pus.Active =1
                            where i.URLUploadDate >= @TWStartDate and i.URLUploadDate<= @TWEndDate and  i.IsInvalidURL = 0
                            union
                            Select i.VideoURL VideoURL,i.VideoTitle VideoTitle,A.AssetName AssetName,it.Name InfringementType, i.PostUploadDate PostUploadDate, convert_tz(i.DiscoveryDoneAt,'+00:00','+05:30') URLUploadDate, i.Views Views, i.Likes Likes,i.CommentCount CommentCount,i.Subscriber,
                            i.ChannelOrProfileName,i.ChannelOrProfileURL,i.Duration Duration,qp.Name QualityOfPrint,pus.SignPostURL,lng.Name Language,i.Keyword, cn.Name Country,i.Season,i.Episode from ChomikujURLs i
                            inner join Asset A on A.id = i.AssetId and A.Active=1 and i.Active=1 and AssetId=@assetId
                            join ClientMaster cl on cl.Id=A.ClientMasterId and cl.Active=1 and cl.Id=@ClientId
                            left join InfringmentType it on i.InfringmentTypeId  =it.Id and it.Active=1
                            left join Countries cn on i.CountryId=cn.Id and cn.Active
                            left join Language lng on i.LanguageId=lng.Id and lng.Active=1
                            left join QualityOfPrint qp on i.QualityOfPrintId=qp.Id and qp.Active=1
                            Left Join PlatformUrlSignPostURLs pus on pus.UrlId=i.Id and pus.PlatformId='6D74B5F2-D086-4756-BD2E-F4C8F9993CB3' and pus.Active =1
                            where i.URLUploadDate >= @TWStartDate and i.URLUploadDate<= @TWEndDate and  i.IsInvalidURL = 0
                            union
                            Select i.VideoURL VideoURL,i.VideoTitle VideoTitle,A.AssetName AssetName,it.Name InfringementType, i.PostUploadDate PostUploadDate, convert_tz(i.DiscoveryDoneAt,'+00:00','+05:30') URLUploadDate, i.Views Views, i.Likes Likes,i.CommentCount CommentCount,i.Subscriber,
                            i.ChannelOrProfileName,i.ChannelOrProfileURL,i.Duration Duration,qp.Name QualityOfPrint,pus.SignPostURL,lng.Name Language,i.Keyword, cn.Name Country,i.Season,i.Episode from BiliBiliURLs i
                            inner join Asset A on A.id = i.AssetId and A.Active=1 and i.Active=1 and AssetId=@assetId
                            join ClientMaster cl on cl.Id=A.ClientMasterId and cl.Active=1 and cl.Id=@ClientId
                            left join InfringmentType it on i.InfringmentTypeId  =it.Id and it.Active=1
                            left join Countries cn on i.CountryId=cn.Id and cn.Active
                            left join Language lng on i.LanguageId=lng.Id and lng.Active=1
                            left join QualityOfPrint qp on i.QualityOfPrintId=qp.Id and qp.Active=1
                            Left Join PlatformUrlSignPostURLs pus on pus.UrlId=i.Id and pus.PlatformId='E7391B65-78D6-40EE-9723-E43BA778BDB8' and pus.Active =1
                            where i.URLUploadDate >= @TWStartDate and i.URLUploadDate<= @TWEndDate and  i.IsInvalidURL = 0;"
                                , new { ClientId, TWStartDate = StartDate.AddDays(-1).ToString("yyyy-MM-dd") + " 18:30:00", TWEndDate = EndDate?.ToString("yyyy-MM-dd") + " 18:30:00", assetId, commandTimeout = 3000 });
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
