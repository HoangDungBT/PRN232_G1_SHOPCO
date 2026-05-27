using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SHOP.CO.MVC.Models;

namespace SHOP.CO.MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly string apiUrl = "https://localhost:7196/api/products";

        public async Task<IActionResult> Index()
        {
            List<ProductVM> products = new();

            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetAsync(apiUrl);

                var json = await response.Content.ReadAsStringAsync();

                products = JsonConvert.DeserializeObject<List<ProductVM>>(json);
            }

            return View(products);
        }

        // CATEGORY PAGE
        public async Task<IActionResult> Category()
        {
            List<ProductVM> products = new();

            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetAsync(apiUrl);

                var json = await response.Content.ReadAsStringAsync();

                products = JsonConvert.DeserializeObject<List<ProductVM>>(json);
            }

            return View(products);
        }

        // PRODUCT DETAIL
        public async Task<IActionResult> Detail(int id)
        {
            ProductVM product = new();

            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetAsync($"{apiUrl}/{id}");

                var json = await response.Content.ReadAsStringAsync();

                product = JsonConvert.DeserializeObject<ProductVM>(json);
            }

            return View(product);
        }

        // SHOPPING CART
        public IActionResult Cart()
        {
            return View();
        }
    }
}