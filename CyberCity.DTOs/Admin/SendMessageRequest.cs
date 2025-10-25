using System.ComponentModel.DataAnnotations;

namespace CyberCity.DTOs.Admin
{
    public class SendMessageRequest
    {
        [Required(ErrorMessage = "Message is required")]
        [MinLength(1, ErrorMessage = "Message cannot be empty")]
        public string Message { get; set; }
    }
}
