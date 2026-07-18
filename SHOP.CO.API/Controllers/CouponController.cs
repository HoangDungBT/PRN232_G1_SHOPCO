using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SHOP.CO.Application.DTOs;
using SHOP.CO.Application.Services;

namespace SHOP.CO.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CouponController : ControllerBase
    {
        private readonly ICouponService _couponService;

        public CouponController(ICouponService couponService)
        {
            _couponService = couponService;
        }

        [HttpPost("apply")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(typeof(object), 400)]
        public async Task<IActionResult> ApplyCoupon([FromBody] ApplyCouponRequestDto requestDto)
        {
            if (requestDto == null)
            {
                return BadRequest(new { success = false, message = "Invalid JSON" });
            }

            if (requestDto.UserId <= 0)
            {
                return BadRequest(new { success = false, message = "Negative amount or invalid user" });
            }

            if (string.IsNullOrWhiteSpace(requestDto.CouponCode))
            {
                return BadRequest(new { success = false, message = "Empty code or missing code" });
            }

            try
            {
                var result = await _couponService.ApplyCouponAsync(requestDto.UserId, requestDto.CouponCode);
                if (result.Success)
                {
                    return Ok(new
                    {
                        success = true,
                        message = result.Message,
                        data = new
                        {
                            discountAmount = result.DiscountAmount,
                            finalAmount = result.FinalAmount
                        }
                    });
                }
                else
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = result.Message
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An unexpected error occurred: " + ex.Message });
            }
        }
    }
}
