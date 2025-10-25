using System;
using System.Collections.Generic;

namespace CyberCity.DTOs.Quiz
{
    public class QuizDto
    {
        public string Uid { get; set; }
        public string LessonUid { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int? TotalQuestions { get; set; }
        public DateTime? CreatedAt { get; set; }
    }

    public class CreateQuizDto
    {
        public string LessonUid { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }

    public class UpdateQuizDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
    }

    public class QuizDetailDto : QuizDto
    {
        public List<QuizQuestionDto> Questions { get; set; } = new List<QuizQuestionDto>();
    }
}
