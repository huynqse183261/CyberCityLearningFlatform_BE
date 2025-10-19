namespace CyberCity.DTOs.Admin
{
    public class MessagesListResponse
    {
        public List<AdminMessageDto> Items { get; set; } = new List<AdminMessageDto>();
        public int TotalItems { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }
}
