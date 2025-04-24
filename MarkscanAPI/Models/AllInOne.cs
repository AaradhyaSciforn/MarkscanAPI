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
    public class AllInOne
    {
        public string? Platform { get; set; }              
        public string? AssetName { get; set; }

        // URLs

        public string? VideoURL { get; set; }
        public string? ProfileURL { get; set; }
        public string? ChannelURL { get; set; }

        public string? SignPostURL { get; set; }

        // Metadata
        public string? VideoTitle { get; set; }

        public string? Caption { get; set; }

        public string? Keywords { get; set; }
        public string? InfringementType { get; set; }
        public string? QualityOfPrint { get; set; }

        // User / Profile Info
        public string? ProfileName { get; set; }
        public string? ChannelName { get; set; }
        public string? ChannelId { get; set; }
        public string? Username { get; set; }
 
        public string? UserFullName { get; set; }


        // Engagement Metrics

        public string? ViewCount { get; set; }
        public string? LikeCount { get; set; }

        public string? dislikeCount { get; set; }
        public string? FavouriteCount { get; set; }
        public string? Subscriber { get; set; }
  
        public string? SubscriberCount { get; set; }
        public string? FollowersCount { get; set; }

        public string? CommentCount { get; set; }
        public string? RetweetCount { get; set; }


        // Video Info
        public string? VideoId { get; set; }
        public string? VideoName { get; set; }
  
        public string? VideoDuration { get; set; }
   

        // Date Info
        public DateTime? UploadDate { get; set; }
        public DateTime? URLUploadDate { get; set; }

        public DateTime? ChannelCreationDate { get; set; }
  
        // Media Categorization
        public string? Category { get; set; }
        public string? Season { get; set; }
        public string? Episode { get; set; }

        // Languages
  
        public string? Language { get; set; }
        public string? Language1 { get; set; }
        public string? Language2 { get; set; }
        public string? Language3 { get; set; }
        public string? Language4 { get; set; }

        // Others
        public string? Country { get; set; }
        public string? RemovalStatus { get; set; }
        public bool? IsChannelSuspended { get; set; }

        public static async Task<IEnumerable<AllInOne>> GetURLsForClient(IDatabaseConnection databaseConnection, string? ClientId, DateTime StartDate, DateTime? EndDate, string? AssetName)
        {
            try {
                using var conn = databaseConnection.GetConnection();
                if (string.IsNullOrEmpty(AssetName))
                {
                    return await conn.QueryAsync<AllInOne>(@"
                           
Select 
 'TWITTER' Platform,
 i.source_url_link VideoURL,
 A.AssetName AssetName,
 it.Name InfringementType, 
 convert_tz(i.upload_date,'+00:00','+05:30') UploadDate,
 convert_tz(i.URLUploadDate,'+00:00','+05:30') URLUploadDate,
 i.view_count ViewCount,
 i.like_count LikeCount,
 '0' CommentCount,
 i.retweet_count RetweetCount,
 '0' dislikeCount,
 '0' SubscriberCount,
 '0' FavouriteCount,
  i.RemovalStatus,
 null IsChannelSuspended,     
 null VideoId,
 i.Title VideoTitle,
 i.UserName Username,
 i.UserFullName UserFullName,
 i.ProfileURL ProfileURL,
 i.VideoLength VideoDuration,
 qp.Name QualityOfPrint,
 null ChannelName,
 null ChannelId,
 null ChannelCreationDate,
null ChannelURL,
 pus.SignPostURL,
 lng.Name Language,
 null Language1,
 null Language2,
 null Language3,
 null Language4,
 i.Keywords, 
 cn.Name Country,
 i.Season,
 i.Episode 
 from TwitterURLsNEW i
 inner join Asset A on A.id = i.AssetId and A.Active=1 and i.Active=1
 join ClientMaster cl on cl.Id=A.ClientMasterId and cl.Active=1 and cl.Id=@ClientId
 left join InfringmentType it on i.InfringementTypeId  =it.Id and it.Active=1
 left join Countries cn on i.CountryId=cn.Id and cn.Active
 left join Language lng on i.LanguageId=lng.Id and lng.Active=1
 left join QualityOfPrint qp on i.QualityOfPrintId=qp.Id and qp.Active=1
 Left Join PlatformUrlSignPostURLs pus on pus.UrlId=i.Id and pus.PlatformId='0265483E-B288-11ED-A6F5-00155D03A4B9' and pus.Active =1
 where i.upload_date >= @TWStartDate and i.upload_date<= @TWEndDate and  i.IsInvalidURL = 0
         union all
 Select 
 'FACEBOOK' Platform,
 i.VideoURL VideoURL,
 A.AssetName AssetName,
 it.Name InfringementType, 
 convert_tz(i.publishedDate,'+00:00','+05:30') UploadDate,
 convert_tz(i.URLUploadDate,'+00:00','+05:30') URLUploadDate,
 i.Views ViewCount, 
 i.like_count LikeCount,
 i.comment_count CommentCount,
 '0' RetweetCount,
 '0' dislikeCount,
 '0' SubscriberCount,
 '0' FavouriteCount,
  i.RemovalStatus,
 null IsChannelSuspended,     
 null VideoId,
 i.VideoTitle VideoTitle,
 i.ProfileName Username,
 null UserFullName,
 i.ProfileURL ProfileURL,
 i.VideoLength VideoDuration,
 qp.Name QualityOfPrint,
 null ChannelName,
 null ChannelId,
 null ChannelCreationDate,
null ChannelURL,
 pus.SignPostURL,
 lng.Name Language,
 null Language1,
null Language2,
null Language3,
null Language4,
 i.Keywords, 
 cn.Name Country,
 i.Season,
 i.Episode 
 from FBUrlsNEW i
 inner join Asset A on A.id = i.AssetId and A.Active=1 and i.Active=1
 join ClientMaster cl on cl.Id=A.ClientMasterId and cl.Active=1 and cl.Id=@ClientId
 left join InfringmentType it on i.InfringementTypeId  =it.Id and it.Active=1
 left join Countries cn on i.CountryId=cn.Id and cn.Active
 left join Language lng on i.AudioLanguageId=lng.Id and lng.Active=1
 left join QualityOfPrint qp on i.QualityOfPrintId=qp.Id and qp.Active=1
 Left Join PlatformUrlSignPostURLs pus on pus.UrlId=i.Id and pus.PlatformId='F6A79626-B287-11ED-A6F5-00155D03A4B9' and pus.Active =1
 where i.publishedDate >= @TWStartDate and i.publishedDate<= @TWEndDate and  i.IsInvalidURL = 0
      union all
 Select
 'INSTAGRAM' Platform,
 i.VideoURL,
 A.AssetName AssetName,
 it.Name InfringementType, 
 convert_tz(i.PostDate,'+00:00','+05:30') UploadDate,
 convert_tz(i.URLUploadDate,'+00:00','+05:30') URLUploadDate,
 i.ViewCount, 
 i.LikeCount,
 i.CommentsCount,
 '0' RetweetCount,
 '0' dislikeCount,
 '0' SubscriberCount,
 '0' FavouriteCount,
  i.RemovalStatus,
 null IsChannelSuspended,     
 null VideoId,
 null VideoTitle,
 i.UserName Username,
 i.UserFullName,
 i.ProfileURL,
 i.VideoDuration,
 qp.Name QualityOfPrint,
 null ChannelName,
 null ChannelId,
 null ChannelCreationDate,
null ChannelURL,
 pus.SignPostURL,
 lng.Name Language,
 null Language1,
null Language2,
null Language3,
null Language4,
 i.Keywords,
 cn.Name Country,
 i.Season,
 i.Episode 
 from InstagramURLs i
 inner join Asset A on A.id = i.AssetId and A.Active=1 and i.Active=1
 join ClientMaster cl on cl.Id=A.ClientMasterId and cl.Active=1 and cl.Id=@ClientId
 left join InfringmentType it on i.InfringmentTypeId  =it.Id and it.Active=1
 left join Countries cn on i.CountryId=cn.Id and cn.Active
 left join Language lng on i.AudioLanguageId=lng.Id and lng.Active=1
 left join QualityOfPrint qp on i.QualityOfPrintId=qp.Id and qp.Active=1
 Left Join PlatformUrlSignPostURLs pus on pus.UrlId=i.Id and pus.PlatformId='1547A1E7-B288-11ED-A6F5-00155D03A4B9' and pus.Active =1
 where i.PostDate >= @TWStartDate and i.PostDate<= @TWEndDate and  i.IsInvalidURL = 0
      union all
 Select 
 'YOUTUBE' Platform,
 i.SourceURL VideoURL,
 A.AssetName AssetName,
 it.Name InfringementType,
 convert_tz(i.UploadDate,'+00:00','+05:30') UploadDate, 
 convert_tz(i.URLUploadDate,'+00:00','+05:30') URLUploadDate,
 i.ViewCount,
 i.LikeCount,
 i.CommentCount,
 '0' RetweetCount,
 i.dislikeCount,
 i.SubscriberCount,
 i.FavouriteCount,
 i.RemovalStatus,
 i.IsChannelSuspended,     
 i.VideoId,
 i.VideoName VideoTitle,
 null Username,
 null UserFullName,
 null ProfileURL,
 i.VideoDuration,
 qp.Name QualityOfPrint,
 i.ChannelName,
 i.ChannelId,
null ChannelCreationDate,
null ChannelURL,
 null SignPostURL,
 lng.Name Language,
null Language1,
null Language2,
null Language3,
null Language4,
 i.Keywords,
 cn.Name Country,
 i.Season,
 i.Episode
 from YoutubeURLs i
 inner join Asset A on A.id = i.AssetId and A.Active=1 and i.Active=1
 join ClientMaster cl on cl.Id=A.ClientMasterId and cl.Active=1 and cl.Id=@ClientId
 left join InfringmentType it on i.InfringmentTypeId  =it.Id and it.Active=1
 left join Countries cn on i.CountryId=cn.Id and cn.Active
 left join Language lng on i.LanguageId=lng.Id and lng.Active=1
 left join QualityOfPrint qp on i.QualityOfPrintId=qp.Id and qp.Active=1
 where i.UploadDate >= @TWStartDate and i.UploadDate<= @TWEndDate and  i.IsInvalidURL = 0
	union all
Select 
 'TELEGRAM' Platform,
i.SourceURL VideoURL,
A.AssetName AssetName,
it.Name InfringementType, 
i.PostUploadDate UploadDate,
convert_tz(i.URLUploadDate,'+00:00','+05:30') URLUploadDate,
i.Views ViewCount,
null LikeCount,
'0' CommentCount,
'0' RetweetCount,
'0' dislikeCount,
i.Subscribers SubscriberCount,
'0' FavouriteCount,
 i.RemovalStatus,
 null IsChannelSuspended,     
 null VideoId,
 null VideoTitle,
 null Username,
 null UserFullName,
 null ProfileURL,
i.Duration VideoDuration,
qp.Name QualityOfPrint,
i.ChannelName,
null ChannelId,
i.ChannelCreationDate,
i.ChannelURL,
pus.SignPostURL,
lng1.Name Language,
lng2.Name Language1,
lng3.Name Language2,
lng4.Name Language3,
null Language4,
null Keywords,
null Country,
i.Season,
i.Episode 
from TelegramURLsNEW i
inner join Asset A on A.id = i.AssetId and A.Active=1 and i.Active=1 
join ClientMaster cl on cl.Id=A.ClientMasterId and cl.Active=1 and cl.Id=@ClientId
left join InfringmentType it on i.InfringementTypeId  =it.Id and it.Active=1
left join Language lng1 on i.Language1Id=lng1.Id and lng1.Active=1
left join Language lng2 on i.Language2Id=lng2.Id and lng2.Active=1
left join Language lng3 on i.Language3Id=lng3.Id and lng3.Active=1
left join Language lng4 on i.Language4Id=lng4.Id and lng4.Active=1
left join TelegramCategory tc on tc.Id=i.TelegramCategoryId and tc.Active=1
left join QualityOfPrint qp on i.QualityOfPrintId=qp.Id and qp.Active=1
Left Join PlatformUrlSignPostURLs pus on pus.UrlId=i.Id and pus.PlatformId='301B6496-B288-11ED-A6F5-00155D03A4B9' and pus.Active =1
where i.PostUploadDate >= @TWStartDate and i.PostUploadDate<= @TWEndDate and  i.IsInvalidURL = 0
union all 
    Select 
     'UGC' Platform,
    i.VideoURL VideoURL,
    A.AssetName AssetName,
    it.Name InfringementType,
    i.PostUploadDate UploadDate,
    convert_tz(i.DiscoveryDoneAt,'+00:00','+05:30') URLUploadDate,
    i.Views ViewCount,
    i.Likes LikeCount,
    i.CommentCount CommentCount,
	'0' RetweetCount,
	'0' dislikeCount,
    i.Subscriber SubscriberCount,
     null FavouriteCount,
	 null RemovalStatus,
	 null IsChannelSuspended,     
	 null VideoId,
	 i.VideoTitle VideoTitle,
    i.ChannelOrProfileName Username,
     null UserFullName,
    i.ChannelOrProfileURL ProfileURL,
    i.Duration VideoDuration,
    qp.Name QualityOfPrint,
	null ChannelName,
	null ChannelId,
    null ChannelCreationDate,
	null ChannelURL,
    pus.SignPostURL,
    lng.Name Language,
    null Language1,
	null Language2,
	null Language3,
	null Language4,
    i.Keyword Keywords,
    cn.Name Country,
    i.Season,
    i.Episode 
    from UGCAndOtherSocialMediaURLs i
    inner join Asset A on A.id = i.AssetId and A.Active=1 and i.Active=1 
    join ClientMaster cl on cl.Id=A.ClientMasterId and cl.Active=1 and cl.Id=@ClientId
    left join InfringmentType it on i.InfringmentTypeId  =it.Id and it.Active=1
    left join Countries cn on i.CountryId=cn.Id and cn.Active
    left join Language lng on i.LanguageId=lng.Id and lng.Active=1
    left join QualityOfPrint qp on i.QualityOfPrintId=qp.Id and qp.Active=1
    Left Join PlatformUrlSignPostURLs pus on pus.UrlId=i.Id and pus.PlatformId='33D60DB2-FAAA-45F2-9761-1B3882F4CB39' and pus.Active =1
    where i.PostUploadDate >= @TWStartDate and i.PostUploadDate<= @TWEndDate and  i.IsInvalidURL = 0
    union all
    Select 
     'OKURLS' Platform,
	i.VideoURL VideoURL,
    A.AssetName AssetName,
    it.Name InfringementType,
    i.PostUploadDate UploadDate,
    convert_tz(i.DiscoveryDoneAt,'+00:00','+05:30') URLUploadDate,
    i.Views ViewCount,
    i.Likes LikeCount,
    i.CommentCount CommentCount,
	'0' RetweetCount,
	'0' dislikeCount,
    i.Subscriber SubscriberCount,
     null FavouriteCount,
	 null RemovalStatus,
	 null IsChannelSuspended,     
	 null VideoId,
	 i.VideoTitle VideoTitle,
    i.ChannelOrProfileName Username,
     null UserFullName,
    i.ChannelOrProfileURL ProfileURL,
    i.Duration VideoDuration,
    qp.Name QualityOfPrint,
	null ChannelName,
	null ChannelId,
    null ChannelCreationDate,
	null ChannelURL,
    pus.SignPostURL,
    lng.Name Language,
    null Language1,
	null Language2,
	null Language3,
	null Language4,
    i.Keyword Keywords,
    cn.Name Country,
    i.Season,
    i.Episode 
    from OkURLs i
    inner join Asset A on A.id = i.AssetId and A.Active=1 and i.Active=1 
    join ClientMaster cl on cl.Id=A.ClientMasterId and cl.Active=1 and cl.Id=@ClientId
    left join InfringmentType it on i.InfringmentTypeId  =it.Id and it.Active=1
    left join Countries cn on i.CountryId=cn.Id and cn.Active
    left join Language lng on i.LanguageId=lng.Id and lng.Active=1
    left join QualityOfPrint qp on i.QualityOfPrintId=qp.Id and qp.Active=1
    Left Join PlatformUrlSignPostURLs pus on pus.UrlId=i.Id and pus.PlatformId='422EF444-8AD9-4DD6-8A41-D9F3670C735E' and pus.Active =1
    where i.PostUploadDate >= @TWStartDate and i.PostUploadDate<= @TWEndDate and  i.IsInvalidURL = 0
    union
    Select 
     'TIKTOK' Platform,
	i.VideoURL VideoURL,
    A.AssetName AssetName,
    it.Name InfringementType,
    i.PostUploadDate UploadDate,
    convert_tz(i.DiscoveryDoneAt,'+00:00','+05:30') URLUploadDate,
    i.Views ViewCount,
    i.Likes LikeCount,
    i.CommentCount CommentCount,
	'0' RetweetCount,
	'0' dislikeCount,
    i.Subscriber SubscriberCount,
     null FavouriteCount,
	 null RemovalStatus,
	 null IsChannelSuspended,     
	 null VideoId,
	 i.VideoTitle VideoTitle,
    i.ChannelOrProfileName Username,
     null UserFullName,
    i.ChannelOrProfileURL ProfileURL,
    i.Duration VideoDuration,
    qp.Name QualityOfPrint,
	null ChannelName,
	null ChannelId,
    null ChannelCreationDate,
	null ChannelURL,
    pus.SignPostURL,
    lng.Name Language,
    null Language1,
	null Language2,
	null Language3,
	null Language4,
    i.Keyword Keywords,
    cn.Name Country,
    i.Season,
    i.Episode 
    from TiktokURLs i
    inner join Asset A on A.id = i.AssetId and A.Active=1 and i.Active=1 
    join ClientMaster cl on cl.Id=A.ClientMasterId and cl.Active=1 and cl.Id=@ClientId
    left join InfringmentType it on i.InfringmentTypeId  =it.Id and it.Active=1
    left join Countries cn on i.CountryId=cn.Id and cn.Active
    left join Language lng on i.LanguageId=lng.Id and lng.Active=1
    left join QualityOfPrint qp on i.QualityOfPrintId=qp.Id and qp.Active=1
    Left Join PlatformUrlSignPostURLs pus on pus.UrlId=i.Id and pus.PlatformId='86FAC8E8-0F1F-4141-BA84-CD8711B72E39' and pus.Active =1
    where i.PostUploadDate >= @TWStartDate and i.PostUploadDate<= @TWEndDate and  i.IsInvalidURL = 0
    union all
    Select   
     'SHARECHAT' Platform,
    i.VideoURL VideoURL,
    A.AssetName AssetName,
    it.Name InfringementType,
    i.PostUploadDate UploadDate,
    convert_tz(i.DiscoveryDoneAt,'+00:00','+05:30') URLUploadDate,
    i.Views ViewCount,
    i.Likes LikeCount,
    i.CommentCount CommentCount,
	'0' RetweetCount,
	'0' dislikeCount,
    i.Subscriber SubscriberCount,
     null FavouriteCount,
	 null RemovalStatus,
	 null IsChannelSuspended,     
	 null VideoId,
	 i.VideoTitle VideoTitle,
    i.ChannelOrProfileName Username,
     null UserFullName,
    i.ChannelOrProfileURL ProfileURL,
    i.Duration VideoDuration,
    qp.Name QualityOfPrint,
	null ChannelName,
	null ChannelId,
    null ChannelCreationDate,
	null ChannelURL,
    pus.SignPostURL,
    lng.Name Language,
    null Language1,
	null Language2,
	null Language3,
	null Language4,
    i.Keyword Keywords,
    cn.Name Country,
    i.Season,
    i.Episode from ShareChatURLs i
    inner join Asset A on A.id = i.AssetId and A.Active=1 and i.Active=1 
    join ClientMaster cl on cl.Id=A.ClientMasterId and cl.Active=1 and cl.Id=@ClientId
    left join InfringmentType it on i.InfringmentTypeId  =it.Id and it.Active=1
    left join Countries cn on i.CountryId=cn.Id and cn.Active
    left join Language lng on i.LanguageId=lng.Id and lng.Active=1
    left join QualityOfPrint qp on i.QualityOfPrintId=qp.Id and qp.Active=1
    Left Join PlatformUrlSignPostURLs pus on pus.UrlId=i.Id and pus.PlatformId='38750701-0627-48E0-B8B9-F48315DCE47F' and pus.Active =1
    where i.PostUploadDate >= @TWStartDate and i.PostUploadDate<= @TWEndDate and  i.IsInvalidURL = 0
    union all
    Select    
     'VKURLS' Platform,
    i.VideoURL VideoURL,
    A.AssetName AssetName,
    it.Name InfringementType,
    i.PostUploadDate UploadDate,
    convert_tz(i.DiscoveryDoneAt,'+00:00','+05:30') URLUploadDate,
    i.Views ViewCount,
    i.Likes LikeCount,
    i.CommentCount CommentCount,
	'0' RetweetCount,
	'0' dislikeCount,
    i.Subscriber SubscriberCount,
     null FavouriteCount,
	 null RemovalStatus,
	 null IsChannelSuspended,     
	 null VideoId,
	 i.VideoTitle VideoTitle,
    i.ChannelOrProfileName Username,
     null UserFullName,
    i.ChannelOrProfileURL ProfileURL,
    i.Duration VideoDuration,
    qp.Name QualityOfPrint,
	null ChannelName,
	null ChannelId,
    null ChannelCreationDate,
	null ChannelURL,
    pus.SignPostURL,
    lng.Name Language,
    null Language1,
	null Language2,
	null Language3,
	null Language4,
    i.Keyword Keywords,
    cn.Name Country,
    i.Season,
    i.Episode 
    from VKURLs i
    inner join Asset A on A.id = i.AssetId and A.Active=1 and i.Active=1 
    join ClientMaster cl on cl.Id=A.ClientMasterId and cl.Active=1 and cl.Id=@ClientId
    left join InfringmentType it on i.InfringmentTypeId  =it.Id and it.Active=1
    left join Countries cn on i.CountryId=cn.Id and cn.Active
    left join Language lng on i.LanguageId=lng.Id and lng.Active=1
    left join QualityOfPrint qp on i.QualityOfPrintId=qp.Id and qp.Active=1
    Left Join PlatformUrlSignPostURLs pus on pus.UrlId=i.Id and pus.PlatformId='2773562C-0B73-400F-8A4F-68C3F1B24BDE' and pus.Active =1
    where i.PostUploadDate >= @TWStartDate and i.PostUploadDate<= @TWEndDate and  i.IsInvalidURL = 0
    union all 
    Select   
     'DAILYMOTIONURLS' Platform,
    i.VideoURL VideoURL,
    A.AssetName AssetName,
    it.Name InfringementType,
    i.PostUploadDate UploadDate,
    convert_tz(i.DiscoveryDoneAt,'+00:00','+05:30') URLUploadDate,
    i.Views ViewCount,
    i.Likes LikeCount,
    i.CommentCount CommentCount,
	'0' RetweetCount,
	'0' dislikeCount,
    i.Subscriber SubscriberCount,
     null FavouriteCount,
	 null RemovalStatus,
	 null IsChannelSuspended,     
	 null VideoId,
	 i.VideoTitle VideoTitle,
    i.ChannelOrProfileName Username,
     null UserFullName,
    i.ChannelOrProfileURL ProfileURL,
    i.Duration VideoDuration,
    qp.Name QualityOfPrint,
	null ChannelName,
	null ChannelId,
    null ChannelCreationDate,
	null ChannelURL,
    pus.SignPostURL,
    lng.Name Language,
    null Language1,
	null Language2,
	null Language3,
	null Language4,
    i.Keyword Keywords,
    cn.Name Country,
    i.Season,
    i.Episode 
    from DailymotionURLs i
    inner join Asset A on A.id = i.AssetId and A.Active=1 and i.Active=1 
    join ClientMaster cl on cl.Id=A.ClientMasterId and cl.Active=1 and cl.Id=@ClientId
    left join InfringmentType it on i.InfringmentTypeId  =it.Id and it.Active=1
    left join Countries cn on i.CountryId=cn.Id and cn.Active
    left join Language lng on i.LanguageId=lng.Id and lng.Active=1
    left join QualityOfPrint qp on i.QualityOfPrintId=qp.Id and qp.Active=1
    Left Join PlatformUrlSignPostURLs pus on pus.UrlId=i.Id and pus.PlatformId='6FD30070-962C-4949-9100-F45A368039A0' and pus.Active =1
    where i.PostUploadDate >= @TWStartDate and i.PostUploadDate<= @TWEndDate and  i.IsInvalidURL = 0
    union all
    Select  
     'CHOMIKUJ' Platform,
    i.VideoURL VideoURL,
    A.AssetName AssetName,
    it.Name InfringementType,
    i.PostUploadDate UploadDate,
    convert_tz(i.DiscoveryDoneAt,'+00:00','+05:30') URLUploadDate,
    i.Views ViewCount,
    i.Likes LikeCount,
    i.CommentCount CommentCount,
	'0' RetweetCount,
	'0' dislikeCount,
    i.Subscriber SubscriberCount,
     null FavouriteCount,
	 null RemovalStatus,
	 null IsChannelSuspended,     
	 null VideoId,
	 i.VideoTitle VideoTitle,
    i.ChannelOrProfileName Username,
     null UserFullName,
    i.ChannelOrProfileURL ProfileURL,
    i.Duration VideoDuration,
    qp.Name QualityOfPrint,
	null ChannelName,
	null ChannelId,
    null ChannelCreationDate,
	null ChannelURL,
    pus.SignPostURL,
    lng.Name Language,
    null Language1,
	null Language2,
	null Language3,
	null Language4,
    i.Keyword Keywords,
    cn.Name Country,
    i.Season,
    i.Episode 
    from ChomikujURLs i
    inner join Asset A on A.id = i.AssetId and A.Active=1 and i.Active=1 
    join ClientMaster cl on cl.Id=A.ClientMasterId and cl.Active=1 and cl.Id=@ClientId
    left join InfringmentType it on i.InfringmentTypeId  =it.Id and it.Active=1
    left join Countries cn on i.CountryId=cn.Id and cn.Active
    left join Language lng on i.LanguageId=lng.Id and lng.Active=1
    left join QualityOfPrint qp on i.QualityOfPrintId=qp.Id and qp.Active=1
    Left Join PlatformUrlSignPostURLs pus on pus.UrlId=i.Id and pus.PlatformId='6D74B5F2-D086-4756-BD2E-F4C8F9993CB3' and pus.Active =1
    where i.PostUploadDate >= @TWStartDate and i.PostUploadDate<= @TWEndDate and  i.IsInvalidURL = 0
    union all
    Select     
     'BiliBiliURLs' Platform,
    i.VideoURL VideoURL,
    A.AssetName AssetName,
    it.Name InfringementType,
    i.PostUploadDate UploadDate,
    convert_tz(i.DiscoveryDoneAt,'+00:00','+05:30') URLUploadDate,
    i.Views ViewCount,
    i.Likes LikeCount,
    i.CommentCount CommentCount,
	'0' RetweetCount,
	'0' dislikeCount,
    i.Subscriber SubscriberCount,
     null FavouriteCount,
	 null RemovalStatus,
	 null IsChannelSuspended,     
	 null VideoId,
	 i.VideoTitle VideoTitle,
    i.ChannelOrProfileName Username,
     null UserFullName,
    i.ChannelOrProfileURL ProfileURL,
    i.Duration VideoDuration,
    qp.Name QualityOfPrint,
	null ChannelName,
	null ChannelId,
    null ChannelCreationDate,
	null ChannelURL,
    pus.SignPostURL,
    lng.Name Language,
    null Language1,
	null Language2,
	null Language3,
	null Language4,
    i.Keyword Keywords,
    cn.Name Country,
    i.Season,
    i.Episode 
    from BiliBiliURLs i
    inner join Asset A on A.id = i.AssetId and A.Active=1 and i.Active=1 
    join ClientMaster cl on cl.Id=A.ClientMasterId and cl.Active=1 and cl.Id=@ClientId
    left join InfringmentType it on i.InfringmentTypeId  =it.Id and it.Active=1
    left join Countries cn on i.CountryId=cn.Id and cn.Active
    left join Language lng on i.LanguageId=lng.Id and lng.Active=1
    left join QualityOfPrint qp on i.QualityOfPrintId=qp.Id and qp.Active=1
    Left Join PlatformUrlSignPostURLs pus on pus.UrlId=i.Id and pus.PlatformId='E7391B65-78D6-40EE-9723-E43BA778BDB8' and pus.Active =1
    where i.PostUploadDate >= @TWStartDate and i.PostUploadDate<= @TWEndDate and  i.IsInvalidURL = 0;
                            
"
                                , new { ClientId, TWStartDate = StartDate.AddDays(-1).ToString("yyyy-MM-dd") + " 18:30:00", TWEndDate = EndDate?.ToString("yyyy-MM-dd") + " 18:30:00", commandTimeout = 3000 });
                }
                else
                {
                    var assetId = await conn.QueryFirstOrDefaultAsync<string>(@"select Id from Asset where lower(AssetName)=lower(@AssetName)", new { AssetName });
                    return await conn.QueryAsync<AllInOne>(@"
                         

Select 
 'TWITTER' Platform,
 i.source_url_link VideoURL,
 A.AssetName AssetName,
 it.Name InfringementType, 
 convert_tz(i.upload_date,'+00:00','+05:30') UploadDate,
 convert_tz(i.URLUploadDate,'+00:00','+05:30') URLUploadDate,
 i.view_count ViewCount,
 i.like_count LikeCount,
 '0' CommentCount,
 i.retweet_count RetweetCount,
 '0' dislikeCount,
 '0' SubscriberCount,
 '0' FavouriteCount,
  i.RemovalStatus,
 null IsChannelSuspended,     
 null VideoId,
 i.Title VideoTitle,
 i.UserName Username,
 i.UserFullName UserFullName,
 i.ProfileURL ProfileURL,
 i.VideoLength VideoDuration,
 qp.Name QualityOfPrint,
 null ChannelName,
 null ChannelId,
 null ChannelCreationDate,
null ChannelURL,
 pus.SignPostURL,
 lng.Name Language,
 null Language1,
 null Language2,
 null Language3,
 null Language4,
 i.Keywords, 
 cn.Name Country,
 i.Season,
 i.Episode 
 from TwitterURLsNEW i
 inner join Asset A on A.id = i.AssetId and A.Active=1 and i.Active=1 and i.AssetId=@assetId
 join ClientMaster cl on cl.Id=A.ClientMasterId and cl.Active=1 and cl.Id=@ClientId
 left join InfringmentType it on i.InfringementTypeId  =it.Id and it.Active=1
 left join Countries cn on i.CountryId=cn.Id and cn.Active
 left join Language lng on i.LanguageId=lng.Id and lng.Active=1
 left join QualityOfPrint qp on i.QualityOfPrintId=qp.Id and qp.Active=1
 Left Join PlatformUrlSignPostURLs pus on pus.UrlId=i.Id and pus.PlatformId='0265483E-B288-11ED-A6F5-00155D03A4B9' and pus.Active =1
 where i.upload_date >= @TWStartDate and i.upload_date<= @TWEndDate and  i.IsInvalidURL = 0
         union all
 Select 
 'FACEBOOK' Platform,
 i.VideoURL VideoURL,
 A.AssetName AssetName,
 it.Name InfringementType, 
 convert_tz(i.publishedDate,'+00:00','+05:30') UploadDate,
 convert_tz(i.URLUploadDate,'+00:00','+05:30') URLUploadDate,
 i.Views ViewCount, 
 i.like_count LikeCount,
 i.comment_count CommentCount,
 '0' RetweetCount,
 '0' dislikeCount,
 '0' SubscriberCount,
 '0' FavouriteCount,
  i.RemovalStatus,
 null IsChannelSuspended,     
 null VideoId,
 i.VideoTitle VideoTitle,
 i.ProfileName Username,
 null UserFullName,
 i.ProfileURL ProfileURL,
 i.VideoLength VideoDuration,
 qp.Name QualityOfPrint,
 null ChannelName,
 null ChannelId,
 null ChannelCreationDate,
null ChannelURL,
 pus.SignPostURL,
 lng.Name Language,
 null Language1,
null Language2,
null Language3,
null Language4,
 i.Keywords, 
 cn.Name Country,
 i.Season,
 i.Episode 
 from FBUrlsNEW i
 inner join Asset A on A.id = i.AssetId and A.Active=1 and i.Active=1  and i.AssetId=@assetId
 join ClientMaster cl on cl.Id=A.ClientMasterId and cl.Active=1 and cl.Id=@ClientId
 left join InfringmentType it on i.InfringementTypeId  =it.Id and it.Active=1
 left join Countries cn on i.CountryId=cn.Id and cn.Active
 left join Language lng on i.AudioLanguageId=lng.Id and lng.Active=1
 left join QualityOfPrint qp on i.QualityOfPrintId=qp.Id and qp.Active=1
 Left Join PlatformUrlSignPostURLs pus on pus.UrlId=i.Id and pus.PlatformId='F6A79626-B287-11ED-A6F5-00155D03A4B9' and pus.Active =1
 where i.publishedDate >= @TWStartDate and i.publishedDate<= @TWEndDate and  i.IsInvalidURL = 0
      union all
 Select
 'INSTAGRAM' Platform,
 i.VideoURL,
 A.AssetName AssetName,
 it.Name InfringementType, 
 convert_tz(i.PostDate,'+00:00','+05:30') UploadDate,
 convert_tz(i.URLUploadDate,'+00:00','+05:30') URLUploadDate,
 i.ViewCount, 
 i.LikeCount,
 i.CommentsCount,
 '0' RetweetCount,
 '0' dislikeCount,
 '0' SubscriberCount,
 '0' FavouriteCount,
  i.RemovalStatus,
 null IsChannelSuspended,     
 null VideoId,
 null VideoTitle,
 i.UserName Username,
 i.UserFullName,
 i.ProfileURL,
 i.VideoDuration,
 qp.Name QualityOfPrint,
 null ChannelName,
 null ChannelId,
 null ChannelCreationDate,
null ChannelURL,
 pus.SignPostURL,
 lng.Name Language,
 null Language1,
null Language2,
null Language3,
null Language4,
 i.Keywords,
 cn.Name Country,
 i.Season,
 i.Episode 
 from InstagramURLs i
 inner join Asset A on A.id = i.AssetId and A.Active=1 and i.Active=1 and i.AssetId=@assetId
 join ClientMaster cl on cl.Id=A.ClientMasterId and cl.Active=1 and cl.Id=@ClientId
 left join InfringmentType it on i.InfringmentTypeId  =it.Id and it.Active=1
 left join Countries cn on i.CountryId=cn.Id and cn.Active
 left join Language lng on i.AudioLanguageId=lng.Id and lng.Active=1
 left join QualityOfPrint qp on i.QualityOfPrintId=qp.Id and qp.Active=1
 Left Join PlatformUrlSignPostURLs pus on pus.UrlId=i.Id and pus.PlatformId='1547A1E7-B288-11ED-A6F5-00155D03A4B9' and pus.Active =1
 where i.PostDate >= @TWStartDate and i.PostDate<= @TWEndDate and  i.IsInvalidURL = 0
      union all
 Select 
 'YOUTUBE' Platform,
 i.SourceURL VideoURL,
 A.AssetName AssetName,
 it.Name InfringementType,
 convert_tz(i.UploadDate,'+00:00','+05:30') UploadDate, 
 convert_tz(i.URLUploadDate,'+00:00','+05:30') URLUploadDate,
 i.ViewCount,
 i.LikeCount,
 i.CommentCount,
 '0' RetweetCount,
 i.dislikeCount,
 i.SubscriberCount,
 i.FavouriteCount,
 i.RemovalStatus,
 i.IsChannelSuspended,     
 i.VideoId,
 i.VideoName VideoTitle,
 null Username,
 null UserFullName,
 null ProfileURL,
 i.VideoDuration,
 qp.Name QualityOfPrint,
 i.ChannelName,
 i.ChannelId,
null ChannelCreationDate,
null ChannelURL,
 null SignPostURL,
 lng.Name Language,
null Language1,
null Language2,
null Language3,
null Language4,
 i.Keywords,
 cn.Name Country,
 i.Season,
 i.Episode
 from YoutubeURLs i
 inner join Asset A on A.id = i.AssetId and A.Active=1 and i.Active=1 and i.AssetId=@assetId
 join ClientMaster cl on cl.Id=A.ClientMasterId and cl.Active=1 and cl.Id=@ClientId
 left join InfringmentType it on i.InfringmentTypeId  =it.Id and it.Active=1
 left join Countries cn on i.CountryId=cn.Id and cn.Active
 left join Language lng on i.LanguageId=lng.Id and lng.Active=1
 left join QualityOfPrint qp on i.QualityOfPrintId=qp.Id and qp.Active=1
 where i.UploadDate >= @TWStartDate and i.UploadDate<= @TWEndDate and  i.IsInvalidURL = 0
	union all
Select 
 'TELEGRAM' Platform,
i.SourceURL VideoURL,
A.AssetName AssetName,
it.Name InfringementType, 
i.PostUploadDate UploadDate,
convert_tz(i.URLUploadDate,'+00:00','+05:30') URLUploadDate,
i.Views ViewCount,
null LikeCount,
'0' CommentCount,
'0' RetweetCount,
'0' dislikeCount,
i.Subscribers SubscriberCount,
'0' FavouriteCount,
 i.RemovalStatus,
 null IsChannelSuspended,     
 null VideoId,
 null VideoTitle,
 null Username,
 null UserFullName,
 null ProfileURL,
i.Duration VideoDuration,
qp.Name QualityOfPrint,
i.ChannelName,
null ChannelId,
i.ChannelCreationDate,
i.ChannelURL,
pus.SignPostURL,
lng1.Name Language,
lng2.Name Language1,
lng3.Name Language2,
lng4.Name Language3,
null Language4,
null Keywords,
null Country,
i.Season,
i.Episode 
from TelegramURLsNEW i
inner join Asset A on A.id = i.AssetId and A.Active=1 and i.Active=1  and i.AssetId=@assetId
join ClientMaster cl on cl.Id=A.ClientMasterId and cl.Active=1 and cl.Id=@ClientId
left join InfringmentType it on i.InfringementTypeId  =it.Id and it.Active=1
left join Language lng1 on i.Language1Id=lng1.Id and lng1.Active=1
left join Language lng2 on i.Language2Id=lng2.Id and lng2.Active=1
left join Language lng3 on i.Language3Id=lng3.Id and lng3.Active=1
left join Language lng4 on i.Language4Id=lng4.Id and lng4.Active=1
left join TelegramCategory tc on tc.Id=i.TelegramCategoryId and tc.Active=1
left join QualityOfPrint qp on i.QualityOfPrintId=qp.Id and qp.Active=1
Left Join PlatformUrlSignPostURLs pus on pus.UrlId=i.Id and pus.PlatformId='301B6496-B288-11ED-A6F5-00155D03A4B9' and pus.Active =1
where i.PostUploadDate >= @TWStartDate and i.PostUploadDate<= @TWEndDate and  i.IsInvalidURL = 0
union all 
    Select 
     'UGC' Platform,
    i.VideoURL VideoURL,
    A.AssetName AssetName,
    it.Name InfringementType,
    i.PostUploadDate UploadDate,
    convert_tz(i.DiscoveryDoneAt,'+00:00','+05:30') URLUploadDate,
    i.Views ViewCount,
    i.Likes LikeCount,
    i.CommentCount CommentCount,
	'0' RetweetCount,
	'0' dislikeCount,
    i.Subscriber SubscriberCount,
     null FavouriteCount,
	 null RemovalStatus,
	 null IsChannelSuspended,     
	 null VideoId,
	 i.VideoTitle VideoTitle,
    i.ChannelOrProfileName Username,
     null UserFullName,
    i.ChannelOrProfileURL ProfileURL,
    i.Duration VideoDuration,
    qp.Name QualityOfPrint,
	null ChannelName,
	null ChannelId,
    null ChannelCreationDate,
	null ChannelURL,
    pus.SignPostURL,
    lng.Name Language,
    null Language1,
	null Language2,
	null Language3,
	null Language4,
    i.Keyword Keywords,
    cn.Name Country,
    i.Season,
    i.Episode 
    from UGCAndOtherSocialMediaURLs i
    inner join Asset A on A.id = i.AssetId and A.Active=1 and i.Active=1  and i.AssetId=@assetId
    join ClientMaster cl on cl.Id=A.ClientMasterId and cl.Active=1 and cl.Id=@ClientId
    left join InfringmentType it on i.InfringmentTypeId  =it.Id and it.Active=1
    left join Countries cn on i.CountryId=cn.Id and cn.Active
    left join Language lng on i.LanguageId=lng.Id and lng.Active=1
    left join QualityOfPrint qp on i.QualityOfPrintId=qp.Id and qp.Active=1
    Left Join PlatformUrlSignPostURLs pus on pus.UrlId=i.Id and pus.PlatformId='33D60DB2-FAAA-45F2-9761-1B3882F4CB39' and pus.Active =1
    where i.PostUploadDate >= @TWStartDate and i.PostUploadDate<= @TWEndDate and  i.IsInvalidURL = 0
    union all
    Select 
     'OKURLS' Platform,
	i.VideoURL VideoURL,
    A.AssetName AssetName,
    it.Name InfringementType,
    i.PostUploadDate UploadDate,
    convert_tz(i.DiscoveryDoneAt,'+00:00','+05:30') URLUploadDate,
    i.Views ViewCount,
    i.Likes LikeCount,
    i.CommentCount CommentCount,
	'0' RetweetCount,
	'0' dislikeCount,
    i.Subscriber SubscriberCount,
     null FavouriteCount,
	 null RemovalStatus,
	 null IsChannelSuspended,     
	 null VideoId,
	 i.VideoTitle VideoTitle,
    i.ChannelOrProfileName Username,
     null UserFullName,
    i.ChannelOrProfileURL ProfileURL,
    i.Duration VideoDuration,
    qp.Name QualityOfPrint,
	null ChannelName,
	null ChannelId,
    null ChannelCreationDate,
	null ChannelURL,
    pus.SignPostURL,
    lng.Name Language,
    null Language1,
	null Language2,
	null Language3,
	null Language4,
    i.Keyword Keywords,
    cn.Name Country,
    i.Season,
    i.Episode 
    from OkURLs i
    inner join Asset A on A.id = i.AssetId and A.Active=1 and i.Active=1  and i.AssetId=@assetId
    join ClientMaster cl on cl.Id=A.ClientMasterId and cl.Active=1 and cl.Id=@ClientId
    left join InfringmentType it on i.InfringmentTypeId  =it.Id and it.Active=1
    left join Countries cn on i.CountryId=cn.Id and cn.Active
    left join Language lng on i.LanguageId=lng.Id and lng.Active=1
    left join QualityOfPrint qp on i.QualityOfPrintId=qp.Id and qp.Active=1
    Left Join PlatformUrlSignPostURLs pus on pus.UrlId=i.Id and pus.PlatformId='422EF444-8AD9-4DD6-8A41-D9F3670C735E' and pus.Active =1
    where i.PostUploadDate >= @TWStartDate and i.PostUploadDate<= @TWEndDate and  i.IsInvalidURL = 0
    union
    Select 
     'TIKTOK' Platform,
	i.VideoURL VideoURL,
    A.AssetName AssetName,
    it.Name InfringementType,
    i.PostUploadDate UploadDate,
    convert_tz(i.DiscoveryDoneAt,'+00:00','+05:30') URLUploadDate,
    i.Views ViewCount,
    i.Likes LikeCount,
    i.CommentCount CommentCount,
	'0' RetweetCount,
	'0' dislikeCount,
    i.Subscriber SubscriberCount,
     null FavouriteCount,
	 null RemovalStatus,
	 null IsChannelSuspended,     
	 null VideoId,
	 i.VideoTitle VideoTitle,
    i.ChannelOrProfileName Username,
     null UserFullName,
    i.ChannelOrProfileURL ProfileURL,
    i.Duration VideoDuration,
    qp.Name QualityOfPrint,
	null ChannelName,
	null ChannelId,
    null ChannelCreationDate,
	null ChannelURL,
    pus.SignPostURL,
    lng.Name Language,
    null Language1,
	null Language2,
	null Language3,
	null Language4,
    i.Keyword Keywords,
    cn.Name Country,
    i.Season,
    i.Episode 
    from TiktokURLs i
    inner join Asset A on A.id = i.AssetId and A.Active=1 and i.Active=1 and i.AssetId=@assetId
    join ClientMaster cl on cl.Id=A.ClientMasterId and cl.Active=1 and cl.Id=@ClientId
    left join InfringmentType it on i.InfringmentTypeId  =it.Id and it.Active=1
    left join Countries cn on i.CountryId=cn.Id and cn.Active
    left join Language lng on i.LanguageId=lng.Id and lng.Active=1
    left join QualityOfPrint qp on i.QualityOfPrintId=qp.Id and qp.Active=1
    Left Join PlatformUrlSignPostURLs pus on pus.UrlId=i.Id and pus.PlatformId='86FAC8E8-0F1F-4141-BA84-CD8711B72E39' and pus.Active =1
    where i.PostUploadDate >= @TWStartDate and i.PostUploadDate<= @TWEndDate and  i.IsInvalidURL = 0
    union all
    Select   
     'SHARECHAT' Platform,
    i.VideoURL VideoURL,
    A.AssetName AssetName,
    it.Name InfringementType,
    i.PostUploadDate UploadDate,
    convert_tz(i.DiscoveryDoneAt,'+00:00','+05:30') URLUploadDate,
    i.Views ViewCount,
    i.Likes LikeCount,
    i.CommentCount CommentCount,
	'0' RetweetCount,
	'0' dislikeCount,
    i.Subscriber SubscriberCount,
     null FavouriteCount,
	 null RemovalStatus,
	 null IsChannelSuspended,     
	 null VideoId,
	 i.VideoTitle VideoTitle,
    i.ChannelOrProfileName Username,
     null UserFullName,
    i.ChannelOrProfileURL ProfileURL,
    i.Duration VideoDuration,
    qp.Name QualityOfPrint,
	null ChannelName,
	null ChannelId,
    null ChannelCreationDate,
	null ChannelURL,
    pus.SignPostURL,
    lng.Name Language,
    null Language1,
	null Language2,
	null Language3,
	null Language4,
    i.Keyword Keywords,
    cn.Name Country,
    i.Season,
    i.Episode from ShareChatURLs i
    inner join Asset A on A.id = i.AssetId and A.Active=1 and i.Active=1 and i.AssetId=@assetId
    join ClientMaster cl on cl.Id=A.ClientMasterId and cl.Active=1 and cl.Id=@ClientId
    left join InfringmentType it on i.InfringmentTypeId  =it.Id and it.Active=1
    left join Countries cn on i.CountryId=cn.Id and cn.Active
    left join Language lng on i.LanguageId=lng.Id and lng.Active=1
    left join QualityOfPrint qp on i.QualityOfPrintId=qp.Id and qp.Active=1
    Left Join PlatformUrlSignPostURLs pus on pus.UrlId=i.Id and pus.PlatformId='38750701-0627-48E0-B8B9-F48315DCE47F' and pus.Active =1
    where i.PostUploadDate >= @TWStartDate and i.PostUploadDate<= @TWEndDate and  i.IsInvalidURL = 0
    union all
    Select    
     'VKURLS' Platform,
    i.VideoURL VideoURL,
    A.AssetName AssetName,
    it.Name InfringementType,
    i.PostUploadDate UploadDate,
    convert_tz(i.DiscoveryDoneAt,'+00:00','+05:30') URLUploadDate,
    i.Views ViewCount,
    i.Likes LikeCount,
    i.CommentCount CommentCount,
	'0' RetweetCount,
	'0' dislikeCount,
    i.Subscriber SubscriberCount,
     null FavouriteCount,
	 null RemovalStatus,
	 null IsChannelSuspended,     
	 null VideoId,
	 i.VideoTitle VideoTitle,
    i.ChannelOrProfileName Username,
     null UserFullName,
    i.ChannelOrProfileURL ProfileURL,
    i.Duration VideoDuration,
    qp.Name QualityOfPrint,
	null ChannelName,
	null ChannelId,
    null ChannelCreationDate,
	null ChannelURL,
    pus.SignPostURL,
    lng.Name Language,
    null Language1,
	null Language2,
	null Language3,
	null Language4,
    i.Keyword Keywords,
    cn.Name Country,
    i.Season,
    i.Episode 
    from VKURLs i
    inner join Asset A on A.id = i.AssetId and A.Active=1 and i.Active=1 and i.AssetId=@assetId
    join ClientMaster cl on cl.Id=A.ClientMasterId and cl.Active=1 and cl.Id=@ClientId
    left join InfringmentType it on i.InfringmentTypeId  =it.Id and it.Active=1
    left join Countries cn on i.CountryId=cn.Id and cn.Active
    left join Language lng on i.LanguageId=lng.Id and lng.Active=1
    left join QualityOfPrint qp on i.QualityOfPrintId=qp.Id and qp.Active=1
    Left Join PlatformUrlSignPostURLs pus on pus.UrlId=i.Id and pus.PlatformId='2773562C-0B73-400F-8A4F-68C3F1B24BDE' and pus.Active =1
    where i.PostUploadDate >= @TWStartDate and i.PostUploadDate<= @TWEndDate and  i.IsInvalidURL = 0
    union all 
    Select   
     'DAILYMOTIONURLS' Platform,
    i.VideoURL VideoURL,
    A.AssetName AssetName,
    it.Name InfringementType,
    i.PostUploadDate UploadDate,
    convert_tz(i.DiscoveryDoneAt,'+00:00','+05:30') URLUploadDate,
    i.Views ViewCount,
    i.Likes LikeCount,
    i.CommentCount CommentCount,
	'0' RetweetCount,
	'0' dislikeCount,
    i.Subscriber SubscriberCount,
     null FavouriteCount,
	 null RemovalStatus,
	 null IsChannelSuspended,     
	 null VideoId,
	 i.VideoTitle VideoTitle,
    i.ChannelOrProfileName Username,
     null UserFullName,
    i.ChannelOrProfileURL ProfileURL,
    i.Duration VideoDuration,
    qp.Name QualityOfPrint,
	null ChannelName,
	null ChannelId,
    null ChannelCreationDate,
	null ChannelURL,
    pus.SignPostURL,
    lng.Name Language,
    null Language1,
	null Language2,
	null Language3,
	null Language4,
    i.Keyword Keywords,
    cn.Name Country,
    i.Season,
    i.Episode 
    from DailymotionURLs i
    inner join Asset A on A.id = i.AssetId and A.Active=1 and i.Active=1 and i.AssetId=@assetId
    join ClientMaster cl on cl.Id=A.ClientMasterId and cl.Active=1 and cl.Id=@ClientId
    left join InfringmentType it on i.InfringmentTypeId  =it.Id and it.Active=1
    left join Countries cn on i.CountryId=cn.Id and cn.Active
    left join Language lng on i.LanguageId=lng.Id and lng.Active=1
    left join QualityOfPrint qp on i.QualityOfPrintId=qp.Id and qp.Active=1
    Left Join PlatformUrlSignPostURLs pus on pus.UrlId=i.Id and pus.PlatformId='6FD30070-962C-4949-9100-F45A368039A0' and pus.Active =1
    where i.PostUploadDate >= @TWStartDate and i.PostUploadDate<= @TWEndDate and  i.IsInvalidURL = 0
    union all
    Select  
     'CHOMIKUJ' Platform,
    i.VideoURL VideoURL,
    A.AssetName AssetName,
    it.Name InfringementType,
    i.PostUploadDate UploadDate,
    convert_tz(i.DiscoveryDoneAt,'+00:00','+05:30') URLUploadDate,
    i.Views ViewCount,
    i.Likes LikeCount,
    i.CommentCount CommentCount,
	'0' RetweetCount,
	'0' dislikeCount,
    i.Subscriber SubscriberCount,
     null FavouriteCount,
	 null RemovalStatus,
	 null IsChannelSuspended,     
	 null VideoId,
	 i.VideoTitle VideoTitle,
    i.ChannelOrProfileName Username,
     null UserFullName,
    i.ChannelOrProfileURL ProfileURL,
    i.Duration VideoDuration,
    qp.Name QualityOfPrint,
	null ChannelName,
	null ChannelId,
    null ChannelCreationDate,
	null ChannelURL,
    pus.SignPostURL,
    lng.Name Language,
    null Language1,
	null Language2,
	null Language3,
	null Language4,
    i.Keyword Keywords,
    cn.Name Country,
    i.Season,
    i.Episode 
    from ChomikujURLs i
    inner join Asset A on A.id = i.AssetId and A.Active=1 and i.Active=1 and i.AssetId=@assetId
    join ClientMaster cl on cl.Id=A.ClientMasterId and cl.Active=1 and cl.Id=@ClientId
    left join InfringmentType it on i.InfringmentTypeId  =it.Id and it.Active=1
    left join Countries cn on i.CountryId=cn.Id and cn.Active
    left join Language lng on i.LanguageId=lng.Id and lng.Active=1
    left join QualityOfPrint qp on i.QualityOfPrintId=qp.Id and qp.Active=1
    Left Join PlatformUrlSignPostURLs pus on pus.UrlId=i.Id and pus.PlatformId='6D74B5F2-D086-4756-BD2E-F4C8F9993CB3' and pus.Active =1
    where i.PostUploadDate >= @TWStartDate and i.PostUploadDate<= @TWEndDate and  i.IsInvalidURL = 0
    union all
    Select     
     'BiliBiliURLs' Platform,
    i.VideoURL VideoURL,
    A.AssetName AssetName,
    it.Name InfringementType,
    i.PostUploadDate UploadDate,
    convert_tz(i.DiscoveryDoneAt,'+00:00','+05:30') URLUploadDate,
    i.Views ViewCount,
    i.Likes LikeCount,
    i.CommentCount CommentCount,
	'0' RetweetCount,
	'0' dislikeCount,
    i.Subscriber SubscriberCount,
     null FavouriteCount,
	 null RemovalStatus,
	 null IsChannelSuspended,     
	 null VideoId,
	 i.VideoTitle VideoTitle,
    i.ChannelOrProfileName Username,
     null UserFullName,
    i.ChannelOrProfileURL ProfileURL,
    i.Duration VideoDuration,
    qp.Name QualityOfPrint,
	null ChannelName,
	null ChannelId,
    null ChannelCreationDate,
	null ChannelURL,
    pus.SignPostURL,
    lng.Name Language,
    null Language1,
	null Language2,
	null Language3,
	null Language4,
    i.Keyword Keywords,
    cn.Name Country,
    i.Season,
    i.Episode 
    from BiliBiliURLs i
    inner join Asset A on A.id = i.AssetId and A.Active=1 and i.Active=1  and i.AssetId=@assetId
    join ClientMaster cl on cl.Id=A.ClientMasterId and cl.Active=1 and cl.Id=@ClientId
    left join InfringmentType it on i.InfringmentTypeId  =it.Id and it.Active=1 
    left join Countries cn on i.CountryId=cn.Id and cn.Active
    left join Language lng on i.LanguageId=lng.Id and lng.Active=1
    left join QualityOfPrint qp on i.QualityOfPrintId=qp.Id and qp.Active=1
    Left Join PlatformUrlSignPostURLs pus on pus.UrlId=i.Id and pus.PlatformId='E7391B65-78D6-40EE-9723-E43BA778BDB8' and pus.Active =1
    where i.PostUploadDate >= @TWStartDate and i.PostUploadDate<= @TWEndDate and  i.IsInvalidURL = 0;"
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
