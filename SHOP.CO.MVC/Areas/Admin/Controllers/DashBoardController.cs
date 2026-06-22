using Microsoft.AspNetCore.Mvc;
using SHOP.CO.MVC.Areas.Admin.Models;
using System.Threading.Tasks;

namespace SHOP.CO.MVC.Areas.Admin.Controllers
{
    public class DashboardController : BaseAdminController
    {
        public DashboardController(IHttpClientFactory factory) : base(factory)
        {
        }

        [HttpGet("/Admin")] // 🟢 Hỗ trợ vào thẳng bằng domain.com/Admin
        [HttpGet("")]       // 🟢 Hỗ trợ domain.com/Admin/Dashboard
        [HttpGet("Index")]  // 🟢 Hỗ trợ domain.com/Admin/Dashboard/Index
        public async Task<IActionResult> Index()
        {
            // Gọi API lấy dữ liệu thống kê
            var result = await GetApiAsync<DashboardVM>("api/admin/dashboard/summary");

            if (result != null && result.IsSuccess && result.Data != null)
            {
                return View(result.Data);
            }

            // Nếu lỗi (mất mạng, DB sập...), truyền model rỗng kèm thông báo lỗi
            ViewBag.ErrorMessage = result?.Message ?? "Không thể lấy dữ liệu thống kê.";
            return View(new DashboardVM());
        }
    }
}
