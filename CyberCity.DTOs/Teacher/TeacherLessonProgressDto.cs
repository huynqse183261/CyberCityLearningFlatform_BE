namespace CyberCity.DTOs.Teacher
{
    public class TeacherLessonProgressDto
    {
        public string LessonUid { get; set; }
        public string LessonTitle { get; set; } = string.Empty;
        public double Progress { get; set; }
        public int TotalSubtopics { get; set; }
        public int CompletedSubtopics { get; set; }
    }
}
