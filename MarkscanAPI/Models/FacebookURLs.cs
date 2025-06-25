using Dapper;
using Dapper.Contrib.Extensions;
using DbAccess;
using System.ComponentModel.DataAnnotations.Schema;

namespace MarkscanAPI.Models
{
    [System.ComponentModel.DataAnnotations.Schema.Table("FBUrlsNEW")]
    public class FacebookURLs
    {
        [Column("VideoURL")]
        public string? VideoURL { get; set; }
        [Computed]
        [Write(false)]
        public string? AssetName { get; set; }
        [Column("ProfileURL")]
        public string? ProfileURL { get; set; }
        [Computed]
        [Write(false)]
        public string? InfringementType { get; set; }
        [Column("VideoTitle")]
        public string? VideoTitle { get; set; }
        [Column("ProfileName")]
        public string? ProfileName { get; set; }
        [Computed]
        [Write(false)]
        public string? QualityOfPrint { get; set; }
        [Column("Views")]
        public string? Views { get; set; }
        [Column("like_count")]
        public string? like_count { get; set; }
        [Column("comment_count")]
        public string? comment_count { get; set; }
        [Column("VideoLength")]
        public string? VideoLength { get; set; }
        [Computed]
        [Write(false)]
        public string? SignPostURLs { get; set; }
        [Column("publishedDate")]
        public DateTime? publishedDate { get; set; }
        [Column("URLUploadDate")]
        public DateTime? URLUploadDate { get; set; }
        [Column("Keywords")]
        public string? Keywords { get; set; }
        [Computed]
        [Write(false)]
        public string? AudioLanguage { get; set; }
        [Column("Season")]
        public string? Season { get; set; }
        [Column("Episode")]
        public string? Episode { get; set; }
        [Computed]
        [Write(false)]
        public string? Country { get; set; }

        public static async Task<IEnumerable<FacebookURLs>> GetURLsForClient(IDatabaseConnection databaseConnection, string? ClientId, DateTime StartDate, DateTime? EndDate, string? AssetName)
        {
            try
            {
                using var conn = databaseConnection.GetConnection();
                if (string.IsNullOrEmpty(AssetName))
                {
                    return await conn.QueryAsync<FacebookURLs>(@"Select i.VideoURL,A.AssetName AssetName,it.Name InfringementType, convert_tz(i.publishedDate,'+00:00','+05:30') publishedDate, convert_tz(i.URLUploadDate,'+00:00','+05:30') URLUploadDate, i.Views, i.like_count,i.comment_count,
                            i.ProfileName,i.ProfileURL,i.VideoTitle,i.VideoLength,qp.Name QualityOfPrint,pus.SignPostURL,lng.Name AudioLanguage,i.Keywords, cn.Name Country,i.Season,i.Episode from FBUrlsNEW i
                            inner join Asset A on A.id = i.AssetId and A.Active=1 and i.Active=1
                            join ClientMaster cl on cl.Id=A.ClientMasterId and cl.Active=1 and cl.Id=@ClientId
                            left join InfringmentType it on i.InfringementTypeId  =it.Id and it.Active=1
                            left join Countries cn on i.CountryId=cn.Id and cn.Active
                            left join Language lng on i.AudioLanguageId=lng.Id and lng.Active=1
                            left join QualityOfPrint qp on i.QualityOfPrintId=qp.Id and qp.Active=1
                            Left Join PlatformUrlSignPostURLs pus on pus.UrlId=i.Id and pus.PlatformId='F6A79626-B287-11ED-A6F5-00155D03A4B9' and pus.Active =1
                            where i.URLUploadDate >= @FBStartDate and i.URLUploadDate<= @FBEndDate and  i.IsInvalidURL = 0;"
                                , new { ClientId, FBStartDate = StartDate.AddDays(-1).ToString("yyyy-MM-dd") + " 18:30:00", FBEndDate = EndDate?.ToString("yyyy-MM-dd") + " 18:30:00", commandTimeout = 3000 });
                }
                else
                {
                    var assetId = await conn.QueryFirstOrDefaultAsync<string>(@"select Id from Asset where lower(AssetName)=lower(@AssetName)", new { AssetName });
                    return await conn.QueryAsync<FacebookURLs>(@"Select i.VideoURL,A.AssetName AssetName,it.Name InfringementType, convert_tz(i.publishedDate,'+00:00','+05:30') publishedDate, convert_tz(i.URLUploadDate,'+00:00','+05:30') URLUploadDate, i.Views, i.like_count,i.comment_count,
                            i.ProfileName,i.ProfileURL,i.VideoTitle,i.VideoLength,qp.Name QualityOfPrint,pus.SignPostURL,lng.Name AudioLanguage,i.Keywords, cn.Name Country,i.Season,i.Episode from FBUrlsNEW i
                            inner join Asset A on A.id = i.AssetId and A.Active=1 and i.Active=1 and AssetId=@assetId
                            join ClientMaster cl on cl.Id=A.ClientMasterId and cl.Active=1 and cl.Id=@ClientId
                            left join InfringmentType it on i.InfringementTypeId  =it.Id and it.Active=1
                            left join Countries cn on i.CountryId=cn.Id and cn.Active
                            left join Language lng on i.AudioLanguageId=lng.Id and lng.Active=1
                            left join QualityOfPrint qp on i.QualityOfPrintId=qp.Id and qp.Active=1
                            Left Join PlatformUrlSignPostURLs pus on pus.UrlId=i.Id and pus.PlatformId='F6A79626-B287-11ED-A6F5-00155D03A4B9' and pus.Active =1
                            where i.URLUploadDate >= @FBStartDate and i.URLUploadDate<= @FBEndDate and  i.IsInvalidURL = 0;"
                                , new { ClientId, FBStartDate = StartDate.AddDays(-1).ToString("yyyy-MM-dd") + " 18:30:00", FBEndDate = EndDate?.ToString("yyyy-MM-dd") + " 18:30:00", assetId, commandTimeout = 3000 });
                }
            }
            catch (Exception ex) 
            {
                return null;
            }
        }
    }

}

