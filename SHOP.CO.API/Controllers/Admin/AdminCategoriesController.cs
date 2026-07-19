using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SHOP.CO.Application.DTOs;
using SHOP.CO.Application.Services;
using System.Threading.Tasks;

namespace SHOP.CO.API.Controllers
{
    [Route("api/admin/categories")]
    [ApiController]
    [Authorize(Roles = "Admin,Staff")]
    public class AdminCategoriesController : ControllerBase
    {
        private readonly IAdminCategoryService _service;

        public AdminCategoriesController(IAdminCategoryService service)
        {
            _service = service;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            return StatusCode(result.Code, result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SaveCategoryRequestDto request)
        {
            var result = await _service.CreateAsync(request);
            return StatusCode(result.Code, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] SaveCategoryRequestDto request)
        {
            var result = await _service.UpdateAsync(id, request);
            return StatusCode(result.Code, result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.SoftDeleteAsync(id);
            return StatusCode(result.Code, result);
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var result = await _service.ToggleCategoryStatusAsync(id);
            return StatusCode(result.Code, result);
        }
    }
}
