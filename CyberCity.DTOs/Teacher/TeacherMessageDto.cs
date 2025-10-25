namespace CyberCity.DTOs.Teacher
{
    public class TeacherMessageDto
    {
        public string Uid { get; set; }
        public string SenderUid { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool IsRead { get; set; }
    }
}
