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
    public class ConversationMemberRepo: GenericRepository<ConversationMember>
    {
        public ConversationMemberRepo() { }
        public ConversationMemberRepo(CyberCityLearningFlatFormDBContext context) => _context = context;
        
        public IQueryable<ConversationMember> GetAllAsync()
        {
            var query = _context.ConversationMembers.AsQueryable();
            return query.OrderByDescending(c => c.JoinedAt);
        }
        
        public async Task<List<ConversationMember>> GetByConversationUidAsync(string conversationUid)
        {
            return await _context.ConversationMembers
                .Include(cm => cm.UserU)
                .Where(c => c.ConversationUid == conversationUid)
                .ToListAsync();
        }

        public async Task<ConversationMember> GetMemberAsync(string conversationUid, string userUid)
        {
            return await _context.ConversationMembers
                .Include(cm => cm.UserU)
                .FirstOrDefaultAsync(cm => cm.ConversationUid == conversationUid && cm.UserUid == userUid);
        }

        public async Task<bool> IsMemberAsync(string conversationUid, string userUid)
        {
            return await _context.ConversationMembers
                .AnyAsync(cm => cm.ConversationUid == conversationUid && cm.UserUid == userUid);
        }

        public async Task RemoveMemberAsync(string conversationUid, string userUid)
        {
            var member = await _context.ConversationMembers
                .FirstOrDefaultAsync(cm => cm.ConversationUid == conversationUid && cm.UserUid == userUid);
            
            if (member != null)
            {
                _context.ConversationMembers.Remove(member);
                await _context.SaveChangesAsync();
            }
        }

        public async Task AddMembersAsync(string conversationUid, string[] userUids)
        {
            // Check if all users exist first
            var existingUsers = await _context.Users
                .Where(u => userUids.Contains(u.Uid))
                .Select(u => u.Uid)
                .ToListAsync();

            if (existingUsers.Count != userUids.Length)
            {
                var missingUsers = userUids.Except(existingUsers).ToList();
                throw new ArgumentException($"Users not found: {string.Join(", ", missingUsers)}");
            }

            var existingMembers = await _context.ConversationMembers
                .Where(cm => cm.ConversationUid == conversationUid && userUids.Contains(cm.UserUid))
                .Select(cm => cm.UserUid)
                .ToListAsync();

            var newMembers = userUids
                .Where(uid => !existingMembers.Contains(uid))
                .Select(uid => new ConversationMember
                {
                    Uid = Guid.NewGuid().ToString(),
                    ConversationUid = conversationUid,
                    UserUid = uid,
                    JoinedAt = DateTime.Now
                })
                .ToList();

            if (newMembers.Any())
            {
                await _context.ConversationMembers.AddRangeAsync(newMembers);
                await _context.SaveChangesAsync();
            }
        }
    }
}
