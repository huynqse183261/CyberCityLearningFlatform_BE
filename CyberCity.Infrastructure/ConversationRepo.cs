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
    public class ConversationRepo: GenericRepository<Conversation>
    {
        public ConversationRepo() { }
        public ConversationRepo(CyberCityLearningFlatFormDBContext context) => _context = context;
        public IQueryable<Conversation> GetAllAsync(bool descending = true)
        {
            var query = _context.Conversations.AsQueryable();
            return descending
                ? query.OrderByDescending(c => c.CreatedAt)
                : query.OrderBy(c => c.CreatedAt);
        }
        public async Task<List<Conversation>> GetByAssignmentUidAsync(Guid conversationId)
        {
            return await _context.Conversations.Where(c => c.Uid == conversationId).ToListAsync();
        }
    }
}
