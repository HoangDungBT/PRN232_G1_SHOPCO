using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SHOP.CO.MVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Staff")]
    public class NotificationsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
