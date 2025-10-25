using System;
using System.Collections.Generic;

namespace CyberCity.DTOs.Quiz
{
    public class QuizSubmissionDto
    {
        public string Uid { get; set; }
        public string QuizUid { get; set; }
        public string StudentUid { get; set; }
        public decimal? Score { get; set; }
        public DateTime? SubmittedAt { get; set; }
        public List<QuizSubmissionAnswerDto> SubmissionAnswers { get; set; } = new List<QuizSubmissionAnswerDto>();
    }

    public class CreateQuizSubmissionDto
    {
        public string QuizUid { get; set; }
        public string StudentUid { get; set; }
        public List<QuizSubmissionAnswerDto> SubmissionAnswers { get; set; } = new List<QuizSubmissionAnswerDto>();
    }

    public class QuizSubmissionAnswerDto
    {
        public string Uid { get; set; }
        public string SubmissionUid { get; set; }
        public string QuestionUid { get; set; }
        public string SelectedAnswerUid { get; set; }
        public bool? IsCorrect { get; set; }
    }

    public class CreateQuizSubmissionAnswerDto
    {
        public string QuestionUid { get; set; }
        public string SelectedAnswerUid { get; set; }
    }
}
