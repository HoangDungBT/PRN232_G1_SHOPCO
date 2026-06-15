using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using SHOP.CO.Application.Services;
using SHOP.CO.Application.DTOs;

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
        public IActionResult GetProductsMock()
        {
            var products = new[]
            {
        new
        {
            Id = 1,
            Name = "T-Shirt",
            Price = 29,
            Image = "/images/newarrivalimg1.png",
            Description = "Premium cotton t-shirt.",
            Category = "T-Shirts"
        },

        new
        {
            Id = 2,
            Name = "Jeans",
            Price = 59,
            Image = "/images/newarrivalimg2.png",
            Description = "Modern slim fit jeans.",
            Category = "Jeans"
        },

        new
        {
            Id = 3,
            Name = "Hoodie",
            Price = 99,
            Image = "/images/newarrivalimg3.png",
            Description = "Warm fashion hoodie.",
            Category = "Hoodies"
        },

        new
        {
            Id = 4,
            Name = "Jacket",
            Price = 120,
            Image = "/images/newarrivalimg4.png",
            Description = "Luxury winter jacket.",
            Category = "Jackets"
        }
    };

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
    }
}