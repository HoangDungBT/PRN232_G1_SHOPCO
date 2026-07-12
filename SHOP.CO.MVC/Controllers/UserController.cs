using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SHOP.CO.MVC.Models;
using System.Text;

namespace SHOP.CO.MVC.Controllers
{
    public class UserController : Controller
    {
        private readonly string apiBaseUrl = "https://localhost:7196/api"; // Đổi port đúng với API của bạn
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserController(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        // Helper lấy Token từ Cookie
        private string GetToken()
        {
            return _httpContextAccessor.HttpContext?.Request.Cookies["AuthToken"];
        }

        // === 1. Profile ===
        public async Task<IActionResult> Profile()
        {
            ProfileViewModel model = null;
            using (HttpClient client = new HttpClient())
            {
                var token = GetToken();
                if (!string.IsNullOrEmpty(token))
                {
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                }

                var response = await client.GetAsync($"{apiBaseUrl}/users/profile");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    model = JsonConvert.DeserializeObject<ProfileViewModel>(json);
                }
                else
                {
                    return RedirectToAction("Login", "Account");
                }
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfile(ProfileViewModel model)
        {
            using (HttpClient client = new HttpClient())
            {
                var token = GetToken();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

                var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                var response = await client.PutAsync($"{apiBaseUrl}/users/profile", content);

                if (response.IsSuccessStatusCode) TempData["Success"] = "Cập nhật thành công!";
                else TempData["Error"] = "Có lỗi xảy ra!";
            }
            return RedirectToAction("Profile");
        }

        // === 2. Change Password ===
        public IActionResult ChangePassword() => View();

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (model.NewPassword != model.ConfirmPassword)
            {
                ModelState.AddModelError("ConfirmPassword", "Mật khẩu xác nhận không khớp.");
                return View(model);
            }

            using (HttpClient client = new HttpClient())
            {
                var token = GetToken();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

                var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                var response = await client.PostAsync($"{apiBaseUrl}/users/change-password", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Đổi mật khẩu thành công!";
                    return RedirectToAction("Profile");
                }
                else
                {
                    ModelState.AddModelError("CurrentPassword", "Mật khẩu hiện tại không đúng.");
                }
            }
            return View(model);
        }

        // === 3. Address Book ===
        public async Task<IActionResult> AddressBook()
        {
            List<AddressViewModel> addresses = new();
            using (HttpClient client = new HttpClient())
            {
                var token = GetToken();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

                var response = await client.GetAsync($"{apiBaseUrl}/users/addresses");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    addresses = JsonConvert.DeserializeObject<List<AddressViewModel>>(json);
                }
            }
            return View(addresses);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAddress(int id)
        {
            using (HttpClient client = new HttpClient())
            {
                var token = GetToken();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

                await client.DeleteAsync($"{apiBaseUrl}/users/addresses/{id}");
            }
            return RedirectToAction("AddressBook");
        }

        // === 4. Delete Account ===
        [HttpPost]
        public async Task<IActionResult> DeleteAccount()
        {
            using (HttpClient client = new HttpClient())
            {
                var token = GetToken();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

                var response = await client.DeleteAsync($"{apiBaseUrl}/users/delete-account");
                if (response.IsSuccessStatusCode)
                {
                    Response.Cookies.Delete("AuthToken");
                    return RedirectToAction("Index", "Home");
                }
            }
            TempData["Error"] = "Xóa tài khoản thất bại.";
            return RedirectToAction("Profile");
        }

        // === 5. Subscribe Newsletter ===
        [HttpPost]
        public async Task<IActionResult> SubscribeNewsletter(bool subscribe)
        {
            using (HttpClient client = new HttpClient())
            {
                var token = GetToken();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                await client.PutAsync($"{apiBaseUrl}/users/newsletter?subscribe={subscribe}", null);
            }
            return RedirectToAction("Profile");
        }
    }
}