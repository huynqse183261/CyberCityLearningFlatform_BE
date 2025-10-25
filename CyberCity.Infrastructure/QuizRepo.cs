using CyberCity.Doman.DBContext;
using CyberCity.Doman.Models;
using CyberCity.Infrastructure.Basic;
using Microsoft.EntityFrameworkCore;

namespace CyberCity.Infrastructure
{
    public interface IQuizRepository
    {
        Task<Quiz> GetByIdAsync(Guid id);
        Task<Quiz> GetQuizByLessonIdAsync(Guid lessonId);
        Task<List<QuizQuestion>> GetQuizQuestionsAsync(Guid quizId);
        Task<List<QuizAnswer>> GetQuestionAnswersAsync(Guid questionId);
        Task<QuizSubmission> GetUserSubmissionAsync(Guid quizId, Guid studentId);
        Task<QuizSubmission> CreateSubmissionAsync(QuizSubmission submission);
        Task CreateSubmissionAnswersAsync(List<QuizSubmissionAnswer> answers);
    }

    public class QuizRepo : GenericRepository<Quiz>, IQuizRepository
    {
        private readonly CyberCityLearningFlatFormDBContext _dbContext;

        public QuizRepo(CyberCityLearningFlatFormDBContext context) : base(context)
        {
            _dbContext = context;
        }

        public async Task<Quiz> GetQuizByLessonIdAsync(Guid lessonId)
        {
            return await _dbContext.Quizzes
                .FirstOrDefaultAsync(q => q.LessonUid == lessonId.ToString());
        }

        public async Task<List<QuizQuestion>> GetQuizQuestionsAsync(Guid quizId)
        {
            return await _dbContext.QuizQuestions
                .Where(qq => qq.QuizUid == quizId.ToString())
                .OrderBy(qq => qq.OrderIndex)
                .ToListAsync();
        }

        public async Task<List<QuizAnswer>> GetQuestionAnswersAsync(Guid questionId)
        {
            return await _dbContext.QuizAnswers
                .Where(qa => qa.QuestionUid == questionId.ToString())
                .ToListAsync();
        }

        public async Task<QuizSubmission> GetUserSubmissionAsync(Guid quizId, Guid studentId)
        {
            return await _dbContext.QuizSubmissions
                .FirstOrDefaultAsync(qs => qs.QuizUid == quizId.ToString() && qs.StudentUid == studentId.ToString());
        }

        public async Task<QuizSubmission> CreateSubmissionAsync(QuizSubmission submission)
        {
            await _dbContext.QuizSubmissions.AddAsync(submission);
            await _dbContext.SaveChangesAsync();
            return submission;
        }

        public async Task CreateSubmissionAnswersAsync(List<QuizSubmissionAnswer> answers)
        {
            await _dbContext.QuizSubmissionAnswers.AddRangeAsync(answers);
            await _dbContext.SaveChangesAsync();
        }
    }
}

