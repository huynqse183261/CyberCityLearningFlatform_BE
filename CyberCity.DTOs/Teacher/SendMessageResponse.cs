namespace CyberCity.DTOs.Teacher
{
    public class SendMessageResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public TeacherMessageDto? Data { get; set; }
    }
}
