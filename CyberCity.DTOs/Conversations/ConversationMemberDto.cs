using System;

namespace CyberCity.DTOs.Conversations
{
    public class ConversationMemberDto
    {
        public Guid Uid { get; set; }
        public Guid ConversationUid { get; set; }
        public Guid UserUid { get; set; }
        public DateTime? JoinedAt { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public string Image { get; set; }
    }
}