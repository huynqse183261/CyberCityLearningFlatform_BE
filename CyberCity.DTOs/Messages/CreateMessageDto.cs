using System;
using System.ComponentModel.DataAnnotations;

namespace CyberCity.DTOs.Messages
{
    public class CreateMessageDto
    {
        [Required]
        [StringLength(2000)]
        public string Message { get; set; }
    }
}