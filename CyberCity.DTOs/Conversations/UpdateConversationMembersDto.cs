using System;
using System.ComponentModel.DataAnnotations;

namespace CyberCity.DTOs.Conversations
{
    public class UpdateConversationMembersDto
    {
        [Required]
        public Guid[] MemberIdsToAdd { get; set; } = new Guid[0];

        [Required]
        public Guid[] MemberIdsToRemove { get; set; } = new Guid[0];
    }
}