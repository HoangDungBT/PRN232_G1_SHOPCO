using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SHOP.CO.Application.DTOs;
using SHOP.CO.Application.Services;

namespace SHOP.CO.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IInvoiceService _invoiceService;

        public OrdersController(IOrderService orderService, IInvoiceService invoiceService)
        {
            _orderService = orderService;
            _invoiceService = invoiceService;
        }

        [HttpPost("checkout")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(typeof(object), 400)]
        public async Task<IActionResult> Checkout([FromBody] CheckoutRequestDto requestDto)
        {
            try
            {
                var result = await _orderService.CheckoutAsync(requestDto);
                return Ok(new
                {
                    success = true,
                    data = new
                    {
                        orderId = result.OrderId,
                        orderCode = result.OrderCode,
                        orderStatus = result.OrderStatus,
                        totalAmount = result.TotalAmount,
                        createdAt = result.CreatedAt
                    }
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An unexpected error occurred: " + ex.Message });
            }
        }

        [HttpGet("user/{userId:int}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(typeof(object), 400)]
        public async Task<IActionResult> GetOrdersByUser([FromRoute] int userId)
        {
            try
            {
                var orders = await _orderService.GetOrdersByUserIdAsync(userId);
                return Ok(new { success = true, message = "Orders retrieved successfully", data = orders });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An unexpected error occurred: " + ex.Message });
            }
        }

        [HttpGet("{orderId:int}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(typeof(object), 404)]
        public async Task<IActionResult> GetOrderDetails([FromRoute] int orderId)
        {
            try
            {
                var order = await _orderService.GetOrderByIdAsync(orderId);
                if (order == null)
                {
                    return NotFound(new { success = false, message = "Order not found" });
                }
                return Ok(new { success = true, message = "Order details retrieved successfully", data = order });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An unexpected error occurred: " + ex.Message });
            }
        }

        [HttpPost("{orderId:int}/cancel")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(typeof(object), 400)]
        [ProducesResponseType(typeof(object), 403)]
        [ProducesResponseType(typeof(object), 404)]
        public async Task<IActionResult> CancelOrder([FromRoute] int orderId, [FromQuery] int userId, [FromBody] CancelOrderRequestDto requestDto)
        {
            try
            {
                if (requestDto == null || string.IsNullOrWhiteSpace(requestDto.CancelReason))
                {
                    return BadRequest(new { success = false, message = "Cancel reason is required." });
                }

                var cancelReason = requestDto.CancelReason;
                var result = await _orderService.CancelOrderAsync(orderId, userId, cancelReason);
                return Ok(new { success = true, message = "Order canceled successfully", data = result });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new { success = false, message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An unexpected error occurred: " + ex.Message });
            }
        }
        [HttpGet("{orderId:int}/tracking")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(typeof(object), 400)]
        [ProducesResponseType(typeof(object), 404)]
        public async Task<IActionResult> GetOrderTracking([FromRoute] int orderId)
        {
            try
            {
                if (orderId <= 0)
                {
                    return BadRequest(new { success = false, message = "Invalid order ID." });
                }

                var tracking = await _orderService.GetOrderTrackingAsync(orderId);
                if (tracking == null)
                {
                    return NotFound(new { success = false, message = "Order not found." });
                }

                return Ok(new { success = true, message = "Order tracking retrieved successfully.", data = tracking });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An unexpected error occurred: " + ex.Message });
            }
        }
        [HttpPost("{orderId:int}/return-request")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(typeof(object), 400)]
        [ProducesResponseType(typeof(object), 403)]
        [ProducesResponseType(typeof(object), 404)]
        public async Task<IActionResult> SubmitReturnRequest(
            [FromRoute] int orderId,
            [FromQuery] int userId,
            [FromBody] ReturnRequestDto requestDto)
        {
            try
            {
                if (requestDto == null || string.IsNullOrWhiteSpace(requestDto.Reason))
                {
                    return BadRequest(new { success = false, message = "Return reason is required." });
                }

                var result = await _orderService.SubmitReturnRequestAsync(orderId, userId, requestDto.Reason, requestDto.Description ?? string.Empty);
                return Ok(new { success = true, message = "Return request submitted successfully.", data = result });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new { success = false, message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
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

        [HttpGet("{orderId:int}/invoice")]
        public async Task<IActionResult> GetInvoice([FromRoute] int orderId, [FromQuery] int userId)
        {
            try
            {
                var pdfBytes = await _invoiceService.GenerateInvoiceAsync(orderId, userId);
                var order = await _orderService.GetOrderByIdAsync(orderId);
                string orderCode = order?.OrderCode ?? orderId.ToString();
                return File(pdfBytes, "application/pdf", $"Invoice-{orderCode}.pdf");
            }
            catch (System.Collections.Generic.KeyNotFoundException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new { success = false, message = ex.Message });
            }
            catch (InvalidOperationException ex)
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
