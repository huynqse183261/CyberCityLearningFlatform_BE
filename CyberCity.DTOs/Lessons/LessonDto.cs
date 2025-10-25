namespace CyberCity.DTOs.Lessons
{
    public class LessonDto
    {
        public string Uid { get; set; }
        public string ModuleUid { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string LessonType { get; set; }
        public int OrderIndex { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}

