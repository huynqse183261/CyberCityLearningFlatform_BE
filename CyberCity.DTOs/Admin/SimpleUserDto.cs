namespace CyberCity.DTOs.Admin
{
    public class SimpleUserDto
    {
        public string Uid { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public string Role { get; set; } // student, teacher, admin
        public string? Image { get; set; }
    }
}
