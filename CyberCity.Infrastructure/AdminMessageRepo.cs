using CyberCity.Doman.DBContext;
using CyberCity.Doman.Models;
using CyberCity.DTOs.Admin;
using CyberCity.Infrastructure.Basic;
using Microsoft.EntityFrameworkCore;

namespace CyberCity.Infrastructure
{
    public interface IAdminMessageRepository
    {
        Task<(List<AdminConversationDto> conversations, int totalCount)> GetConversationsAsync(GetConversationsQuery query);
        Task<(List<AdminMessageDto> messages, int totalCount)> GetMessagesAsync(Guid conversationId, GetMessagesQuery query);
        Task<AdminMessageDto?> SendMessageAsync(Guid conversationId, Guid adminUserId, string message);
        Task<bool> DeleteMessageAsync(Guid messageId);
        Task<MessageStatsResponse> GetStatsAsync();
        Task<List<SimpleUserDto>> GetConversationMembersAsync(Guid conversationId);
    }

    public class AdminMessageRepository : GenericRepository<Message>, IAdminMessageRepository
    {
        public AdminMessageRepository() { }
        public AdminMessageRepository(CyberCityLearningFlatFormDBContext context) => _context = context;

        public async Task<(List<AdminConversationDto> conversations, int totalCount)> GetConversationsAsync(GetConversationsQuery query)
        {
            var conversationsQuery = _context.Conversations.AsQueryable();

            // Tìm kiếm theo title
            if (!string.IsNullOrWhiteSpace(query.SearchQuery))
            {
                conversationsQuery = conversationsQuery.Where(c => 
                    EF.Functions.ILike(c.Title, $"%{query.SearchQuery}%"));
            }

            // Đếm tổng
            var totalCount = await conversationsQuery.CountAsync();

            // Lấy dữ liệu với phân trang
            var conversations = await conversationsQuery
                .Select(c => new
                {
                    c.Uid,
                    c.Title,
                    c.IsGroup,
                    c.CreatedAt,
                    TotalMessages = _context.Messages.Count(m => m.ConversationUid == c.Uid),
                    LastMessageAt = _context.Messages
                        .Where(m => m.ConversationUid == c.Uid)
                        .Max(m => (DateTime?)m.SentAt)
                })
                .OrderByDescending(c => c.LastMessageAt ?? c.CreatedAt)
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync();

            var conversationDtos = new List<AdminConversationDto>();
            
            foreach (var conv in conversations)
            {
                var members = await GetConversationMembersAsync(Guid.Parse(conv.Uid));
                
                conversationDtos.Add(new AdminConversationDto
                {
                    Uid = conv.Uid,
                    Title = conv.Title ?? string.Empty,
                    IsGroup = conv.IsGroup ?? false,
                    TotalMessages = conv.TotalMessages,
                    CreatedAt = conv.CreatedAt ?? DateTime.Now,
                    LastMessageAt = conv.LastMessageAt,
                    Members = members
                });
            }

            return (conversationDtos, totalCount);
        }

        public async Task<List<SimpleUserDto>> GetConversationMembersAsync(Guid conversationId)
        {
            var conversationIdString = conversationId.ToString();
            var members = await _context.ConversationMembers
                .Where(cm => cm.ConversationUid == conversationIdString)
                .Join(_context.Users,
                    cm => cm.UserUid,
                    u => u.Uid,
                    (cm, u) => new SimpleUserDto
                    {
                        Uid = u.Uid,
                        Username = u.Username ?? string.Empty,
                        FullName = u.FullName ?? string.Empty,
                        Role = u.Role ?? string.Empty,
                        Image = u.Image
                    })
                .ToListAsync();

            return members;
        }

        public async Task<(List<AdminMessageDto> messages, int totalCount)> GetMessagesAsync(Guid conversationId, GetMessagesQuery query)
        {
            var conversationIdString = conversationId.ToString();
            var messagesQuery = _context.Messages
                .Where(m => m.ConversationUid == conversationIdString);

            var totalCount = await messagesQuery.CountAsync();

            var messages = await messagesQuery
                .Include(m => m.SenderU)
                .OrderByDescending(m => m.SentAt)
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(m => new AdminMessageDto
                {
                    Uid = m.Uid,
                    ConversationUid = m.ConversationUid,
                    SenderUid = m.SenderUid,
                    Message = m.Message1 ?? string.Empty,
                    SentAt = m.SentAt ?? DateTime.Now,
                    Sender = new SimpleUserDto
                    {
                        Uid = m.SenderU.Uid,
                        Username = m.SenderU.Username ?? string.Empty,
                        FullName = m.SenderU.FullName ?? string.Empty,
                        Role = m.SenderU.Role ?? string.Empty,
                        Image = m.SenderU.Image
                    }
                })
                .ToListAsync();

            return (messages, totalCount);
        }

        public async Task<AdminMessageDto?> SendMessageAsync(Guid conversationId, Guid adminUserId, string message)
        {
            var newMessage = new Message
            {
                Uid = Guid.NewGuid().ToString(),
                ConversationUid = conversationId.ToString(),
                SenderUid = adminUserId.ToString(),
                Message1 = message,
                SentAt = DateTime.Now
            };

            await _context.Messages.AddAsync(newMessage);
            await _context.SaveChangesAsync();

            // Lấy thông tin sender
            var adminUserIdString = adminUserId.ToString();
            var sender = await _context.Users
                .Where(u => u.Uid == adminUserIdString)
                .Select(u => new SimpleUserDto
                {
                    Uid = u.Uid,
                    Username = u.Username ?? string.Empty,
                    FullName = u.FullName ?? string.Empty,
                    Role = u.Role ?? string.Empty,
                    Image = u.Image
                })
                .FirstOrDefaultAsync();

            if (sender == null)
                return null;

            return new AdminMessageDto
            {
                Uid = newMessage.Uid,
                ConversationUid = newMessage.ConversationUid,
                SenderUid = newMessage.SenderUid,
                Message = newMessage.Message1 ?? string.Empty,
                SentAt = newMessage.SentAt ?? DateTime.Now,
                Sender = sender
            };
        }

        public async Task<bool> DeleteMessageAsync(Guid messageId)
        {
            var messageIdString = messageId.ToString();
            var message = await _context.Messages.FindAsync(messageIdString);
            if (message == null)
                return false;

            _context.Messages.Remove(message);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<MessageStatsResponse> GetStatsAsync()
        {
            var totalConversations = await _context.Conversations.CountAsync();
            var totalMessages = await _context.Messages.CountAsync();
            
            var today = DateTime.Now.Date;
            var todayMessages = await _context.Messages
                .CountAsync(m => m.SentAt >= today);

            // Tính ngày đầu tuần (Monday)
            var currentDate = DateTime.Now.Date;
            var daysToSubtract = ((int)currentDate.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7;
            var weekStart = currentDate.AddDays(-daysToSubtract);
            
            var thisWeekMessages = await _context.Messages
                .CountAsync(m => m.SentAt >= weekStart);

            return new MessageStatsResponse
            {
                TotalConversations = totalConversations,
                TotalMessages = totalMessages,
                TodayMessages = todayMessages,
                ThisWeekMessages = thisWeekMessages
            };
        }
    }
}
