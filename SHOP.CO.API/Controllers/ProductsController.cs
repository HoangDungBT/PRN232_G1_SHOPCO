using Microsoft.AspNetCore.Mvc;
using SHOP.CO.Application.Services;
using System.Threading.Tasks;

namespace SHOP.CO.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        #region 1. Lấy danh sách sản phẩm (Có phân trang & Tìm kiếm)
        /// <summary>
        /// API tìm kiếm sản phẩm theo tên/slug kết hợp phân trang
        /// </summary>
        /// <param name="keyword">Từ khóa tìm kiếm (Tên sản phẩm hoặc Slug)</param>
        /// <param name="page">Số trang hiện tại (Mặc định là trang 1)</param>
        /// <param name="size">Số lượng phần tử trên mỗi trang (Mặc định là 10)</param>
        /// <returns>Trả về đối tượng PagedResult chứa danh sách ProductDto</returns>
        [HttpGet("search")]
        public async Task<IActionResult> SearchProducts(
            [FromQuery] string? keyword,
            [FromQuery] int page = 1,
            [FromQuery] int size = 10)
        {
            // Gọi tầng Service xử lý logic nghiệp vụ và map dữ liệu sang DTO
            var result = await _productService.GetProductsAsync(keyword, page, size);

            // Trả về HTTP Status Code 200 kèm theo cục dữ liệu JSON chuẩn hóa
            return Ok(result);
        }
        #endregion
    }
}