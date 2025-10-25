namespace CyberCity.DTOs.Quizzes
{
    public class QuizDetailDto
    {
        public QuizDto Quiz { get; set; }
        public List<QuestionWithAnswersDto> Questions { get; set; }
        public QuizSubmissionDto UserSubmission { get; set; }
    }

    public class QuestionWithAnswersDto
    {
        public QuizQuestionDto Question { get; set; }
        public List<QuizAnswerDto> Answers { get; set; }
    }
}

