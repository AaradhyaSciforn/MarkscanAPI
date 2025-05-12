namespace MarkscanAPI.Models
{
    public class ClientRequest_DTO
    {
        public string? CompanyName { get; set; }
        public string? TypeOfClient { get; set; }
        public List<string>? GenreList { get; set; }
        public List<string>? CopyrightOwnerNameList { get; set; }

    }
}
