using Dapper.Contrib.Extensions;
using System.Text.Json.Serialization;

namespace MarkscanAPI.Common
{
    public class CommonFields : BaseFields
    {
        [JsonIgnore]
        public string? Code1 { get; set; }

        [JsonIgnore]
        public string? Code2 { get; set; }

        [JsonIgnore]
        public string? Code3 { get; set; }

        [JsonIgnore]
        public string? Code4 { get; set; }
    }
    public class BaseFields
    {

        [ExplicitKey]
        [JsonIgnore]
        public string? Id { get; set; }

        [JsonIgnore]
        public DateTime? UpdatedOn { get; set; }

        [Computed]
        [JsonIgnore]
        [Write(false)]
        public DateTime? UpdatedOnLocal
        {
            get
            {
                if (UpdatedOn != null)
                {
                    return UpdatedOn.Value.ToLocalTime();
                }
                return UpdatedOn;
            }
        }

        [JsonIgnore]
        public bool Active { get; set; }

        [Computed]
        [JsonIgnore]
        public string? ChangeType { get; set; }

        [Computed]
        [JsonIgnore]
        public DateTime? ChangedOn { get; set; }

        [Computed]
        [JsonIgnore]
        public string? ChangedBy { get; set; }
    }

}
