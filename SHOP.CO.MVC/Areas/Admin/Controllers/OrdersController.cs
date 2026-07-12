using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SHOP.CO.MVC.Common;

namespace SHOP.CO.MVC.Areas.Admin.Controllers
{
    public class OrdersController : BaseAdminController
    {
        private IConfiguration _configuration;
        public OrdersController(IHttpClientFactory factory, IConfiguration configuration) : base(factory)
        {
            _configuration = configuration;
        }


        [HttpGet("")]       // 🟢 Map với URL: /Admin/Products
        [HttpGet("Index")]  // 🟢 Map với URL: /Admin/Products/Index
        public IActionResult Index()
        {
            // Truyền URL API và Token xuống View để AJAX gọi
            ViewBag.ApiBaseUrl = _configuration.GetSection("ApiSettings:BaseUrl").Value;
            ViewBag.Token = HttpContext.Session.GetString(MvcConstants.SessionToken);
            return View();
        }
    }
}
