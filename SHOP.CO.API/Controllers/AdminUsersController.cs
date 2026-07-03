using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SHOP.CO.Application.DTOs;
using SHOP.CO.Application.Services;
using System.Threading.Tasks;

namespace SHOP.CO.API.Controllers
{
    [Route("api/admin/users")]
    [ApiController]
    [Authorize(Roles = "Admin")] 
    public class AdminUsersController : ControllerBase
    {
        private readonly IUserAdminService _service;

        public AdminUsersController(IUserAdminService service)
        {
            _service = service;
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateUserFieldDto dto)
        {
            var result = await _service.UpdateUserStatusAsync(id, dto.Value);
            return StatusCode(result.Code, result);
        }

        [HttpPut("{id}/role")]
        public async Task<IActionResult> UpdateRole(int id, [FromBody] UpdateUserFieldDto dto)
        {
            var result = await _service.UpdateUserRoleAsync(id, dto.Value);
            return StatusCode(result.Code, result);
        }
    }
}
