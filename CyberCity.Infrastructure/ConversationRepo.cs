using CyberCity.Doman.DBContext;
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
    public class ConversationRepo : GenericRepository<Conversation>
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
            var conversationIdString = conversationId.ToString();
            return await _context.Conversations.Where(c => c.Uid == conversationIdString).ToListAsync();
        }

        public async Task<List<Conversation>> GetConversationsByUserIdAsync(Guid userId)
        {
            var userIdString = userId.ToString();
            return await _context.Conversations
                .Include(c => c.ConversationMembers)
                    .ThenInclude(cm => cm.UserU)
                .Include(c => c.Messages.OrderByDescending(m => m.SentAt).Take(1))
                    .ThenInclude(m => m.SenderU)
                .Where(c => c.ConversationMembers.Any(cm => cm.UserUid == userIdString))
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<Conversation> GetConversationByIdAsync(Guid conversationId)
        {
            var conversationIdString = conversationId.ToString();
            return await _context.Conversations
                .Include(c => c.ConversationMembers)
                    .ThenInclude(cm => cm.UserU)
                .Include(c => c.Messages)
                    .ThenInclude(m => m.SenderU)
                .FirstOrDefaultAsync(c => c.Uid == conversationIdString);
        }

        public async Task<Conversation> GetConversationWithMembersAsync(Guid conversationId)
        {
            var conversationIdString = conversationId.ToString();
            return await _context.Conversations
                .Include(c => c.ConversationMembers)
                    .ThenInclude(cm => cm.UserU)
                .FirstOrDefaultAsync(c => c.Uid == conversationIdString);
        }

        public async Task<bool> IsUserMemberOfConversationAsync(Guid conversationId, Guid userId)
        {
            var conversationIdString = conversationId.ToString();
            var userIdString = userId.ToString();
            return await _context.ConversationMembers
                .AnyAsync(cm => cm.ConversationUid == conversationIdString && cm.UserUid == userIdString);
        }
    }
}
