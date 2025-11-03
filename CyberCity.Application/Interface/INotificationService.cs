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
        public Task<List<NotificationResponse>> GetNotificationsAsync(string userId);
        public Task MarkAllAsReadAsync(string userId);
        public Task DeleteAllNotificationsAsync(string userId);
        public Task<int> GetUnreadCountAsync(string userUid);
        public Task SendNotificationAsync(NotificationRequest request);
        public Task MarkNotificationAsReadAsync(string notificationId);

        // New methods for deleting notifications with deleterUid
        public Task DeleteAllNotificationsByUserAsync(string receiverUid, string deleterUid);
        public Task DeleteNotificationByIdAsync(string notificationUid, string deleterUid);
        
    }
}
