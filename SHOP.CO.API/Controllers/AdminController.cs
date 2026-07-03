using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SHOP.CO.Application.DTOs;
using SHOP.CO.Application.Services;


namespace SHOP.CO.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,Staff")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet("dashboard/summary")]
        public async Task<IActionResult> GetDashboardSumary()
        {
            var result = await _adminService.GetDashBoardSummaryAsync();
            return StatusCode(result.Code, result);
        }

        [HttpGet("dashboard/revenue-chart")]
        public async Task<IActionResult> GetRevenueChart([FromQuery] int days = 7)
        {
            // Mặc định lấy 7 ngày, nếu Frontend truyền param days=30 thì lấy 30 ngày
            var result = await _adminService.GetRevenueChartAsync(days);
            return StatusCode(result.Code, result);
        }

        [HttpPost("inventory/adjust")]
        public async Task<IActionResult> AdjustStock([FromBody] StockAdjustmentDto dto)
        {
            // logType có thể truyền từ client hoặc định nghĩa cứng ở đây
            var result = await _adminService.AdjustStockAsync(dto.VariantId, dto.QuantityChange, dto.Reason, "StockMovement");
            return StatusCode(result.Code, result);
        }
    }
}
