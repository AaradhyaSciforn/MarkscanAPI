using StackExchange.Redis;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Threading.Channels;
using DbAccess;
using Dapper;
using Dapper.Contrib.Extensions;
using System.ComponentModel.DataAnnotations.Schema;


namespace MarkscanAPI.Models
{


    [System.ComponentModel.DataAnnotations.Schema.Table("InternetURLsNEW")]
    public class InternetURLs
    {

        [Column("SourceURL")]
        public string? SourceURL { get; set; }

        [Column("SourceDomain")]
        public string? SourceDomain { get; set; }

        [Column("InfringingURL")]
        public string? InfringingURL { get; set; }

        public string? InfringementType { get; set; }

        [Column("InfringingDomain")]
        public string? InfringingDomain { get; set; }


        public string? SearchEngine { get; set; }

        [Column("PageNo")]
        public string? PageNo { get; set; }

        [Column("URLRank")]
        public string? URLRank { get; set; }

        [Column("KeyWord")]
        public string? KeyWord { get; set; }

        public string? QualityOfPrint { get; set; }

        [Column("Season")]
        public string? Season { get; set; }

        [Column("Episode")]
        public string? Episode { get; set; }

        [Column("URLUploadDate")]
        public DateTime? URLUploadDate { get; set; }

        public string? Country { get; set; }


        [Column("FileName")]
        public string? FileName { get; set; }

        [Column("FileSize")]
        public string? FileSize { get; set; }

        [Column("SourceHTMLTag")]
        public string? SourceHTMLTag { get; set; }

        [Column("InfringingHTMLTag")]
        public string? InfringingHTMLTag { get; set; }


        [Column("DDLIndexURL1")]
        public string? DDLIndexURL1 { get; set; }

        [Column("DDLIndexURL2")]
        public string? DDLIndexURL2 { get; set; }

        [Column("DDLIndexURL3")]
        public string? DDLIndexURL3 { get; set; }

        [Column("Note1")]
        public string? Note1 { get; set; }

        [Column("Note2")]
        public string? Note2 { get; set; }


        public string? Language1 { get; set; }


        public string? Language2 { get; set; }


        public string? Language3 { get; set; }


        public string? Language4 { get; set; }

        [Column("NoOfDownloads")]
        public string? NoOfDownloads { get; set; }

        [Column("MatchedTitle")]
        public string? MatchedTitle { get; set; }


        [Column("IsSingleAudio")]
        public bool IsSingleAudio { get; set; }


        [Column("PageUploadDate")]
        public DateTime? PageUploadDate { get; set; }



      
        public string? TVChannel { get; set; }



        [Computed]
        [Write(false)]
        public string? AssetName { get; set; }

        [Computed]
        [Write(false)]
        public string? removalstatus { get; set; }

        [Computed]
        [Write(false)]
        public DateTime? RemovalTime { get; set; }

        [Computed]
        [Write(false)]
        public string? delistingremovalstatus { get; set; }

        [Computed]
        [Write(false)]
        public DateTime? DelistingTime { get; set; }

        [Computed]
        [Write(false)]
        public string? dmcaremovalstatus { get; set; }

        [Computed]
        [Write(false)]
        public DateTime? DMCARemovalTime { get; set; }









        public static async Task<IEnumerable<InternetURLs>> GetURLsForClient(IDatabaseConnection databaseConnection, string? ClientId, DateTime StartDate, DateTime? EndDate, string? AssetName)
        {
            try
            {
                using var conn = databaseConnection.GetConnection();
                if (string.IsNullOrEmpty(AssetName))
                {
                    return await conn.QueryAsync<InternetURLs>(@"Select i.SourceURL,i.SourceDomain,i.InfringingURL,i.InfringingDomain,A.AssetName AssetName,it.Name InfringementType, convert_tz(i.URLUploadDate,'+00:00','+05:30') URLUploadDate,
                            qp.Name Quality,lng1.Name Language1,lng2.Name Language2,lng3.Name Language3,lng4.Name Language4,
                            i.Season,i.Episode, se.Name SearchEngine,i.KeyWord,i.PageNo,i.URLRank,ct.Name Country,i.SourceHTMLTag,i.DDLIndexURL1,i.DDLIndexURL2,i.DDLIndexURL3,i.Note1,i.Note2,ch.Name TVChannel, qp.Name QualityOfPrint,
                            i.SourceRemovalTime RemovalTime,i.InfringingRemovalTime DelistingTime,i.InfringingDMCARemovalTime DMCARemovalTime,i.SourceRemovalStatus removalstatus,i.InfringingRemovalStatus delistingremovalstatus,i.InfringingDMCARemovalStatus  dmcaremovalstatus
                            from InternetURLsNEW i
                            inner join Asset A on A.id = i.AssetId and A.Active=1 and i.Active=1 
                            join ClientMaster cl on cl.Id=A.ClientMasterId and cl.Active=1 and cl.Id='83C95897-D4DF-4B51-9C95-1EE42DAA34C8'
                            left join InfringmentType it on i.InfringementTypeId  =it.Id and it.Active=1
                            left join Language lng1 on i.Language1Id=lng1.Id and lng1.Active=1
                            left join Language lng2 on i.Language2Id=lng2.Id and lng2.Active=1
                            left join Language lng3 on i.Language3Id=lng3.Id and lng3.Active=1
                            left join Language lng4 on i.Language4Id=lng4.Id and lng4.Active=1
 						    left join Channel ch on ch.Id = i.TVChannelId and ch.Active =1
                            left Join SearchEngine se on se.Id = i.SearchEngineId and se.Active =1
                            left join QualityOfPrint qp on i.QualityOfPrintId=qp.Id and qp.Active=1
                            Left join Countries ct on ct.Id = i.CountryId and ct.Active =1
                            where i.DiscoveryDoneAt >= @TLStartDate and i.DiscoveryDoneAt<= @TLEndDate and  i.IsInvalidURL = 0;"
                                , new { ClientId, TLStartDate = StartDate.AddDays(-1).ToString("yyyy-MM-dd") + " 18:30:00", TLEndDate = EndDate?.ToString("yyyy-MM-dd") + " 18:30:00", commandTimeout = 3000 });
                }
                else
                {
                    var assetId = await conn.QueryFirstOrDefaultAsync<string>(@"select Id from Asset where lower(AssetName)=lower(@AssetName)", new { AssetName });
                    return await conn.QueryAsync<InternetURLs>(@"Select i.SourceURL,i.SourceDomain,i.InfringingURL,i.InfringingDomain,A.AssetName AssetName,it.Name InfringementType, convert_tz(i.URLUploadDate,'+00:00','+05:30') URLUploadDate,
                            qp.Name Quality,lng1.Name Language1,lng2.Name Language2,lng3.Name Language3,lng4.Name Language4,
                            i.Season,i.Episode, se.Name SearchEngine,i.KeyWord,i.PageNo,i.URLRank,ct.Name Country,i.SourceHTMLTag,i.DDLIndexURL1,i.DDLIndexURL2,i.DDLIndexURL3,i.Note1,i.Note2,ch.Name TVChannel,qp.Name QualityOfPrint,
                            i.SourceRemovalTime RemovalTime,i.InfringingRemovalTime DelistingTime,i.InfringingDMCARemovalTime DMCARemovalTime,i.SourceRemovalStatus removalstatus,i.InfringingRemovalStatus delistingremovalstatus,i.InfringingDMCARemovalStatus  dmcaremovalstatus
                            from InternetURLsNEW i
                            inner join Asset A on A.id = i.AssetId and A.Active=1 and i.Active=1 and AssetId=@assetId
                            join ClientMaster cl on cl.Id=A.ClientMasterId and cl.Active=1 and cl.Id=@ClientId
                            left join InfringmentType it on i.InfringementTypeId  =it.Id and it.Active=1
                            left join Language lng1 on i.Language1Id=lng1.Id and lng1.Active=1
                            left join Language lng2 on i.Language2Id=lng2.Id and lng2.Active=1
                            left join Language lng3 on i.Language3Id=lng3.Id and lng3.Active=1
                            left join Language lng4 on i.Language4Id=lng4.Id and lng4.Active=1
						    left join Channel ch on ch.Id = i.TVChannelId and ch.Active =1
                            left Join SearchEngine se on se.Id = i.SearchEngineId and se.Active =1
                            left join QualityOfPrint qp on i.QualityOfPrintId=qp.Id and qp.Active=1
                            Left join Countries ct on ct.Id = i.CountryId and ct.Active =1
                            where i.DiscoveryDoneAt >= @TLStartDate and i.DiscoveryDoneAt<= @TLEndDate and  i.IsInvalidURL = 0;"
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
