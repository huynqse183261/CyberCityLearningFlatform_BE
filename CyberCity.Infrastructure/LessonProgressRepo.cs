using CyberCity.Doman.DBcontext;
using CyberCity.Doman.Models;
using CyberCity.Infrastructure.Basic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberCity.Infrastructure
{
    public class LessonProgressRepo:GenericRepository<LessonProgress>
    {
        public LessonProgressRepo() { }
        public LessonProgressRepo(CyberCityLearningFlatFormDBContext context) => _context = context;
        public IQueryable<LessonProgress> GetAllAsync(bool descending = true)
        {
            var query = _context.LessonProgresses.AsQueryable();
            return descending
                ? query.OrderByDescending(c => c.CompletedAt)
                : query.OrderBy(c => c.CompletedAt);
        }
    }
}
