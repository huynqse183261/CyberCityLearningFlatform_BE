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

        public async Task<List<Conversation>> GetByAssignmentUidAsync(string conversationId)
        {
            return await _context.Conversations.Where(c => c.Uid == conversationId).ToListAsync();
        }

        public async Task<List<Conversation>> GetConversationsByUserIdAsync(string userId)
        {
            return await _context.Conversations
                .Include(c => c.ConversationMembers)
                    .ThenInclude(cm => cm.UserU)
                .Include(c => c.Messages.OrderByDescending(m => m.SentAt).Take(1))
                    .ThenInclude(m => m.SenderU)
                .Where(c => c.ConversationMembers.Any(cm => cm.UserUid == userId))
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<Conversation> GetConversationByIdAsync(string conversationId)
        {
            return await _context.Conversations
                .Include(c => c.ConversationMembers)
                    .ThenInclude(cm => cm.UserU)
                .Include(c => c.Messages)
                    .ThenInclude(m => m.SenderU)
                .FirstOrDefaultAsync(c => c.Uid == conversationId);
        }

        public async Task<Conversation> GetConversationWithMembersAsync(string conversationId)
        {
            return await _context.Conversations
                .Include(c => c.ConversationMembers)
                    .ThenInclude(cm => cm.UserU)
                .FirstOrDefaultAsync(c => c.Uid == conversationId);
        }

        public async Task<bool> IsUserMemberOfConversationAsync(string conversationId, string userId)
        {
            return await _context.ConversationMembers
                .AnyAsync(cm => cm.ConversationUid == conversationId && cm.UserUid == userId);
        }
    }
}
