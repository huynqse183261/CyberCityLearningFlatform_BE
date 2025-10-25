namespace CyberCity.DTOs.Teacher
{
    public class TeacherCourseProgressDto
    {
        public string CourseUid { get; set; }
        public string CourseTitle { get; set; } = string.Empty;
        public double OverallProgress { get; set; }
        public int TotalSubtopics { get; set; }
        public int CompletedSubtopics { get; set; }
        public List<TeacherModuleProgressDto> Modules { get; set; } = new List<TeacherModuleProgressDto>();
        public DateTime? LastActivity { get; set; }
    }
}
