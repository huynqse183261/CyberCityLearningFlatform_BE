using System;

namespace CyberCity.DTOs.Conversations
{
    public class ConversationMemberDto
    {
        public string Uid { get; set; }
        public string ConversationUid { get; set; }
        public string UserUid { get; set; }
        public DateTime? JoinedAt { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public string Image { get; set; }
    }
}