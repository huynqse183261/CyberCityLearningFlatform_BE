using System;
using System.Collections.Generic;

namespace CyberCity.DTOs.Quiz
{
    public class QuizQuestionDto
    {
        public string Uid { get; set; }
        public string QuizUid { get; set; }
        public string QuestionText { get; set; }
        public string QuestionType { get; set; }
        public int? OrderIndex { get; set; }
        public DateTime? CreatedAt { get; set; }
        public List<QuizAnswerDto> Answers { get; set; } = new List<QuizAnswerDto>();
    }

    public class CreateQuizQuestionDto
    {
        public string QuizUid { get; set; }
        public string QuestionText { get; set; }
        public string QuestionType { get; set; }
        public int? OrderIndex { get; set; }
    }

    public class UpdateQuizQuestionDto
    {
        public string QuestionText { get; set; }
        public string QuestionType { get; set; }
        public int? OrderIndex { get; set; }
    }
}
