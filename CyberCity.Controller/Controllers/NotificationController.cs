using CyberCity.Application.Interface;
using CyberCity.DTOs.Notifications;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CyberCity.Controller.Controllers
{
    [Route("api/notifications")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private INotificationService _notificationService;
        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetNotifications(string userId)
        {
            var notifications = await _notificationService.GetNotificationsAsync(userId);
            return Ok(notifications);
        }
        [HttpGet("unreadCount/{userId}")]
        public async Task<IActionResult> GetUnreadCount(string userId)
        {
            var count = await _notificationService.GetUnreadCountAsync(userId);
            return Ok(count);
        }
        [HttpPut("mark-all-read")]
        public async Task<IActionResult> MarkAllAsRead([FromQuery] string userId)
        {
            await _notificationService.MarkAllAsReadAsync(userId);
            return Ok();
        }

        [HttpPut("{notificationId}/read")]
        public async Task<IActionResult> MarkNotificationAsRead(string notificationId)
        {
            // Cần bổ sung hàm MarkNotificationAsReadAsync ở service
            await _notificationService.MarkNotificationAsReadAsync(notificationId);
            return Ok();
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendNotification([FromBody] NotificationRequest request)
        {
            await _notificationService.SendNotificationAsync(request);
            return Ok();
        }
        [HttpDelete("deleteAll/{receiverUid}/{deleterUid}")]
        public async Task<IActionResult> DeleteAllNotificationsByUser(string receiverUid, string deleterUid)
        {
            await _notificationService.DeleteAllNotificationsByUserAsync(receiverUid, deleterUid);
            return Ok();
        }
        [HttpDelete("delete/{notificationUid}/{deleterUid}")]
        public async Task<IActionResult> DeleteNotificationById(string notificationUid, string deleterUid)
        {
            await _notificationService.DeleteNotificationByIdAsync(notificationUid, deleterUid);
            return Ok();
        }

    }
}
