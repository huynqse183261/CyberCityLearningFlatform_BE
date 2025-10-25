namespace CyberCity.DTOs.Admin
{
    public class GetConversationsQuery
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public string? SearchQuery { get; set; } // Tìm theo title
    }
}
