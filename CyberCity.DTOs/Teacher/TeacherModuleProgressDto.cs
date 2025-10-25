namespace CyberCity.DTOs.Teacher
{
    public class TeacherModuleProgressDto
    {
        public string ModuleUid { get; set; }
        public string ModuleTitle { get; set; } = string.Empty;
        public double Progress { get; set; }
        public int TotalSubtopics { get; set; }
        public int CompletedSubtopics { get; set; }
        public List<TeacherLessonProgressDto> Lessons { get; set; } = new List<TeacherLessonProgressDto>();
    }
}
