using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SHOP.CO.MVC.Models;

namespace SHOP.CO.MVC.Controllers
{
    public class OrderController : Controller
    {
        private readonly string apiBaseUrl = "https://localhost:7196/api"; // Đổi port
        private readonly IHttpContextAccessor _httpContextAccessor;

        public OrderController(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private string GetToken() => _httpContextAccessor.HttpContext?.Request.Cookies["AuthToken"];

        public async Task<IActionResult> History()
        {
            List<OrderHistoryViewModel> orders = new();
            using (HttpClient client = new HttpClient())
            {
                var token = GetToken();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

                var response = await client.GetAsync($"{apiBaseUrl}/orders/history");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    orders = JsonConvert.DeserializeObject<List<OrderHistoryViewModel>>(json);
                }
            }
            return View(orders);
        }

        public async Task<IActionResult> Detail(int id)
        {
            OrderHistoryViewModel order = null;
            using (HttpClient client = new HttpClient())
            {
                var token = GetToken();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

                var response = await client.GetAsync($"{apiBaseUrl}/orders/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    order = JsonConvert.DeserializeObject<OrderHistoryViewModel>(json);
                }
            }
            if (order == null) return NotFound();
            return View(order);
        }
    }
}