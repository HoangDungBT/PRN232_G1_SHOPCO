using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SHOP.CO.MVC.Common;
using SHOP.CO.MVC.Models;

namespace SHOP.CO.MVC.Controllers
{
    public class NotificationController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;

        public NotificationController(IHttpClientFactory clientFactory)
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

        public async Task<IActionResult> Index()
        {
            var token = HttpContext.Session.GetString(MvcConstants.SessionToken);
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Auth");

            List<NotificationViewModel> list = new();
            try
            {
                using var client = CreateAuthClient();
                var response = await client.GetAsync("api/notifications");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    list = JsonConvert.DeserializeObject<List<NotificationViewModel>>(json) ?? new();
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Không thể tải thông báo: " + ex.Message;
            }

            return View(list);
        }

        [HttpPost]
        public async Task<IActionResult> MarkRead(int id)
        {
            try
            {
                using var client = CreateAuthClient();
                var response = await client.PutAsync($"api/notifications/{id}/read", new StringContent(""));
                if (!response.IsSuccessStatusCode)
                {
                    TempData["Error"] = "Lỗi khi đánh dấu đã đọc: " + await response.Content.ReadAsStringAsync();
                }
            }
            catch (Exception ex) 
            { 
                TempData["Error"] = "Ngoại lệ: " + ex.Message;
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> UnreadCount()
        {
            var token = HttpContext.Session.GetString(MvcConstants.SessionToken);
            if (string.IsNullOrEmpty(token)) return Json(new { count = 0 });

            try
            {
                using var client = CreateAuthClient();
                var response = await client.GetAsync("api/notifications");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var list = JsonConvert.DeserializeObject<List<NotificationViewModel>>(json) ?? new();
                    var unread = list.Count(n => !n.IsRead);
                    return Json(new { count = unread });
                }
            }
            catch { }
            return Json(new { count = 0 });
        }
    }
}