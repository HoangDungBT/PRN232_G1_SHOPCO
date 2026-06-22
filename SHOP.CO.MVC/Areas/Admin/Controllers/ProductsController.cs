using Microsoft.AspNetCore.Mvc;
using SHOP.CO.MVC.Common;

namespace SHOP.CO.MVC.Areas.Admin.Controllers
{
    public class ProductsController : BaseAdminController
    {
        private IConfiguration _configuration;
        public ProductsController(IHttpClientFactory factory, IConfiguration configuration) : base(factory)
        {
            _configuration = configuration;
        }

        [HttpGet("")]       // 🟢 Map với URL: /Admin/Products
        [HttpGet("Index")]  // 🟢 Map với URL: /Admin/Products/Index
        public IActionResult Index()
        {
            ViewBag.ApiBaseUrl = _configuration.GetSection("ApiSettings:BaseUrl").Value;
            ViewBag.Token = HttpContext.Session.GetString(MvcConstants.SessionToken);
            return View();
        }
    }
}
