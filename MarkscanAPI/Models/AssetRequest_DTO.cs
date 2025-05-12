using System.ComponentModel.DataAnnotations.Schema;

namespace MarkscanAPI.Models
{
    public class AssetRequest_DTO
    {
        public string? CompanyName { get; set; }
        public string? AssetName { get; set; }
        public string? CopyrightOwnerName { get; set; }
        public string? GenreName { get; set; }
        public string? OfficialURL { get; set; }
        public List<string>? OriginLanguageList { get; set; }
        public List<string>? ContentLanguageList { get; set; }
        public List<string>? CountryList { get; set; }
        public List<string>? ExclusiveCountryList { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public DateTime? RightsExpiryDate { get; set; }
        public DateTime? OttBroadcastReleaseDateTime { get; set; }
        public DateTime? OttReleaseDateTime { get; set; }
        public string? ImdbId { get; set; }
        public int? ReleaseYear { get; set; }
        public bool IsAssetExclusive { get; set; }
        public bool IsMonitoringOn { get; set; }
        public bool IsApproved { get; set; }
    }
}
