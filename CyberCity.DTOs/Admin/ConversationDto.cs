namespace CyberCity.DTOs.Admin
{
    public class AdminConversationDto
    {
        public string Uid { get; set; }
        public string Title { get; set; }
        public bool IsGroup { get; set; }
        public int TotalMessages { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastMessageAt { get; set; }
        public List<SimpleUserDto> Members { get; set; } = new List<SimpleUserDto>();
    }
}
