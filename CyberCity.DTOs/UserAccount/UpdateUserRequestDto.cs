using System;
using System.ComponentModel.DataAnnotations;

namespace CyberCity.DTOs.UserAccount
{
    public class UpdateUserRequestDto
    {
        [EmailAddress]
        public string Email { get; set; }

        [StringLength(128)]
        public string FullName { get; set; }

        public string Role { get; set; }
    }
}


