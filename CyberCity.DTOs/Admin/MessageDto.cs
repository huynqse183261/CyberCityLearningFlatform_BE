namespace CyberCity.DTOs.Admin
{
    public class AdminMessageDto
    {
        public string Uid { get; set; }
        public string ConversationUid { get; set; }
        public string SenderUid { get; set; }
        public string Message { get; set; }
        public DateTime SentAt { get; set; }
        public SimpleUserDto Sender { get; set; }
    }
}
