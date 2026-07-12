using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using SHOP.CO.Application.DTOs;
using SHOP.CO.Application.Services;
using SHOP.CO.Application.Utilities;
using System.Threading.Tasks;

namespace SHOP.CO.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto request)
        {
            var result = await _authService.LoginAsync(request);
            return StatusCode(result.Code, result);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto request)
        {
            var result = await _authService.RegisterAsync(request);
            return StatusCode(result.Code, result);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto dto)
        {
            if (string.IsNullOrEmpty(dto.RefreshToken)) return BadRequest("Token bị trống");
            var result = await _authService.RefreshTokenAsync(dto);
            return StatusCode(result.Code, result);
        }

        [Authorize] // Bắt buộc truyền AccessToken trên Header mới gọi được API này
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            // Trích xuất ID của User đang gọi API từ JWT Token
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;
            if (int.TryParse(userIdClaim, out int userId))
            {
                var result = await _authService.LogoutAsync(userId);
                return StatusCode(result.Code, result);
            }
            return Unauthorized();
        }

    }
}
