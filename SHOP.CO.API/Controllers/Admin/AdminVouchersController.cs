using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SHOP.CO.Application.DTOs;
using SHOP.CO.Application.Services;

namespace SHOP.CO.API.Controllers
{
    [Route("api/admin/vouchers")]
    [ApiController]
    [Authorize(Roles = "Admin,Staff")]
    public class AdminVouchersController : ControllerBase
    {
        private readonly IAdminVoucherService _service;

        public AdminVouchersController(IAdminVoucherService service)
        {
            _service = service;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetVoucherByIdAsync(id);
            return StatusCode(result.Code, result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SaveVoucherDto request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _service.CreateVoucherAsync(request);
            return StatusCode(result.Code, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] SaveVoucherDto request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _service.UpdateVoucherAsync(id, request);
            return StatusCode(result.Code, result);
        }

        // Tạm ngưng / Kích hoạt lại Voucher
        [HttpPut("{id}/status")]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var result = await _service.ToggleVoucherStatusAsync(id);
            return StatusCode(result.Code, result);
        }
    }
}