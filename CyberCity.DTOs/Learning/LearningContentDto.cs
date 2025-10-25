using CyberCity.DTOs.Lessons;
using CyberCity.DTOs.Topics;
using CyberCity.DTOs.Subtopics;
using CyberCity.DTOs.Quizzes;

namespace CyberCity.DTOs.Learning
{
    public class LearningContentDto
    {
        public LessonDto Lesson { get; set; }
        public List<TopicWithSubtopicsDto> Content { get; set; }
        public QuizDto Quiz { get; set; }
        public NavigationDto Navigation { get; set; }
    }

    public class TopicWithSubtopicsDto
    {
        public TopicDto Topic { get; set; }
        public List<SubtopicWithProgressDto> Subtopics { get; set; }
    }

    public class SubtopicWithProgressDto
    {
        public string Uid { get; set; }
        public string TopicUid { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public int OrderIndex { get; set; }
        public bool IsCompleted { get; set; }
        public bool HasAnswer { get; set; }
        public DateTime? CreatedAt { get; set; }
    }

    public class NavigationDto
    {
        public NavigationItemDto PrevLesson { get; set; }
        public NavigationItemDto NextLesson { get; set; }
        public NavigationItemDto CurrentModule { get; set; }
    }

    public class NavigationItemDto
    {
        public string Id { get; set; }
        public string Title { get; set; }
    }
}

