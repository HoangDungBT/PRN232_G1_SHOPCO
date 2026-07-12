using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SHOP.CO.Application.DTOs;
using SHOP.CO.Application.Services;
using SHOP.CO.Infrastructure.Persistence;


namespace SHOP.CO.API.Controllers
{
    [Route("api/admin/products")]
    [ApiController]
    [Authorize(Roles = "Admin,Staff")]
    public class AdminProductsController : ControllerBase
    {
        private readonly IAdminProductService _service;
        private readonly IWebHostEnvironment _env; // 🟢 INJECT WEB HOST ENVIRONMENT

        // tạm
        private readonly ShopCoDbContext _context;
        // 🟢 CẬP NHẬT CONSTRUCTOR
        public AdminProductsController(IAdminProductService service, IWebHostEnvironment env,
            ShopCoDbContext context)
        {
            _service = service;
            _env = env;
            _context = context;
        }

        [HttpGet("/api/admin/products/{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var result = await _service.GetProductByIdAsync(id);
            return StatusCode(result.Code, result);
        }

        [HttpPost("/api/admin/products")]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductRequestDto request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _service.CreateProductAsync(request);
            return StatusCode(result.Code, result);
        }

        [HttpPut("/api/admin/products/{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] UpdateProductRequestDto request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _service.UpdateAsync(id, request);
            return StatusCode(result.Code, result);
        }
        // Thêm 2 API này vào trong AdminProductsController để xử lý hàng loạt
        [HttpPut("bulk/status")]
        public async Task<IActionResult> BulkUpdateStatus([FromBody] BulkUpdateStatusDto dto)
        {
            if (dto.ProductIds == null || !dto.ProductIds.Any()) return BadRequest("Không có sản phẩm nào được chọn.");

            // Tạm thời viết logic trực tiếp ở Controller cho nhanh (Nên chuyển vào Service nếu dự án lớn)
            var products = _context.Products.Where(p => dto.ProductIds.Contains(p.ProductId)).ToList();
            foreach (var p in products)
            {
                p.IsActive = dto.IsActive;
                p.UpdatedAt = DateTime.UtcNow;
            }
            await _context.SaveChangesAsync();
            return Ok(new { isSuccess = true, message = $"Đã cập nhật trạng thái {products.Count} sản phẩm!" });
        }

        [HttpPut("bulk/featured")]
        public async Task<IActionResult> BulkUpdateFeatured([FromBody] BulkUpdateFeaturedDto dto)
        {
            if (dto.ProductIds == null || !dto.ProductIds.Any()) return BadRequest("Không có sản phẩm nào được chọn.");

            var products = _context.Products.Where(p => dto.ProductIds.Contains(p.ProductId)).ToList();
            foreach (var p in products)
            {
                p.IsFeatured = dto.IsFeatured;
                p.UpdatedAt = DateTime.UtcNow;
            }
            await _context.SaveChangesAsync();
            return Ok(new { isSuccess = true, message = $"Đã cập nhật nổi bật {products.Count} sản phẩm!" });
        }
        [HttpDelete("/api/admin/products/{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var result = await _service.DeleteProductAsync(id);
            return StatusCode(result.Code, result);
        }

        [HttpGet("/api/admin/products/attributes")]
        public async Task<IActionResult> GetFormAttributes()
        {
            var result = await _service.GetFormAttributesAsync();
            return StatusCode(result.Code, result);
        }

        // 🟢 HÀM UPLOAD ẢNH ĐÃ ĐƯỢC FIX LỖI TỐI ƯU 🟢
        [HttpPost("upload-images")]
        public async Task<IActionResult> UploadImages(List<IFormFile> images)
        {
            if (images == null || images.Count == 0)
                return BadRequest(new { isSuccess = false, message = "Không có file nào được chọn." });

            var imageUrls = new List<string>();

            // Sử dụng WebRootPath để trỏ chính xác tuyệt đối vào wwwroot
            string webRootPath = _env.WebRootPath;
            if (string.IsNullOrWhiteSpace(webRootPath))
            {
                webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            }

            // ĐỒNG NHẤT 1 TÊN DUY NHẤT LÀ "products" (có s)
            string uploadsFolder = Path.Combine(webRootPath, "uploads", "products");

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            foreach (var file in images)
            {
                if (file.Length > 0)
                {
                    // Path.GetFileName giúp xóa bỏ các đường dẫn rác (C:\fakepath\...) nếu trình duyệt gửi lên
                    string fileName = Path.GetFileName(file.FileName);

                    // Tạo tên file ngẫu nhiên để không bị trùng (Guid)
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + fileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }

                    // Thêm URL tương đối vào danh sách trả về
                    imageUrls.Add($"/uploads/products/{uniqueFileName}");
                }
            }

            return Ok(new { isSuccess = true, data = imageUrls });
        }

        [HttpDelete("images/{imageId}")]
        public async Task<IActionResult> DeleteProductImage(int imageId)
        {
            var result = await _service.DeleteProductImageAsync(imageId);
            return StatusCode(result.Code, result);
        }
    }
}