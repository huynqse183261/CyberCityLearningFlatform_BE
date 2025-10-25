using System;

namespace CyberCity.DTOs.Messages
{
    public class MessageDto
    {
        public string Uid { get; set; }
        public string ConversationUid { get; set; }
        public string SenderUid { get; set; }
        public string Message { get; set; }
        public DateTime? SentAt { get; set; }
        public string SenderUsername { get; set; }
        public string SenderFullName { get; set; }
        public string SenderImage { get; set; }
    }
}