namespace CyberCity.DTOs.Quizzes
{
    public class QuizDto
    {
        public string Uid { get; set; }
        public string LessonUid { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int TotalQuestions { get; set; }
        public DateTime? CreatedAt { get; set; }
    }

    public class QuizQuestionDto
    {
        public string Uid { get; set; }
        public string QuizUid { get; set; }
        public string QuestionText { get; set; }
        public string QuestionType { get; set; } // single_choice, multiple_choice, true_false, text
        public int OrderIndex { get; set; }
        public DateTime? CreatedAt { get; set; }
    }

    public class QuizAnswerDto
    {
        public string Uid { get; set; }
        public string QuestionUid { get; set; }
        public string AnswerText { get; set; }
        public bool IsCorrect { get; set; }
        public DateTime? CreatedAt { get; set; }
    }

    public class QuizSubmissionDto
    {
        public string Uid { get; set; }
        public string QuizUid { get; set; }
        public string StudentUid { get; set; }
        public decimal Score { get; set; }
        public DateTime? SubmittedAt { get; set; }
    }

    public class QuizSubmissionAnswerDto
    {
        public string Uid { get; set; }
        public string SubmissionUid { get; set; }
        public string QuestionUid { get; set; }
        public string SelectedAnswerUid { get; set; }
        public bool IsCorrect { get; set; }
    }
}

