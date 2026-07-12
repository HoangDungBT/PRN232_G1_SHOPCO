using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SHOP.CO.Application.DTOs;
using SHOP.CO.Application.Services;
using System.Security.Claims;

namespace SHOP.CO.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        private int GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out var userId) ? userId : 0;
        }

        // 1. [HttpGet("history")] -> Trả về danh sách OrderHistoryDto
        [HttpGet("history")]
        public async Task<ActionResult<List<OrderHistoryDto>>> GetOrderHistory()
        {
            var userId = GetUserId();
            var orders = await _orderService.GetOrderHistoryAsync(userId);
            return Ok(orders);
        }

        // 2. [HttpGet("{id}")] -> Trả về chi tiết đơn hàng theo ID
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderHistoryDto>> GetOrderDetail(int id)
        {
            var userId = GetUserId();
            var order = await _orderService.GetOrderDetailAsync(userId, id);
            if (order == null) return NotFound("Không tìm thấy đơn hàng hoặc bạn không có quyền truy cập.");
            return Ok(order);
        }
    }
}