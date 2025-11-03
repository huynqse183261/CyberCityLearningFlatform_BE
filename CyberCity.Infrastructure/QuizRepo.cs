using CyberCity.Doman.DBContext;
using CyberCity.Doman.Models;
using CyberCity.Infrastructure.Basic;
using Microsoft.EntityFrameworkCore;

namespace CyberCity.Infrastructure
{
    public interface IQuizRepository
    {
        Task<Quiz> GetByIdAsync(string id);
        Task<Quiz> GetQuizByLessonIdAsync(string lessonId);
        Task<List<QuizQuestion>> GetQuizQuestionsAsync(string quizId);
        Task<List<QuizAnswer>> GetQuestionAnswersAsync(string questionId);
        Task<QuizSubmission> GetUserSubmissionAsync(string quizId, string studentId);
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

        public async Task<Quiz> GetQuizByLessonIdAsync(string lessonId)
        {
            return await _dbContext.Quizzes
                .FirstOrDefaultAsync(q => q.LessonUid == lessonId);
        }

        public async Task<List<QuizQuestion>> GetQuizQuestionsAsync(string quizId)
        {
            return await _dbContext.QuizQuestions
                .Where(qq => qq.QuizUid == quizId)
                .OrderBy(qq => qq.OrderIndex)
                .ToListAsync();
        }

        public async Task<List<QuizAnswer>> GetQuestionAnswersAsync(string questionId)
        {
            return await _dbContext.QuizAnswers
                .Where(qa => qa.QuestionUid == questionId)
                .ToListAsync();
        }

        public async Task<QuizSubmission> GetUserSubmissionAsync(string quizId, string studentId)
        {
            return await _dbContext.QuizSubmissions
                .FirstOrDefaultAsync(qs => qs.QuizUid == quizId && qs.StudentUid == studentId);
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

