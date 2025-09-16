using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CyberCity.Doman.DBcontext;
using CyberCity.Doman.Models;
using CyberCity.Infrastructure.Basic;
using Microsoft.EntityFrameworkCore;

namespace CyberCity.Infrastructure
{
    public class NotificationRepo:GenericRepository<Notification>
    {
        public NotificationRepo() { }
        public NotificationRepo(CyberCityLearningFlatFormDBContext context) => _context = context;
        public IQueryable<Notification> GetAllAsync(bool descending = true)
        {
            var query = _context.Notifications.AsQueryable();
            return descending
                ? query.OrderByDescending(c => c.CreatedAt)
                : query.OrderBy(c => c.CreatedAt);
        }
        public async Task<List<Notification>> GetNotificationsInfoAsync(Guid receiverUid)
        {
            return await _context.Notifications
                .Include(n => n.SenderU) // Include the Sender navigation property
                .Include(n => n.ReceiverU) // Include the Receiver navigation property
                .Where(n => n.ReceiverUid == receiverUid)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }
        public async Task<List<Notification>> GetAllByReceiverUidAsync(Guid receiverUid)
        {
            return await _context.Notifications.Where(n => n.ReceiverUid == receiverUid).OrderByDescending(n => n.CreatedAt).ToListAsync();
        }
        public async Task<int> GetUnreadCountByReceiverUidAsync(Guid receiverUid)
        {
            return await _context.Notifications.CountAsync(n => n.ReceiverUid == receiverUid && (n.IsRead == null || n.IsRead == false));
        }
        public async Task<int> MarkAllAsReadByReceiverUidAsync(Guid receiverUid)
        {
            var notifications = await _context.Notifications.Where(n => n.ReceiverUid == receiverUid && (n.IsRead == null || n.IsRead == false)).ToListAsync();
            foreach (var notification in notifications)
            {
                notification.IsRead = true;
            }
            return await _context.SaveChangesAsync();
        }
        public async Task<int> SendNotificationAsync(Guid receiverUid, Guid? senderUid, string message)
        {
            var notification = new Notification
            {
                Uid = Guid.NewGuid(),
                ReceiverUid = receiverUid,
                SenderUid = senderUid,
                Message = message,
                IsRead = false,
                CreatedAt = DateTime.Now
            };
            _context.Notifications.Add(notification);
            return await _context.SaveChangesAsync();
        }
        public async Task<int> DeleteAllByReceiverUidAsync(Guid receiverUid, Guid deleterUid)
        {
            // Nếu cần kiểm tra quyền hoặc log người xóa, có thể thực hiện ở đây
            var notifications = await _context.Notifications.Where(n => n.ReceiverUid == receiverUid).ToListAsync();
            // Ví dụ: Ghi log hoặc kiểm tra deleterUid có quyền xóa không
            _context.Notifications.RemoveRange(notifications);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> DeleteNotificationAsync(Guid notificationUid, Guid deleterUid)
        {
            // Nếu cần kiểm tra quyền hoặc log người xóa, có thể thực hiện ở đây
            var notification = await _context.Notifications.FirstOrDefaultAsync(n => n.Uid == notificationUid);
            if (notification != null)
            {
                // Ví dụ: Ghi log hoặc kiểm tra deleterUid có quyền xóa không
                _context.Notifications.Remove(notification);
                return await _context.SaveChangesAsync();
            }
            return 0;
        }
        public async Task<Notification> GetByUidAsync(Guid notificationUid)
        {
            return await _context.Notifications.FirstOrDefaultAsync(n => n.Uid == notificationUid);
        }
        public async Task<int> MarkNotificationAsReadAsync(Guid notificationUid)
        {
            var notification = await _context.Notifications.FirstOrDefaultAsync(n => n.Uid == notificationUid);
            if (notification != null && (notification.IsRead == null || notification.IsRead == false))
            {
                notification.IsRead = true;
                return await _context.SaveChangesAsync();
            }
            return 0;
        }
    }
}
