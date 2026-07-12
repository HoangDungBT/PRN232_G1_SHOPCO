using Microsoft.AspNetCore.Mvc;
using SHOP.CO.Application.Services;
using System.Threading.Tasks;

namespace SHOP.CO.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly IProductService _productService;

        public CategoriesController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _productService.GetActiveCategoriesAsync();
            return Ok(categories);
        }
    }
}
