using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SHOP.CO.Application.Services;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SHOP.CO.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class WishlistController : ControllerBase
    {
        private readonly IProductService _productService;

        public WishlistController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpPost("{productId}")]
        public async Task<IActionResult> ToggleWishlist(int productId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized();
            }

            try
            {
                var isAdded = await _productService.ToggleWishlistAsync(productId, userId);
                return Ok(new { isAdded, message = isAdded ? "Đã thêm vào danh sách yêu thích!" : "Đã xóa khỏi danh sách yêu thích!" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetMyWishlist()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized();
            }

            var wishlist = await _productService.GetWishlistAsync(userId);
            return Ok(wishlist);
        }
    }
}
