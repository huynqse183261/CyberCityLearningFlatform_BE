namespace CyberCity.DTOs.Teacher
{
    public class EnrolledCourseDto
    {
        public string CourseUid { get; set; }
        public string CourseTitle { get; set; } = string.Empty;
        public DateTime? EnrolledAt { get; set; }
    }
}
