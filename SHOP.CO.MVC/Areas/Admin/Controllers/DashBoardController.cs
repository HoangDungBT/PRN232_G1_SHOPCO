using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SHOP.CO.MVC.Areas.Admin.Models;
using SHOP.CO.MVC.Common;
using System.Threading.Tasks;

namespace SHOP.CO.MVC.Areas.Admin.Controllers
{
    public class DashboardController : BaseAdminController
    {
        private readonly IConfiguration _configuration;
        public DashboardController(IHttpClientFactory factory, IConfiguration configuration) : base(factory)
        {
            _configuration = configuration;
        }

        [HttpGet("/Admin")]
        [HttpGet("")]      
        [HttpGet("Index")] 
        public async Task<IActionResult> Index()
        {
            // Nếu lỗi (mất mạng, DB sập...), truyền model rỗng kèm thông báo lỗi
            ViewBag.ApiBaseUrl = _configuration.GetSection("ApiSettings:BaseUrl").Value;
            ViewBag.Token = HttpContext.Session.GetString(MvcConstants.SessionToken);// 2. Gọi API lấy dữ liệu thống kê
            var result = await GetApiAsync<DashboardVM>("api/admin/dashboard/summary");

            if (result != null && result.IsSuccess && result.Data != null)
            {
                return View(result.Data);
            }

            // 3. Nếu lỗi (mất mạng, DB sập...), truyền model rỗng kèm thông báo lỗi
            ViewBag.ErrorMessage = result?.Message ?? "Không thể lấy dữ liệu thống kê.";
            return View(new DashboardVM());
        }
    }
}
