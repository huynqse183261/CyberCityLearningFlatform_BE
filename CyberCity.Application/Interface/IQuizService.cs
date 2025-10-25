using CyberCity.DTOs.Quizzes;

namespace CyberCity.Application.Interface
{
    public interface IQuizService
    {
        Task<QuizDetailDto> GetQuizByIdAsync(Guid quizId, Guid studentId);
        Task<SubmitQuizResponseDto> SubmitQuizAsync(Guid quizId, Guid studentId, SubmitQuizDto submitDto);
    }
}

