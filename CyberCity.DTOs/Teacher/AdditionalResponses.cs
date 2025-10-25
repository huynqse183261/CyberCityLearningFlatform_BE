namespace CyberCity.DTOs.Teacher
{
    public class RemoveStudentResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    public class MarkAsReadResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
