using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SHOP.CO.Application.DTOs;
using SHOP.CO.Application.Services;
using System.Security.Claims;

namespace SHOP.CO.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Yêu cầu phải đăng nhập mới gọi được các API trong đây
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        // Lấy UserId từ Token đang gửi lên
        private int GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out var userId) ? userId : 0;
        }

        // 1. [HttpGet("profile")] -> Trả về UserProfileDto
        [HttpGet("profile")]
        public async Task<ActionResult<UserProfileDto>> GetProfile()
        {
            var userId = GetUserId();
            var profile = await _userService.GetProfileAsync(userId);
            if (profile == null) return NotFound("Không tìm thấy người dùng.");
            return Ok(profile);
        }

        // 2. [HttpPut("profile")] -> Cập nhật thông tin
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UserProfileDto dto)
        {
            var userId = GetUserId();
            var result = await _userService.UpdateProfileAsync(userId, dto);
            if (!result) return BadRequest("Cập nhật thông tin thất bại.");
            return Ok("Cập nhật hồ sơ thành công.");
        }

        // 3. [HttpPost("change-password")] -> Đổi mật khẩu
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            var userId = GetUserId();
            var result = await _userService.ChangePasswordAsync(userId, dto);
            if (!result) return BadRequest("Mật khẩu hiện tại không đúng hoặc thay đổi thất bại.");
            return Ok("Đổi mật khẩu thành công.");
        }

        // 4. [HttpDelete("delete-account")] -> Xóa tài khoản (Soft Delete)
        [HttpDelete("delete-account")]
        public async Task<IActionResult> DeleteAccount()
        {
            var userId = GetUserId();
            var result = await _userService.DeleteAccountAsync(userId);
            if (!result) return NotFound("Không tìm thấy tài khoản.");
            return Ok("Tài khoản đã được yêu cầu xóa.");
        }

        // 5. [HttpPut("newsletter")] -> Đăng ký/Hủy nhận tin
        [HttpPut("newsletter")]
        public async Task<IActionResult> ToggleNewsletter([FromQuery] bool subscribe)
        {
            var userId = GetUserId();
            var result = await _userService.ToggleNewsletterAsync(userId, subscribe);
            if (!result) return BadRequest("Thao tác thất bại.");
            return Ok(subscribe ? "Đã đăng ký nhận bản tin." : "Đã hủy nhận bản tin.");
        }
    }
}