using Microsoft.AspNetCore.Mvc;
using SHOP.CO.Application.Services;

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
        public async Task<IActionResult> GetProducts()
        {
            var products = await _productUiService.GetUiProductsAsync();
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var product = await _productUiService.GetUiProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }
    }
}