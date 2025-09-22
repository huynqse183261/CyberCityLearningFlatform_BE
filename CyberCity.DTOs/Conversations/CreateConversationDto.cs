using System;
using System.ComponentModel.DataAnnotations;

namespace CyberCity.DTOs.Conversations
{
    public class CreateConversationDto
    {
        [Required]
        [StringLength(255)]
        public string Title { get; set; }

        public bool IsGroup { get; set; } = false;

        public Guid? OrgUid { get; set; }

        [Required]
        public Guid[] InitialMemberIds { get; set; }
    }
}