using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CyberCity.Doman.DBcontext;
using CyberCity.Doman.Models;
using CyberCity.Infrastructure.Basic;
using Microsoft.EntityFrameworkCore;

namespace CyberCity.Infrastructure
{
    public class SubtopicRepo: GenericRepository<Subtopic>
    {
        public SubtopicRepo() { }
        public SubtopicRepo(CyberCityLearningFlatFormDBContext context) => _context = context;
        public IQueryable<Subtopic> GetAllAsync(bool descending = true)
        {
            var query = _context.Subtopics
                .Include(t => t.TopicU)
                .AsQueryable();
            return descending
                ? query.OrderByDescending(c => c.CreatedAt)
                : query.OrderBy(c => c.CreatedAt);
        }
    }
}
