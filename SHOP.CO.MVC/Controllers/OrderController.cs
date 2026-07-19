using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SHOP.CO.MVC.Common;
using SHOP.CO.MVC.Models;

namespace SHOP.CO.MVC.Controllers
{
    public class OrderController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;

        public OrderController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        private HttpClient CreateAuthClient()
        {
            var client = _clientFactory.CreateClient("ShopCoApi");
            var token = HttpContext.Session.GetString(MvcConstants.SessionToken);
            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
            return client;
        }

        public async Task<IActionResult> History()
        {
            var token = HttpContext.Session.GetString(MvcConstants.SessionToken);
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Auth");

            List<OrderHistoryViewModel> orders = new();
            try
            {
                using var client = CreateAuthClient();
                var response = await client.GetAsync("api/orders/history");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    orders = JsonConvert.DeserializeObject<List<OrderHistoryViewModel>>(json) ?? new();
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return RedirectToAction("Login", "Auth");
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Không thể tải lịch sử đơn hàng: " + ex.Message;
            }

            return View(orders);
        }

        public async Task<IActionResult> Detail(int id)
        {
            var token = HttpContext.Session.GetString(MvcConstants.SessionToken);
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Auth");

            OrderHistoryViewModel? order = null;
            try
            {
                using var client = CreateAuthClient();
                var response = await client.GetAsync($"api/orders/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    order = JsonConvert.DeserializeObject<OrderHistoryViewModel>(json);
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi kết nối: " + ex.Message;
            }

            if (order == null) return NotFound();
            return View(order);
        }
    }
}