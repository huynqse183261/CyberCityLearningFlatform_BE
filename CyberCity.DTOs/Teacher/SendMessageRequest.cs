using System.ComponentModel.DataAnnotations;

namespace CyberCity.DTOs.Teacher
{
    public class SendMessageRequest
    {
        [Required]
        [MinLength(1)]
        public string Content { get; set; } = string.Empty;
    }
}
