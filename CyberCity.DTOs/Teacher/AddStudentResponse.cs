namespace CyberCity.DTOs.Teacher
{
    public class AddStudentResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public AddStudentDataDto? Data { get; set; }
    }

    public class AddStudentDataDto
    {
        public string Uid { get; set; }
        public string TeacherUid { get; set; }
        public string StudentUid { get; set; }
        public string CourseUid { get; set; }
        public DateTime AssignedAt { get; set; }
    }
}
