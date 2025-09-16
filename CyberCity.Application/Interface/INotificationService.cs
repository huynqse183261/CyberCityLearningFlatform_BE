using CyberCity.DTOs.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberCity.Application.Interface
{
    public interface INotificationService
    {
        public Task<List<NotificationResponse>> GetNotificationsAsync(Guid userId);
        public Task MarkAllAsReadAsync(Guid userId);
        public Task DeleteAllNotificationsAsync(Guid userId);
        public Task<int> GetUnreadCountAsync(Guid userUid);
        public Task SendNotificationAsync(NotificationRequest request);
        public Task MarkNotificationAsReadAsync(Guid notificationId);

        // New methods for deleting notifications with deleterUid
        public Task DeleteAllNotificationsByUserAsync(Guid receiverUid, Guid deleterUid);
        public Task DeleteNotificationByIdAsync(Guid notificationUid, Guid deleterUid);
        
    }
}
