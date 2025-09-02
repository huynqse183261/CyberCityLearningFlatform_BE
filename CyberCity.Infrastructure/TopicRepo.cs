using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CyberCity.Doman.DBcontext;
using CyberCity.Doman.Models;
using CyberCity.Infrastructure.Basic;

namespace CyberCity.Infrastructure
{
    public class TopicRepo:GenericRepository<Topic>
    {
        public TopicRepo() { }
        public TopicRepo(CyberCityLearningFlatFormDBContext context) => _context = context;
        public IQueryable<Topic> GetAllAsync(bool descending = true)
        {
            var query = _context.Topics.AsQueryable();
            return descending
                ? query.OrderByDescending(c => c.CreatedAt)
                : query.OrderBy(c => c.CreatedAt);
        }
    }
}
