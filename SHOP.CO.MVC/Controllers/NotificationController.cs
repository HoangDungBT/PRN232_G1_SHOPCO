using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SHOP.CO.MVC.Models;

namespace SHOP.CO.MVC.Controllers
{
    public class NotificationController : Controller
    {
        private readonly string apiBaseUrl = "https://localhost:7196/api";
        private readonly IHttpContextAccessor _httpContextAccessor;

        public NotificationController(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private string GetToken() => _httpContextAccessor.HttpContext?.Request.Cookies["AuthToken"];

        public async Task<IActionResult> Index()
        {
            List<NotificationViewModel> list = new();
            using (HttpClient client = new HttpClient())
            {
                var token = GetToken();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

                var response = await client.GetAsync($"{apiBaseUrl}/notifications");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    list = JsonConvert.DeserializeObject<List<NotificationViewModel>>(json);
                }
            }
            return View(list);
        }

        [HttpPost]
        public async Task<IActionResult> MarkRead(int id)
        {
            using (HttpClient client = new HttpClient())
            {
                var token = GetToken();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                await client.PutAsync($"{apiBaseUrl}/notifications/{id}/read", null);
            }
            return RedirectToAction("Index");
        }
    }
}