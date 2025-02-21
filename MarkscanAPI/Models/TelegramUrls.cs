using StackExchange.Redis;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Threading.Channels;
using DbAccess;
using Dapper;

namespace MarkscanAPI.Models
{
    [System.ComponentModel.DataAnnotations.Schema.Table("TelegramURLsNEW")]
    public class TelegramUrls
    {
        public string? AssetName { get; set; }
        public string? SourceURL{ get; set; }
        public string? ChannelURL { get; set; }
        public string? ChannelName { get; set; }
        public string? SignPostURLs { get; set; }
        public string? Subscribers { get; set; }
        public string? Views { get; set; }
        public string? Duration { get; set; }
        public string? Language1 { get; set; }
        public string? Language2 { get; set; }
        public string? Language3 { get; set; }
        public string? Language4 { get; set; }
        public string? Quality { get; set; }
        public string? Season { get; set; }
        public string? Episode { get; set; }
        public string? Category { get; set; }
        public DateTime? ChannelCreationDate { get; set; }
        public DateTime? PostUploadDateTime { get; set; }
        public string? InfringementType { get; set; }

        public static async Task<IEnumerable<TelegramUrls>> GetURLsForClient(IDatabaseConnection databaseConnection, string? ClientId, DateTime StartDate, DateTime? EndDate, string? AssetName)
        {
            try
            {
                using var conn = databaseConnection.GetConnection();
                if (string.IsNullOrEmpty(AssetName))
                {
                    return await conn.QueryAsync<TelegramUrls>(@"Select i.SourceURL,A.AssetName AssetName,it.Name InfringementType, i.PostUploadDate, i.Views,i.Subscribers,
                            i.ChannelName,i.ChannelCreationDate,i.ChannelURL,i.Duration,qp.Name Quality,pus.SignPostURL,lng1.Name Language1,lng2.Name Language2,lng3.Name Language3,lng4.Name Language4,
                            i.Season,i.Episode from TelegramURLsNEW i
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
                            where i.PostUploadDate >= @TLStartDate and i.PostUploadDate<= @TLEndDate and  i.IsInvalidURL = 0;"
                                , new { ClientId, TLStartDate = StartDate.AddDays(-1).ToString("yyyy-MM-dd") + " 18:30:00", TLEndDate = EndDate?.ToString("yyyy-MM-dd") + " 18:30:00", commandTimeout = 3000 });
                }
                else
                {
                    var assetId = await conn.QueryFirstOrDefaultAsync<string>(@"select Id from Asset where lower(AssetName)=lower(@AssetName)", new { AssetName });
                    return await conn.QueryAsync<TelegramUrls>(@"Select i.SourceURL,A.AssetName AssetName,it.Name InfringementType, i.PostUploadDate, i.Views,i.Subscribers,
                            i.ChannelName,i.ChannelCreationDate,i.ChannelURL,i.Duration,qp.Name Quality,pus.SignPostURL,lng1.Name Language1,lng2.Name Language2,lng3.Name Language3,lng4.Name Language4,
                            i.Season,i.Episode from TelegramURLsNEW i
                            inner join Asset A on A.id = i.AssetId and A.Active=1 and i.Active=1 and AssetId=@assetId
                            join ClientMaster cl on cl.Id=A.ClientMasterId and cl.Active=1 and cl.Id=@ClientId
                            left join InfringmentType it on i.InfringementTypeId  =it.Id and it.Active=1
                            left join Language lng1 on i.Language1Id=lng1.Id and lng1.Active=1
                            left join Language lng2 on i.Language2Id=lng2.Id and lng2.Active=1
                            left join Language lng3 on i.Language3Id=lng3.Id and lng3.Active=1
                            left join Language lng4 on i.Language4Id=lng4.Id and lng4.Active=1
                            left join TelegramCategory tc on tc.Id=i.TelegramCategoryId and tc.Active=1
                            left join QualityOfPrint qp on i.QualityOfPrintId=qp.Id and qp.Active=1
                            Left Join PlatformUrlSignPostURLs pus on pus.UrlId=i.Id and pus.PlatformId='301B6496-B288-11ED-A6F5-00155D03A4B9' and pus.Active =1
                            where i.PostUploadDate >= @TLStartDate and i.PostUploadDate<= @TLEndDate and  i.IsInvalidURL = 0;"
                                , new { ClientId, TLStartDate = StartDate.AddDays(-1).ToString("yyyy-MM-dd") + " 18:30:00", TLEndDate = EndDate?.ToString("yyyy-MM-dd") + " 18:30:00", assetId, commandTimeout = 3000 });
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
