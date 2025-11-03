using System;
using System.ComponentModel.DataAnnotations;

namespace CyberCity.DTOs.Conversations
{
    public class UpdateConversationMembersDto
    {
        [Required]
        public string[] MemberIdsToAdd { get; set; } = new string[0];

        [Required]
        public string[] MemberIdsToRemove { get; set; } = new string[0];
    }
}