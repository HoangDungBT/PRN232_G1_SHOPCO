using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SHOP.CO.MVC.Common;
using SHOP.CO.MVC.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;

namespace SHOP.CO.MVC.Controllers
{
    public class AuthController : ABaseController
    {
        private readonly IConfiguration _configuration;

        public AuthController(IHttpClientFactory factory, IConfiguration configuration) : base(factory)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View(new LoginVM());
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginVM model)
        {
            var result = await PostApiAsync<TokenResponse>("api/auth/login", model);

            if (result != null && result.IsSuccess && result.Data != null) {

                HttpContext.Session.SetString(MvcConstants.SessionToken, result.Data.AccessToken);
                // luuw jwttoken vào session 
                var hander = new JwtSecurityTokenHandler();
                var jwtToken = hander.ReadJwtToken(result.Data.AccessToken);

                var fullName = jwtToken.Claims.FirstOrDefault(c => c.Type == "name" || c.Type == System.Security.Claims.ClaimTypes.Name)?.Value ?? "User";
                var role = jwtToken.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Role)?.Value ?? "Customer";
                var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier || c.Type == "sub")?.Value;

                HttpContext.Session.SetString(MvcConstants.SessionFullName, fullName);
                HttpContext.Session.SetString(MvcConstants.SessionRole, role);
                if (!string.IsNullOrEmpty(userIdClaim))
                {
                    HttpContext.Session.SetString(MvcConstants.SessionUserId, userIdClaim);
                }

                // Check Role
                string redirectUrl = "/Home/Index";
                if (role == "Admin" || role == "Staff")
                {
                    redirectUrl = "/Admin/Dashboard/Index";
                }

                return Json(new { isSuccess = true, redirectUrl = redirectUrl });
            }
            
            return Json(new { isSuccess = false, message = result?.Message ?? "Đăng nhập thất bại." });
        }

        // --- ĐĂNG KÝ ---
        [HttpGet]
        public IActionResult Register()
        {
            // 🟢 THÊM DÒNG NÀY ĐỂ TRUYỀN URL API XUỐNG VIEW
            ViewBag.ApiBaseUrl = _configuration.GetSection("ApiSettings:BaseUrl").Value;

            return View(new RegisterVM());
        }

      

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            ViewBag.ApiBaseUrl = _configuration.GetSection("ApiSettings:BaseUrl").Value;
            return View();
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
