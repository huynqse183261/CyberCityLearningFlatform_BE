using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CyberCity.Doman.DBContext;
using CyberCity.Doman.Models;
using CyberCity.Infrastructure.Basic;
using Microsoft.EntityFrameworkCore;

namespace CyberCity.Infrastructure
{
    public class LessonRepo: GenericRepository<Lesson>
    {
        public LessonRepo() { }
        public LessonRepo(CyberCityLearningFlatFormDBContext context) => _context = context;
        public IQueryable<Lesson> GetAllAsync(bool descending = true)
        {
           var query = _context.Lessons
                .Include(l => l.ModuleU)
                .AsQueryable();
            return descending
                ? query.OrderByDescending(c => c.CreatedAt)
                : query.OrderBy(c => c.CreatedAt);
        }
    }
}
