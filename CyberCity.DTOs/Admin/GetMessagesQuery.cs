namespace CyberCity.DTOs.Admin
{
    public class GetMessagesQuery
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 50;
    }
}
