namespace CyberCity.DTOs.Topics
{
    public class TopicDto
    {
        public string Uid { get; set; }
        public string LessonUid { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int PageNumber { get; set; }
        public int OrderIndex { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}

