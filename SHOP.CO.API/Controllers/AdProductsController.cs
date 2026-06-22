using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SHOP.CO.Application.DTOs;
using SHOP.CO.Application.Services;

namespace SHOP.CO.API.Controllers
{

    [Route("api/admin/products")]
    [ApiController]
    [Authorize(Roles = "Admin,Staff")]
    public class AdminProductsController : ControllerBase
    {
        private readonly IProductAdminService _service;

        public AdminProductsController(IProductAdminService service)
        {
            _service = service;
        }

        // 2. API LẤY CHI TIẾT SẢN PHẨM (Sửa Route tuyệt đối)
        [HttpGet("/api/admin/products/{id}")] // 🟢 FIX LỖI 404 TẠI ĐÂY
        public async Task<IActionResult> GetProductById(int id)
        {
            var result = await _service.GetProductByIdAsync(id);
            return StatusCode(result.Code, result);
        }

        // 3. API THÊM MỚI (Sửa Route tuyệt đối)
        [HttpPost("/api/admin/products")] // 🟢 FIX LỖI 404 TẠI ĐÂY
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductRequestDto request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _service.CreateProductAsync(request);
            return StatusCode(result.Code, result);
        }

        // 4. API CẬP NHẬT (Sửa Route tuyệt đối)
        [HttpPut("/api/admin/products/{id}")] // 🟢 FIX LỖI 404 TẠI ĐÂY
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] UpdateProductRequestDto request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _service.UpdateProductAsync(id, request);
            return StatusCode(result.Code, result);
        }

        // 5. API XÓA (Sửa Route tuyệt đối)
        [HttpDelete("/api/admin/products/{id}")] // 🟢 FIX LỖI 404 TẠI ĐÂY
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var result = await _service.DeleteProductAsync(id);
            return StatusCode(result.Code, result);
        }

        // 🟢 THÊM API LẤY CẤU HÌNH FORM Ở ĐÂY 🟢
        [HttpGet("/api/admin/products/attributes")]
        public async Task<IActionResult> GetFormAttributes()
        {
            var result = await _service.GetFormAttributesAsync();
            return StatusCode(result.Code, result);
        }

        //[HttpPost("upload-images")]
        //public async Task<IActionResult> UploadImage(List<IFormFile> images)
        //{
        //    if (images == null || images.Count() == 0)
        //    {
        //        return BadRequest(new {IsSuccess = false, data = "Không có file nào được chọn!"});
        //    }
        //    var imageUrl = new List<String>();

        //    string uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "product");
        //    if (Directory.Exists(uploadFolder))
        //    {

        //    }
        //    return Ok(new IsSucess = true, data = data)
        //}
    }
    }
