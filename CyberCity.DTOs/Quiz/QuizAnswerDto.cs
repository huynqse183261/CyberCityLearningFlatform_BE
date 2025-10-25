using System;

namespace CyberCity.DTOs.Quiz
{
    public class QuizAnswerDto
    {
        public string Uid { get; set; }
        public string QuestionUid { get; set; }
        public string AnswerText { get; set; }
        public bool? IsCorrect { get; set; }
        public DateTime? CreatedAt { get; set; }
    }

    public class CreateQuizAnswerDto
    {
        public string QuestionUid { get; set; }
        public string AnswerText { get; set; }
        public bool? IsCorrect { get; set; }
    }

    public class UpdateQuizAnswerDto
    {
        public string AnswerText { get; set; }
        public bool? IsCorrect { get; set; }
    }
}
