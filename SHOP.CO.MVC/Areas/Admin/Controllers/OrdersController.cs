using Microsoft.AspNetCore.Mvc;

namespace SHOP.CO.MVC.Areas.Admin.Controllers
{
    public class OrdersController : BaseAdminController
    {
        public OrdersController(IHttpClientFactory factory) : base(factory)
        {
        }

        [HttpGet("")]       // 🟢 Map với URL: /Admin/Products
        [HttpGet("Index")]  // 🟢 Map với URL: /Admin/Products/Index
        public IActionResult Index()
        {
            return View();
        }
    }
}
