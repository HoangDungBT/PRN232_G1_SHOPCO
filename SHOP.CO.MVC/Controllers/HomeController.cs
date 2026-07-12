using Microsoft.AspNetCore.Mvc;
using SHOP.CO.MVC.Models;
using SHOP.CO.MVC.Services;

namespace SHOP.CO.MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProductApiClient _productApiClient;

        public HomeController(IProductApiClient productApiClient)
        {
            _productApiClient = productApiClient;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var products = await _productApiClient.GetProductsAsync();
                return View(products);
            }
            catch (Exception)
            {
                return View(new List<ProductVM>());
            }
        }

        // CATEGORY PAGE
        public async Task<IActionResult> Category()
        {
            try
            {
                var products = await _productApiClient.GetProductsAsync();
                return View(products);
            }
            catch (Exception)
            {
                return View(new List<ProductVM>());
            }
        }

        // PRODUCT DETAIL
        public async Task<IActionResult> Detail(int id)
        {
            try
            {
                var product = await _productApiClient.GetProductByIdAsync(id);
                return View("Details", product);
            }
            catch (Exception)
            {
                return View("Details", new ProductVM());
            }
        }

        // SHOPPING CART
        public IActionResult Cart()
        {
            return View();
        }

        public IActionResult AboutUs()
        {
            return View();
        }

        public IActionResult Faq()
        {
            return View();
        }
    }
}