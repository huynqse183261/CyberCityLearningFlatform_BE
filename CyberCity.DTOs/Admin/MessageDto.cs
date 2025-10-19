namespace CyberCity.DTOs.Admin
{
    public class AdminMessageDto
    {
        public Guid Uid { get; set; }
        public Guid ConversationUid { get; set; }
        public Guid SenderUid { get; set; }
        public string Message { get; set; }
        public DateTime SentAt { get; set; }
        public SimpleUserDto Sender { get; set; }
    }
}
