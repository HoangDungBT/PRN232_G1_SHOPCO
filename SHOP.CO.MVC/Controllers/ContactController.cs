using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SHOP.CO.MVC.Models;
using System.Text;

namespace SHOP.CO.MVC.Controllers
{
    public class ContactController : Controller
    {
        private readonly string apiBaseUrl = "https://localhost:7196/api";

        public IActionResult Index() => View();

        [HttpPost]
        public async Task<IActionResult> Send(ContactViewModel model)
        {
            using (HttpClient client = new HttpClient())
            {
                var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                var response = await client.PostAsync($"{apiBaseUrl}/contact", content);
                if (response.IsSuccessStatusCode) TempData["Success"] = "Gửi liên hệ thành công!";
                else TempData["Error"] = "Gửi liên hệ thất bại!";
            }
            return RedirectToAction("Index");
        }
    }
}