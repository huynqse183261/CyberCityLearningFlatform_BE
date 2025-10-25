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
        
        public async Task<List<ConversationMember>> GetByConversationUidAsync(Guid conversationUid)
        {
            var conversationUidString = conversationUid.ToString();
            return await _context.ConversationMembers
                .Include(cm => cm.UserU)
                .Where(c => c.ConversationUid == conversationUidString)
                .ToListAsync();
        }

        public async Task<ConversationMember> GetMemberAsync(Guid conversationUid, Guid userUid)
        {
            var conversationUidString = conversationUid.ToString();
            var userUidString = userUid.ToString();
            return await _context.ConversationMembers
                .Include(cm => cm.UserU)
                .FirstOrDefaultAsync(cm => cm.ConversationUid == conversationUidString && cm.UserUid == userUidString);
        }

        public async Task<bool> IsMemberAsync(Guid conversationUid, Guid userUid)
        {
            var conversationUidString = conversationUid.ToString();
            var userUidString = userUid.ToString();
            return await _context.ConversationMembers
                .AnyAsync(cm => cm.ConversationUid == conversationUidString && cm.UserUid == userUidString);
        }

        public async Task RemoveMemberAsync(Guid conversationUid, Guid userUid)
        {
            var conversationUidString = conversationUid.ToString();
            var userUidString = userUid.ToString();
            var member = await _context.ConversationMembers
                .FirstOrDefaultAsync(cm => cm.ConversationUid == conversationUidString && cm.UserUid == userUidString);
            
            if (member != null)
            {
                _context.ConversationMembers.Remove(member);
                await _context.SaveChangesAsync();
            }
        }

        public async Task AddMembersAsync(Guid conversationUid, Guid[] userUids)
        {
            var conversationUidString = conversationUid.ToString();
            var userUidStrings = userUids.Select(u => u.ToString()).ToArray();
            
            // Check if all users exist first
            var existingUsers = await _context.Users
                .Where(u => userUidStrings.Contains(u.Uid))
                .Select(u => u.Uid)
                .ToListAsync();

            if (existingUsers.Count != userUidStrings.Length)
            {
                var missingUsers = userUidStrings.Except(existingUsers).ToList();
                throw new ArgumentException($"Users not found: {string.Join(", ", missingUsers)}");
            }

            var existingMembers = await _context.ConversationMembers
                .Where(cm => cm.ConversationUid == conversationUidString && userUidStrings.Contains(cm.UserUid))
                .Select(cm => cm.UserUid)
                .ToListAsync();

            var newMembers = userUidStrings
                .Where(uid => !existingMembers.Contains(uid))
                .Select(uid => new ConversationMember
                {
                    Uid = Guid.NewGuid().ToString(),
                    ConversationUid = conversationUidString,
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
