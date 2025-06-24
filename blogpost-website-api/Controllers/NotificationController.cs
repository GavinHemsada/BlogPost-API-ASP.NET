using blogpost_website_api.DTO;
using blogpost_website_api.Service;
using Microsoft.AspNetCore.Mvc;

namespace blogpost_website_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationController : ControllerBase
    {
        private readonly NotificationService _notificationService;

        public NotificationController(NotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        // Get all notifications for current user (token-based)
        [HttpGet("user")]
        public async Task<IActionResult> GetUserNotifications()
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var result = await _notificationService.FindUserNotification(token);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        // Get all notifications (admin/global)
        [HttpGet("all")]
        public async Task<IActionResult> GetAllNotifications()
        {
            var result = await _notificationService.FindAllNotifications();
            return result.Success ? Ok(result) : BadRequest(result);
        }

        // Update notification read state
        [HttpPut("read/{notificationId}")]
        public async Task<IActionResult> SetNotificationRead(string notificationId, [FromQuery] bool isRead = true)
        {
            var result = await _notificationService.SetNotificationStates(isRead, notificationId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        // Create notification for a single user
        [HttpPost("create")]
        public async Task<IActionResult> CreateNotification([FromBody] NotificationDTO notificationDto)
        {
            var result = await _notificationService.CreateNotification(notificationDto);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        // Send global notification to all users
        [HttpPost("broadcast")]
        public async Task<IActionResult> SendGlobalNotification([FromQuery] string title, [FromQuery] string message, [FromQuery] string link = "")
        {
            var result = await _notificationService.SendGlobalNotification(title, message, link);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
