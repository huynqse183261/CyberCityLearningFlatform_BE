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
    public class MessageRepo:GenericRepository<Message>
    {
        public MessageRepo() { }
        public MessageRepo(CyberCityLearningFlatFormDBContext context) => _context = context;
        public IQueryable<Message> GetAllAsync(bool descending = true)
        {
            var query = _context.Messages.AsQueryable();
            return descending
                ? query.OrderByDescending(c => c.SentAt)
                : query.OrderBy(c => c.SentAt);
        }
    }
}
