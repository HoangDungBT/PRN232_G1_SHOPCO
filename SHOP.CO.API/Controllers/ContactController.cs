using Microsoft.AspNetCore.Mvc;
using SHOP.CO.Application.DTOs;
using SHOP.CO.Application.Services;

namespace SHOP.CO.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContactController : ControllerBase
    {
        private readonly IContactService _contactService;

        public ContactController(IContactService contactService)
        {
            _contactService = contactService;
        }

        // [HttpPost] -> Nhận form liên hệ (Gửi vào InteractionLogs)
        [HttpPost]
        public async Task<IActionResult> SubmitContact([FromBody] ContactDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _contactService.SubmitContactFormAsync(dto);
            if (!result) return StatusCode(500, "Gửi liên hệ thất bại.");

            return Ok("Cảm ơn bạn đã liên hệ, chúng tôi sẽ phản hồi sớm nhất!");
        }
    }
}