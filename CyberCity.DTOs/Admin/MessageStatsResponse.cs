namespace CyberCity.DTOs.Admin
{
    public class MessageStatsResponse
    {
        public int TotalConversations { get; set; }
        public int TotalMessages { get; set; }
        public int TodayMessages { get; set; }
        public int ThisWeekMessages { get; set; }
    }
}
