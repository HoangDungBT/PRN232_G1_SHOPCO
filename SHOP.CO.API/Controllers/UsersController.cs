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

        // 6. [HttpGet("addresses")] -> Danh sách địa chỉ (cho User Profile)
        [HttpGet("addresses")]
        public async Task<IActionResult> GetAddresses([FromServices] IOrderService orderService)
        {
            var userId = GetUserId();
            var addresses = await orderService.GetUserAddressesAsync(userId);
            return Ok(addresses); // Trả về List trực tiếp cho AddressBook()
        }

        // 7. [HttpGet("{userId}/addresses")] -> Danh sách địa chỉ (cho CartApiClient)
        [HttpGet("{userId}/addresses")]
        [AllowAnonymous] // Allow CartApiClient to call without token
        public async Task<IActionResult> GetUserAddressesForCart([FromServices] IOrderService orderService, int userId)
        {
            var addresses = await orderService.GetUserAddressesAsync(userId);
            // Wrap trong ApiResponse để khớp với Serialize của CartApiClient
            return Ok(new { Success = true, Data = addresses });
        }

        // 8. [HttpDelete("addresses/{id}")] -> Xóa địa chỉ
        [HttpDelete("addresses/{id}")]
        public async Task<IActionResult> DeleteAddress(int id)
        {
            var userId = GetUserId();
            var result = await _userService.DeleteUserAddressAsync(userId, id);
            if (!result) return NotFound("Address not found.");
            return Ok("Address deleted.");
        }

        // 9. [HttpPost("addresses")] -> Thêm địa chỉ mới
        [HttpPost("addresses")]
        public async Task<IActionResult> AddAddress([FromBody] UserAddressDto dto)
        {
            var userId = GetUserId();
            var newAddress = await _userService.AddUserAddressAsync(userId, dto);
            if (newAddress == null) return BadRequest("Cannot add address.");
            return Ok(newAddress);
        }

        // 10. [HttpPut("addresses/{id}")] -> Cập nhật địa chỉ
        [HttpPut("addresses/{id}")]
        public async Task<IActionResult> UpdateAddress(int id, [FromBody] UserAddressDto dto)
        {
            var userId = GetUserId();
            dto.AddressId = id;
            var result = await _userService.UpdateUserAddressAsync(userId, dto);
            if (!result) return NotFound("Address not found.");
            return Ok("Address updated.");
        }

        // 11. [HttpPatch("addresses/{id}/default")] -> Đặt làm mặc định
        [HttpPatch("addresses/{id}/default")]
        public async Task<IActionResult> SetDefaultAddress(int id)
        {
            var userId = GetUserId();
            var result = await _userService.SetDefaultAddressAsync(userId, id);
            if (!result) return NotFound("Address not found.");
            return Ok("Default address updated.");
        }
    }
}