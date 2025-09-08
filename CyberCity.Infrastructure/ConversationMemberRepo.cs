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
    public class ConversationMemberRepo: GenericRepository<ConversationMember>
    {
        public ConversationMemberRepo() { }
        public ConversationMemberRepo(CyberCityLearningFlatFormDBContext context) => _context = context;
        public IQueryable<ConversationMember> GetAllAsync()
        {
            var query = _context.ConversationMembers.AsQueryable();
            return query.OrderByDescending(c => c.JoinedAt);
        }
        public async Task<List<ConversationMember>> GetByConversationUidAsync(Guid conversationUid)
        {
            return await _context.ConversationMembers.Where(c => c.ConversationUid == conversationUid).ToListAsync();
        }
    }
}
