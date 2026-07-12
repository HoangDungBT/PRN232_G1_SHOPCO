using Microsoft.AspNetCore.Mvc;
using SHOP.CO.MVC.Common;

namespace SHOP.CO.MVC.Areas.Admin.Controllers
{
    public class InventoryController : BaseAdminController
    {
        private readonly IConfiguration _configuration;
        public InventoryController(IHttpClientFactory factory, IConfiguration configuration) : base(factory)
        {
            _configuration = configuration;
        }

        [HttpGet("")]
        [HttpGet("Index")]
        public IActionResult Index()
        {
            ViewBag.ApiBaseUrl = _configuration.GetSection("ApiSettings:BaseUrl").Value;
            ViewBag.Token = HttpContext.Session.GetString(MvcConstants.SessionToken);
            return View();
        }
    }
}