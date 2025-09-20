using CyberCity.DTOs.Messages;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CyberCity.DTOs.Messages;

namespace CyberCity.DTOs.Conversations
{
    public class ConversationDto
    {
        public Guid Uid { get; set; }
        public Guid? OrgUid { get; set; }
        public string Title { get; set; }
        public bool? IsGroup { get; set; }
        public DateTime? CreatedAt { get; set; }
        public List<ConversationMemberDto> Members { get; set; } = new List<ConversationMemberDto>();
        public int MessageCount { get; set; }
        public MessageDto LastMessage { get; set; }
    }
}