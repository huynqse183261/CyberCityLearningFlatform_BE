using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberCity.DTOs.UserAccount
{
    public class LoginResponseDto
    {
        public Guid Uid { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Role { get; set; }
        public string status { get; set; }
        public string Token { get; set; }
    }
}
