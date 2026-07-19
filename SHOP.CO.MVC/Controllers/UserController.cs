using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SHOP.CO.MVC.Models;
using SHOP.CO.MVC.Common;
using System.Text;

namespace SHOP.CO.MVC.Controllers
{
    public class UserController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IConfiguration _configuration;

        public UserController(IHttpClientFactory clientFactory, IConfiguration configuration)
        {
            _clientFactory = clientFactory;
            _configuration = configuration;
        }

        // Helper: lấy JWT token từ Session (nhất quán với cả hệ thống)
        private string? GetToken()
        {
            return HttpContext.Session.GetString(MvcConstants.SessionToken);
        }

        // Helper: tạo HttpClient đã gắn Auth header
        private HttpClient CreateAuthClient()
        {
            var client = _clientFactory.CreateClient("ShopCoApi");
            var token = GetToken();
            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
            return client;
        }

        // === 1. Profile ===
        public async Task<IActionResult> Profile()
        {
            var token = GetToken();
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Auth");

            ProfileViewModel? model = null;
            try
            {
                using var client = CreateAuthClient();
                var response = await client.GetAsync("api/users/profile");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    model = JsonConvert.DeserializeObject<ProfileViewModel>(json);
                }
                else
                {
                    TempData["Error"] = "Không thể tải thông tin hồ sơ.";
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi kết nối: " + ex.Message;
                return RedirectToAction("Index", "Home");
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfile(ProfileViewModel model)
        {
            var token = GetToken();
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Auth");

            try
            {
                using var client = CreateAuthClient();
                var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                var response = await client.PutAsync("api/users/profile", content);

                if (response.IsSuccessStatusCode)
                    TempData["Success"] = "Cập nhật hồ sơ thành công!";
                else
                    TempData["Error"] = "Có lỗi khi cập nhật. Vui lòng thử lại.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi kết nối: " + ex.Message;
            }

            return RedirectToAction("Profile");
        }

        // === 2. Change Password ===
        [HttpGet]
        public IActionResult ChangePassword()
        {
            if (string.IsNullOrEmpty(GetToken()))
                return RedirectToAction("Login", "Auth");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            var token = GetToken();
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Auth");

            if (model.NewPassword != model.ConfirmPassword)
            {
                ModelState.AddModelError("ConfirmPassword", "Mật khẩu xác nhận không khớp.");
                return View(model);
            }

            try
            {
                using var client = CreateAuthClient();
                var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                var response = await client.PostAsync("api/users/change-password", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Đổi mật khẩu thành công!";
                    return RedirectToAction("Profile");
                }
                else
                {
                    var errorJson = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError("CurrentPassword", "Mật khẩu hiện tại không đúng.");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Lỗi kết nối: " + ex.Message);
            }

            return View(model);
        }

        // === 3. Address Book ===
        public async Task<IActionResult> AddressBook()
        {
            var token = GetToken();
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Auth");

            List<AddressViewModel> addresses = new();
            try
            {
                using var client = CreateAuthClient();
                var response = await client.GetAsync("api/users/addresses");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    addresses = JsonConvert.DeserializeObject<List<AddressViewModel>>(json) ?? new();
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi kết nối: " + ex.Message;
            }

            return View(addresses);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAddress(int id)
        {
            var token = GetToken();
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Auth");

            try
            {
                using var client = CreateAuthClient();
                await client.DeleteAsync($"api/users/addresses/{id}");
            }
            catch { /* ignore */ }

            return RedirectToAction("AddressBook");
        }

        [HttpPost]
        public async Task<IActionResult> AddAddress(AddressViewModel model)
        {
            var token = GetToken();
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Auth");

            try
            {
                using var client = CreateAuthClient();
                var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                var response = await client.PostAsync("api/users/addresses", content);
                if (response.IsSuccessStatusCode)
                    TempData["Success"] = "Thêm địa chỉ thành công!";
                else
                    TempData["Error"] = "Thêm địa chỉ thất bại.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi kết nối: " + ex.Message;
            }
            return RedirectToAction("AddressBook");
        }

        [HttpPost]
        public async Task<IActionResult> EditAddress(AddressViewModel model)
        {
            var token = GetToken();
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Auth");

            try
            {
                using var client = CreateAuthClient();
                var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                var response = await client.PutAsync($"api/users/addresses/{model.AddressId}", content);
                if (response.IsSuccessStatusCode)
                    TempData["Success"] = "Cập nhật địa chỉ thành công!";
                else
                    TempData["Error"] = "Cập nhật địa chỉ thất bại.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi kết nối: " + ex.Message;
            }
            return RedirectToAction("AddressBook");
        }

        [HttpPost]
        public async Task<IActionResult> SetDefaultAddress(int id)
        {
            var token = GetToken();
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Auth");

            try
            {
                using var client = CreateAuthClient();
                var content = new StringContent("{}", Encoding.UTF8, "application/json");
                var response = await client.PatchAsync($"api/users/addresses/{id}/default", content);
                if (response.IsSuccessStatusCode)
                    TempData["Success"] = "Đã đặt làm mặc định!";
                else
                    TempData["Error"] = "Thao tác thất bại.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi kết nối: " + ex.Message;
            }
            return RedirectToAction("AddressBook");
        }

        // === 4. Delete Account ===
        [HttpPost]
        public async Task<IActionResult> DeleteAccount()
        {
            var token = GetToken();
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Auth");

            try
            {
                using var client = CreateAuthClient();
                var response = await client.DeleteAsync("api/users/delete-account");
                if (response.IsSuccessStatusCode)
                {
                    HttpContext.Session.Clear();
                    return RedirectToAction("Index", "Home");
                }
            }
            catch { /* ignore */ }

            TempData["Error"] = "Xóa tài khoản thất bại.";
            return RedirectToAction("Profile");
        }

        // === 5. Subscribe Newsletter ===
        [HttpPost]
        public async Task<IActionResult> SubscribeNewsletter(bool subscribe)
        {
            var token = GetToken();
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Auth");

            try
            {
                using var client = CreateAuthClient();
                await client.PutAsync($"api/users/newsletter?subscribe={subscribe}", null);
            }
            catch { /* ignore */ }

            return RedirectToAction("Profile");
        }
    }
}