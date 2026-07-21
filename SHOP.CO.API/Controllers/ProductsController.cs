using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.Authorization;
using SHOP.CO.Application.Services;
using SHOP.CO.Application.DTOs;
using AutoMapper;

namespace SHOP.CO.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IProductUiService _productUiService;
        private readonly IMapper _mapper;

        public ProductsController(IProductService productService, IProductUiService productUiService, IMapper mapper)
        {
            _productService = productService;
            _productUiService = productUiService;
            _mapper = mapper;
        }

        // OData API hỗ trợ dynamic query (lọc, sắp xếp, tìm kiếm nâng cao)
        [HttpGet("/odata/Products")]
        [EnableQuery]
        public IActionResult GetODataProducts()
        {
            var query = _productService.GetProductsQuery();
            var projected = query.Select(p => new ProductDto
            {
                ProductId = p.ProductId,
                CategoryId = p.CategoryId,
                ProductName = p.ProductName,
                Slug = p.Slug,
                Brand = p.Brand,
                Description = p.Description,
                Material = p.Material,
                GenderTarget = p.GenderTarget,
                BasePrice = p.BasePrice,
                SalePrice = p.SalePrice,
                AverageRating = p.AverageRating,
                ReviewCount = p.ReviewCount,
                ViewCount = p.ViewCount,
                IsFeatured = p.IsFeatured,
                IsBestSeller = p.IsBestSeller,
                IsNewArrival = p.IsNewArrival,
                IsActive = p.IsActive,
                CategoryName = p.Category != null ? p.Category.CategoryName : null,
                ThumbnailUrl = p.ProductImages.FirstOrDefault(i => i.IsThumbnail) != null
                    ? p.ProductImages.FirstOrDefault(i => i.IsThumbnail)!.ImageUrl
                    : p.ProductImages.FirstOrDefault() != null ? p.ProductImages.FirstOrDefault()!.ImageUrl : null,
                HasLowStock = p.ProductVariants.Any(v => v.StockQuantity <= v.LowStockThreshold),
                // ✅ FIX: Map Variants để OData filter Variants/any(v: v/Color eq '...') hoạt động
                Variants = p.ProductVariants.Select(v => new ProductVariantDto
                {
                    VariantId = v.VariantId,
                    ProductId = v.ProductId,
                    Sku = v.Sku ?? "",
                    Size = v.Size,
                    Color = v.Color,
                    ColorHex = v.ColorHex,
                    ExtraPrice = v.ExtraPrice,
                    StockQuantity = v.StockQuantity,
                    LowStockThreshold = v.LowStockThreshold
                }).ToList()

            });
            return Ok(projected);
        }

        // API thật của bạn
        [HttpGet("search")]
        public async Task<IActionResult> SearchProducts(
            [FromQuery] string? keyword,
            [FromQuery] int page = 1,
            [FromQuery] int size = 10)
        {
            var result = await _productService.GetProductsAsync(keyword, page, size);

            return Ok(result);
        }

        // API test giao diện
        [HttpGet]
        public async Task<IActionResult> GetProductsUi()
        {
            var query = _productService.GetProductsQuery();
            var realProducts = query.Where(p => p.IsActive).OrderByDescending(p => p.ProductId).Take(8).ToList();
            var products = realProducts.Select(p => new ProductUiDto
            {
                Id = p.ProductId,
                Name = p.ProductName,
                Price = p.SalePrice ?? p.BasePrice,
                BasePrice = p.BasePrice,
                SalePrice = p.SalePrice,
                Image = p.ProductImages.FirstOrDefault(i => i.IsThumbnail) != null
                    ? p.ProductImages.FirstOrDefault(i => i.IsThumbnail)!.ImageUrl
                    : p.ProductImages.FirstOrDefault() != null ? p.ProductImages.FirstOrDefault()!.ImageUrl : "/images/heroimg.png",
                Description = p.Description ?? "",
                Category = p.Category != null ? p.Category.CategoryName : "Fashion",
                AverageRating = p.AverageRating,
                ReviewCount = p.ReviewCount
            }).ToList();
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound(new { message = $"Product with ID {id} not found." });
            }
            var productDto = _mapper.Map<ProductDto>(product);
            return Ok(productDto);
        }

        [HttpGet("{id}/related")]
        public async Task<IActionResult> GetRelatedProducts(int id, [FromQuery] int limit = 4)
        {
            var relatedProducts = await _productService.GetRelatedProductsAsync(id, 0, limit);
            var dtos = _mapper.Map<List<ProductDto>>(relatedProducts);
            return Ok(dtos);
        }

        [HttpGet("{id}/reviews")]
        public async Task<IActionResult> GetReviews(int id)
        {
            var reviews = await _productService.GetReviewsByProductIdAsync(id);
            return Ok(reviews);
        }

        [Authorize]
        [HttpPost("{id}/reviews")]
        public async Task<IActionResult> AddReview(int id, [FromBody] CreateReviewRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;
                if (!int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized();
                }
                await _productService.AddReviewAsync(userId, id, request.Rating, request.Comment);
                return Ok(new { message = "Đánh giá thành công!" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                // Thực hiện ghi log lỗi tại đây nếu có Logger
                return BadRequest(new { message = "Có lỗi xảy ra trong quá trình gửi đánh giá. Vui lòng thử lại sau." });
            }
        }
    }
}