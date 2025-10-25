using System.ComponentModel.DataAnnotations;

namespace CyberCity.DTOs.Teacher
{
    public class CreateConversationRequest
    {
        [Required]
        public string StudentUid { get; set; }
    }
}
