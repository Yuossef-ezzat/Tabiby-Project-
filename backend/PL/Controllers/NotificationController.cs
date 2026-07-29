using BLL.Services.AbstractServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PL.Extention;
using PresentationLayer.Controller;

namespace PL.Controllers
{
    public class NotificationController(INotificationService _notificationService) : ApiControllerBase
    {
        [HttpGet("GetNotifications")]
        public async Task<IActionResult> GetNotifications()
        {
            var result = await _notificationService.GetNotificationsAsync(User.GetUserId());
            
            return Ok(result);
        }
        [HttpDelete("DeleteNotification")]
        public async Task<IActionResult> DeleteNotification(int notificationId)
        {
            var result = await _notificationService.DeleteNotificationAsync(notificationId);
            if (result.IsFailure)
                return BadRequest(result.Error);
            return NoContent();
        }
        [HttpGet("GetUnreadCount")]
        public async Task<IActionResult> GetUnreadCount()
        {
            var result = await _notificationService.GetUnreadCountAsync(User.GetUserId());
            if (result.IsFailure)
                return BadRequest(result.Error);
            return Ok(result.Value);
        }
        [HttpPost("MarkAsRead")]
        public async Task<IActionResult> MarkAsRead(int notificationId)
        {
            var result = await _notificationService.MarkAsReadAsync(notificationId);
            if (result.IsFailure)
                return BadRequest(result.Error);
            return NoContent();
        }

    }
}
