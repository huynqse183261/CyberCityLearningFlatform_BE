namespace CyberCity.DTOs.Teacher
{
    public class TeacherStudentListDto
    {
        public string Uid { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public List<EnrolledCourseDto> EnrolledCourses { get; set; } = new List<EnrolledCourseDto>();
        public DateTime? LastActive { get; set; }
    }
}
