using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SHOP.CO.MVC.Models;
using System.Text;

namespace SHOP.CO.MVC.Controllers
{
    public class ContactController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;

        public ContactController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public IActionResult Index() => View();

        [HttpPost]
        public async Task<IActionResult> Send(ContactViewModel model)
        {
            try
            {
                using var client = _clientFactory.CreateClient("ShopCoApi");
                var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                var response = await client.PostAsync("api/contact", content);
                if (response.IsSuccessStatusCode)
                    TempData["Success"] = "Gửi liên hệ thành công!";
                else
                    TempData["Error"] = "Gửi liên hệ thất bại!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi kết nối: " + ex.Message;
            }

            return RedirectToAction("Index");
        }
    }
}