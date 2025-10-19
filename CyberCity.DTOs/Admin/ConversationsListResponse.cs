namespace CyberCity.DTOs.Admin
{
    public class ConversationsListResponse
    {
        public List<AdminConversationDto> Items { get; set; } = new List<AdminConversationDto>();
        public int TotalItems { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }
}
