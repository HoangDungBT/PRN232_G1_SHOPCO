using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SHOP.CO.Application.Services;

namespace SHOP.CO.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public UsersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        /// <summary>
        /// GET /api/users/{userId}/addresses
        /// Retrieve the address book for a specific user.
        /// </summary>
        [HttpGet("{userId:int}/addresses")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(typeof(object), 404)]
        public async Task<IActionResult> GetUserAddresses([FromRoute] int userId)
        {
            try
            {
                var addresses = await _orderService.GetUserAddressesAsync(userId);
                return Ok(new { success = true, message = "Addresses retrieved successfully", data = addresses });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An unexpected error occurred: " + ex.Message });
            }
        }
    }
}
