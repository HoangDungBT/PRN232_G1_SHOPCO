using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SHOP.CO.Application.DTOs;
using SHOP.CO.Application.Services;
using System.Threading.Tasks;
using Microsoft.AspNetCore.OData.Query;
using System.Linq;

namespace SHOP.CO.API.Controllers
{
    [Route("api/admin/notifications")]
    [ApiController]
    [Authorize(Roles = "Admin,Staff")]
    public class AdminNotificationsController : ControllerBase
    {
        private readonly IAdminNotificationService _adminNotificationService;

        public AdminNotificationsController(IAdminNotificationService adminNotificationService)
        {
            _adminNotificationService = adminNotificationService;
        }

        [HttpGet]
        [EnableQuery]
        public IActionResult GetNotifications()
        {
            return Ok(_adminNotificationService.GetNotificationsQuery());
        }

        [HttpPost]
        public async Task<IActionResult> CreateNotification([FromBody] SaveNotificationDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _adminNotificationService.CreateNotificationAsync(request);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNotification(int id)
        {
            var success = await _adminNotificationService.DeleteNotificationAsync(id);
            if (!success)
            {
                return NotFound(new { message = "Notification not found or cannot be deleted." });
            }
            return Ok(new { message = "Notification deleted successfully." });
        }
    }
}
