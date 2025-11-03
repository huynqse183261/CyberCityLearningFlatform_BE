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

        public async Task<List<Message>> GetMessagesByConversationIdAsync(string conversationId, int pageNumber = 1, int pageSize = 50)
        {
            return await _context.Messages
                .Include(m => m.SenderU)
                .Where(m => m.ConversationUid == conversationId)
                .OrderByDescending(m => m.SentAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetMessageCountByConversationIdAsync(string conversationId)
        {
            return await _context.Messages
                .CountAsync(m => m.ConversationUid == conversationId);
        }

        public async Task<Message> GetLatestMessageByConversationIdAsync(string conversationId)
        {
            return await _context.Messages
                .Include(m => m.SenderU)
                .Where(m => m.ConversationUid == conversationId)
                .OrderByDescending(m => m.SentAt)
                .FirstOrDefaultAsync();
        }
    }
}
