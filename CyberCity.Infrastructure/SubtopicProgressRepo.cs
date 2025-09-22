using CyberCity.Doman.DBcontext;
using CyberCity.Doman.Models;
using CyberCity.Infrastructure.Basic;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberCity.Infrastructure
{
    public class SubtopicProgressRepo : GenericRepository<SubtopicProgress>
    {

        public SubtopicProgressRepo() { }
        public SubtopicProgressRepo(CyberCityLearningFlatFormDBContext context) => _context = context;
        public async Task<List<SubtopicProgress>> GetByStudentAsync(Guid studentId)
        {
            var query = _context.SubtopicProgresses
                 .Where(p => p.StudentUid == studentId)
                 .AsQueryable();
            return await query.ToListAsync();
        }

       public IQueryable<SubtopicProgress> GetAllAsync(bool descending = true)
        {
            var query = _context.SubtopicProgresses
                .Include(p => p.StudentU)
                .Include(p => p.SubtopicU)
                .AsQueryable();
            return descending
                ? query.OrderByDescending(c => c.CompletedAt)
                : query.OrderBy(c => c.CompletedAt);
        }
        public async Task<SubtopicProgress?> GetBySubtopicAndStudentAsync(Guid subtopicId, Guid studentId)
        {
            var query = _context.SubtopicProgresses
                .Where(p => p.StudentUid == studentId && p.SubtopicUid == subtopicId)
                .AsQueryable();
            return await query.FirstOrDefaultAsync();
        }

        public async Task MarkCompleteAsync(Guid subtopicId, Guid studentId)
        {
            // Đánh dấu hoàn thành 
            var progress = await GetBySubtopicAndStudentAsync(subtopicId, studentId);
            if (progress == null)
            {
                progress = new SubtopicProgress
                {
                    Uid = Guid.NewGuid(),
                    StudentUid = studentId,
                    SubtopicUid = subtopicId,
                    IsCompleted = true,
                    CompletedAt = DateTime.Now
                };
                await CreateAsync(progress);
            }
            else
            {
                progress.IsCompleted = true;
                progress.CompletedAt = DateTime.Now;
                await UpdateAsync(progress);
            }
        }
    }
}
