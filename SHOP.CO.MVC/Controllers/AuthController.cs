using Microsoft.AspNetCore.Mvc;
using SHOP.CO.MVC.Common;
using SHOP.CO.MVC.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;

namespace SHOP.CO.MVC.Controllers
{
    public class AuthController : ABaseController
    {
        public AuthController(IHttpClientFactory factory) : base(factory)
        {
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View(new LoginVM());
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM model)
        {
            var result = await PostApiAsync<TokenResponse>("api/auth/login", model);

            if (result != null && result.IsSuccess && result.Data != null) {

                HttpContext.Session.SetString(MvcConstants.SessionToken, result.Data.AccessToken);
                // luuw jwttoken vào session 
                var hander = new JwtSecurityTokenHandler();
                var jwtToken = hander.ReadJwtToken(result.Data.AccessToken);

                var fullName = jwtToken.Claims.FirstOrDefault(c => c.Type == "name" || c.Type == System.Security.Claims.ClaimTypes.Name)?.Value ?? "User";
                var role = jwtToken.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Role)?.Value ?? "Customer";

                HttpContext.Session.SetString(MvcConstants.SessionFullName, fullName);
                HttpContext.Session.SetString(MvcConstants.SessionRole, role);

                
                // Check Role
                if (role == "Admin" || role == "Staff")
                {
                    return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
                }

                return RedirectToAction("Index", "Home");
            }
            ViewBag.ErrorMessage = result?.Message ?? "Đăng nhập thất bại.";
            return View(model);
        }

        // --- ĐĂNG KÝ ---
        [HttpGet]
        public IActionResult Register() => View(new RegisterVM());

        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM model)
        {
            if (!ModelState.IsValid) return View(model);

            // POST đến API (Kiểu trả về là chuỗi string Message)
            var result = await PostApiAsync<string>("api/auth/register", model);

            if (result != null && result.IsSuccess)
            {
                TempData["SuccessMessage"] = "Đăng ký thành công! Vui lòng đăng nhập.";
                return RedirectToAction("Login");
            }

            ViewBag.ErrorMessage = result?.Message ?? "Đăng ký thất bại.";
            return View(model);
        }

        // --- ĐĂNG XUẤT ---
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            // 1. Lấy Token để gửi lên API (Nếu dùng Bearer Token chuẩn)
            var token = HttpContext.Session.GetString(MvcConstants.SessionToken);

            if (!string.IsNullOrEmpty(token))
            {
                // Gắn token vào Header để API cho phép đi qua cổng [Authorize]
                _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                // 2. Gọi API để xóa RefreshToken dưới DB
                await PostApiAsync<string>("api/auth/logout", new { });
            }

            // 3. Xóa Session ở Trình duyệt
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Auth");
        }
    }
}
