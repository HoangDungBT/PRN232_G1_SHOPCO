using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.Authorization;
using SHOP.CO.Application.Services;
using SHOP.CO.Application.DTOs;

namespace SHOP.CO.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IProductUiService _productUiService;

        public ProductsController(IProductService productService, IProductUiService productUiService)
        {
            _productService = productService;
            _productUiService = productUiService;
        }

        // OData API hỗ trợ dynamic query (lọc, sắp xếp, tìm kiếm nâng cao)
        [HttpGet("/odata/Products")]
        [EnableQuery]
        public IActionResult GetODataProducts()
        {
            var query = _productService.GetProductsQuery();
            return Ok(query);
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
            var products = await _productUiService.GetUiProductsAsync();
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
            return Ok(product);
        }

        [HttpGet("{id}/related")]
        public async Task<IActionResult> GetRelatedProducts(int id, [FromQuery] int limit = 4)
        {
            var relatedProducts = await _productService.GetRelatedProductsAsync(id, limit);
            return Ok(relatedProducts);
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
                await _productService.AddReviewAsync(id, userId, request);
                return Ok(new { message = "Đánh giá thành công!" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception)
            {
                // Thực hiện ghi log lỗi tại đây nếu có Logger
                return BadRequest(new { message = "Có lỗi xảy ra trong quá trình gửi đánh giá. Vui lòng thử lại sau." });
            }
        }
    }
}