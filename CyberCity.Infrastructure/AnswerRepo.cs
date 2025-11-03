using CyberCity.Doman.DBContext;
using CyberCity.Doman.Models;
using CyberCity.Infrastructure.Basic;
using Microsoft.EntityFrameworkCore;

namespace CyberCity.Infrastructure
{
    public interface IAnswerRepository
    {
        Task<Answer> GetAnswerBySubtopicIdAsync(string subtopicId);
        Task<SubtopicProgress> GetUserProgressAsync(string studentId, string subtopicId);
        Task<SubtopicProgress> CreateOrUpdateProgressAsync(SubtopicProgress progress);
        Task<List<SubtopicProgress>> GetUserProgressByLessonAsync(string studentId, string lessonId);
    }

    public class AnswerRepo : GenericRepository<Answer>, IAnswerRepository
    {
        private readonly CyberCityLearningFlatFormDBContext _dbContext;

        public AnswerRepo(CyberCityLearningFlatFormDBContext context) : base(context)
        {
            _dbContext = context;
        }

        public async Task<Answer> GetAnswerBySubtopicIdAsync(string subtopicId)
        {
            return await _dbContext.Answers
                .FirstOrDefaultAsync(a => a.SubtopicUid == subtopicId);
        }

        public async Task<SubtopicProgress> GetUserProgressAsync(string studentId, string subtopicId)
        {
            return await _dbContext.SubtopicProgresses
                .FirstOrDefaultAsync(sp => sp.StudentUid == studentId && sp.SubtopicUid == subtopicId);
        }

        public async Task<SubtopicProgress> CreateOrUpdateProgressAsync(SubtopicProgress progress)
        {
            var existing = await GetUserProgressAsync(progress.StudentUid, progress.SubtopicUid);

            if (existing == null)
            {
                await _dbContext.SubtopicProgresses.AddAsync(progress);
            }
            else
            {
                existing.IsCompleted = progress.IsCompleted;
                existing.CompletedAt = progress.CompletedAt;
                existing.UserOutput = progress.UserOutput;
                existing.IsCorrect = progress.IsCorrect;
                existing.AttemptCount = progress.AttemptCount;
                existing.LastAttemptedAt = progress.LastAttemptedAt;
                _dbContext.SubtopicProgresses.Update(existing);
            }

            await _dbContext.SaveChangesAsync();
            return existing ?? progress;
        }

        public async Task<List<SubtopicProgress>> GetUserProgressByLessonAsync(string studentId, string lessonId)
        {
            return await _dbContext.SubtopicProgresses
                .Where(sp => sp.StudentUid == studentId 
                    && _dbContext.Subtopics.Any(st => st.Uid == sp.SubtopicUid 
                        && _dbContext.Topics.Any(t => t.Uid == st.TopicUid && t.LessonUid == lessonId)))
                .ToListAsync();
        }
    }
}
