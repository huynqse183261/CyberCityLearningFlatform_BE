using System;

namespace CyberCity.DTOs.Messages
{
    public class MessageDto
    {
        public Guid Uid { get; set; }
        public Guid ConversationUid { get; set; }
        public Guid SenderUid { get; set; }
        public string Message { get; set; }
        public DateTime? SentAt { get; set; }
        public string SenderUsername { get; set; }
        public string SenderFullName { get; set; }
        public string SenderImage { get; set; }
    }
}