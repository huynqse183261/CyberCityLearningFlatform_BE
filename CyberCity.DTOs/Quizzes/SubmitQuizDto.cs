namespace CyberCity.DTOs.Quizzes
{
    public class SubmitQuizDto
    {
        public List<QuizAnswerSubmission> Answers { get; set; }
    }

    public class QuizAnswerSubmission
    {
        public string QuestionId { get; set; }
        public string SelectedAnswerId { get; set; }
    }

    public class SubmitQuizResponseDto
    {
        public QuizSubmissionDto Submission { get; set; }
        public decimal Score { get; set; }
        public int TotalQuestions { get; set; }
        public int CorrectAnswers { get; set; }
        public List<QuizResultDetailDto> Details { get; set; }
    }

    public class QuizResultDetailDto
    {
        public string QuestionId { get; set; }
        public bool IsCorrect { get; set; }
        public string CorrectAnswerId { get; set; }
    }
}

