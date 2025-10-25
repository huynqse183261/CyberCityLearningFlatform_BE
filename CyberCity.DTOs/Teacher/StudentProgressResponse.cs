namespace CyberCity.DTOs.Teacher
{
    public class StudentProgressResponse
    {
        public bool Success { get; set; }
        public List<TeacherCourseProgressDto> Data { get; set; } = new List<TeacherCourseProgressDto>();
    }
}
