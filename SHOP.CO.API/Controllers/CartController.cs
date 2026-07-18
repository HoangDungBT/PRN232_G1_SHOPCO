using Microsoft.AspNetCore.Mvc;
using SHOP.CO.Application.DTOs;
using SHOP.CO.Application.Services;

namespace SHOP.CO.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        /// <summary>
        /// Add product variant to user's cart or increase quantity if already exists
        /// </summary>
        /// <param name="userId">User identifier (query parameter)</param>
        /// <param name="request">AddToCartRequestDto in request body</param>
        /// <returns>JSON result with success flag and message</returns>
        [HttpPost]
        [Produces("application/json")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(typeof(object), 400)]
        public async Task<IActionResult> AddToCart([FromQuery] int userId, [FromBody] AddToCartRequestDto request)
        {
            try
            {
                var result = await _cartService.AddToCartAsync(userId, request);

                return Ok(new { success = true, message = "Item added to cart", data = result });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { success = false, message = "An unexpected error occurred." });
            }
        }

        /// <summary>
        /// Get all cart items for a specific user
        /// </summary>
        /// <param name="userId">User identifier in route</param>
        /// <returns>Cart DTO</returns>
        [HttpGet("{userId:int}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(typeof(object), 400)]
        public async Task<IActionResult> GetCart([FromRoute] int userId)
        {
            try
            {
                var cart = await _cartService.GetCartByUserIdAsync(userId);
                return Ok(new { success = true, message = "Cart retrieved", data = cart });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { success = false, message = "An unexpected error occurred." });
            }
        }

        /// <summary>
        /// Remove an item from user's cart
        /// </summary>
        /// <param name="cartItemId">Cart item id in route</param>
        /// <param name="userId">User identifier (query parameter) used for simple authorization</param>
        /// <returns>JSON result</returns>
        [HttpDelete("{cartItemId:int}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(typeof(object), 400)]
        public async Task<IActionResult> RemoveFromCart([FromRoute] int cartItemId, [FromQuery] int userId)
        {
            try
            {
                await _cartService.RemoveFromCartAsync(cartItemId, userId);
                return Ok(new { success = true, message = "Item removed from cart" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { success = false, message = "An unexpected error occurred." });
            }
        }

        /// <summary>
        /// Update a cart item quantity
        /// </summary>
        /// <param name="cartItemId">Cart item ID in route</param>
        /// <param name="userId">User identifier in query</param>
        /// <param name="request">UpdateCartQuantityRequest in body</param>
        /// <returns>JSON result</returns>
        [HttpPut("{cartItemId:int}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(typeof(object), 400)]
        public async Task<IActionResult> UpdateQuantity([FromRoute] int cartItemId, [FromQuery] int userId, [FromBody] UpdateCartQuantityRequest request)
        {
            try
            {
                await _cartService.UpdateCartQuantityAsync(cartItemId, userId, request);
                return Ok(new { success = true, message = "Cart quantity updated" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new { success = false, message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { success = false, message = "An unexpected error occurred." });
            }
        }
    }
}

