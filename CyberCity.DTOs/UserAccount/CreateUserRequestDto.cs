using System;
using System.ComponentModel.DataAnnotations;

namespace CyberCity.DTOs.UserAccount
{
    public class CreateUserRequestDto
    {
        [Required]
        [StringLength(64)]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; }

        [Required]
        [StringLength(128)]
        public string FullName { get; set; }
    }
}


