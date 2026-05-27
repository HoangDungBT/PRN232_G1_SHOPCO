using Microsoft.AspNetCore.Mvc;
using SHOP.CO.Application.Services;

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
        public IActionResult GetProducts()
        {
            var products = new[]
            {
        new
        {
            Id = 1,
            Name = "T-Shirt",
            Price = 29,
            Image = "/images/p1.jpg",
            Description = "Premium cotton t-shirt.",
            Category = "T-Shirts"
        },

        new
        {
            Id = 2,
            Name = "Jeans",
            Price = 59,
            Image = "/images/p2.jpg",
            Description = "Modern slim fit jeans.",
            Category = "Jeans"
        },

        new
        {
            Id = 3,
            Name = "Hoodie",
            Price = 99,
            Image = "/images/p3.jpg",
            Description = "Warm fashion hoodie.",
            Category = "Hoodies"
        },

        new
        {
            Id = 4,
            Name = "Jacket",
            Price = 120,
            Image = "/images/p4.jpg",
            Description = "Luxury winter jacket.",
            Category = "Jackets"
        }
    };

            return Ok(products);
        }
    

    [HttpGet("{id}")]
        public IActionResult GetProductById(int id)
        {
            var products = new[]
            {
        new
        {
            Id = 1,
            Name = "T-Shirt",
            Price = 29,
            Image = "/images/p1.jpg",
            Description = "Premium cotton t-shirt.",
            Category = "T-Shirts"
        },

        new
        {
            Id = 2,
            Name = "Jeans",
            Price = 59,
            Image = "/images/p2.jpg",
            Description = "Modern slim fit jeans.",
            Category = "Jeans"
        },

        new
        {
            Id = 3,
            Name = "Hoodie",
            Price = 99,
            Image = "/images/p3.jpg",
            Description = "Warm fashion hoodie.",
            Category = "Hoodies"
        },

        new
        {
            Id = 4,
            Name = "Jacket",
            Price = 120,
            Image = "/images/p4.jpg",
            Description = "Luxury winter jacket.",
            Category = "Jackets"
        }
    };

            var product = products.FirstOrDefault(x => x.Id == id);

            return Ok(product);
        }
    }
}