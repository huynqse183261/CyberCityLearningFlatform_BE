using AutoMapper;
using CyberCity.Application.Interface;
using CyberCity.Doman.Models;
using CyberCity.DTOs.Notifications;
using CyberCity.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberCity.Application.Implement
{
    public class NotificationService : INotificationService
    {
        private readonly NotificationRepo _notificationRepo;
        private IMapper _mapper;
        public NotificationService(NotificationRepo notificationRepo, IMapper mapper)
        {
            _notificationRepo = notificationRepo;
            _mapper = mapper;
        }
        public async Task DeleteAllNotificationsAsync(Guid userId)
        {
            var notifications = await _notificationRepo.GetByIdAsync(userId);
            if (notifications != null)
            {
                await _notificationRepo.RemoveAsync(notifications);
            }

        }

        public async Task DeleteAllNotificationsByUserAsync(Guid receiverUid, Guid deleterUid)
        {
            var notifications = await _notificationRepo.DeleteAllByReceiverUidAsync(receiverUid, deleterUid);
            return;
        }

        public async Task DeleteNotificationByIdAsync(Guid notificationUid, Guid deleterUid)
        {
            var result = await _notificationRepo.DeleteNotificationAsync(notificationUid, deleterUid);
            return ;
        }

        public async Task<List<NotificationResponse>> GetNotificationsAsync(Guid userId)
        {
            // Lấy danh sách notification và include navigation property để lấy username
          var notifications = await _notificationRepo.GetNotificationsInfoAsync(userId);

            // Map sang NotificationResponse để trả về cả username
            var result = _mapper.Map<List<NotificationResponse>>(notifications);
            return result;
        }

        public async Task<int> GetUnreadCountAsync(Guid userUid)
        {
            var count = await _notificationRepo.GetUnreadCountByReceiverUidAsync(userUid);
            return count;
        }

        public async Task MarkAllAsReadAsync(Guid userId)
        {
            await _notificationRepo.MarkAllAsReadByReceiverUidAsync(userId);
        }

        public async Task SendNotificationAsync(NotificationRequest request)
        {
            var notification = _mapper.Map<Notification>(request);
            notification.Uid = Guid.NewGuid().ToString();      
            notification.IsRead = false;
            notification.CreatedAt = DateTime.Now;
            await _notificationRepo.CreateAsync(notification);
        }
        public async Task MarkNotificationAsReadAsync(Guid notificationId)
        {
            var notification = await _notificationRepo.GetByIdAsync(notificationId);
            if (notification != null && (notification.IsRead == null || notification.IsRead == false))
            {
                notification.IsRead = true;
                await _notificationRepo.UpdateAsync(notification);
            }
        }
    }
}
