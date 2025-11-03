using CyberCity.DTOs.Quizzes;

namespace CyberCity.Application.Interface
{
    public interface IQuizService
    {
        Task<QuizDetailDto> GetQuizByIdAsync(string quizId, string studentId);
        Task<SubmitQuizResponseDto> SubmitQuizAsync(string quizId, string studentId, SubmitQuizDto submitDto);
    }
}

