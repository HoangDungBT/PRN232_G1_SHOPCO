using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using SHOP.CO.MVC.Common;

namespace SHOP.CO.MVC.Areas.Admin.Controllers
{
    public class CategoriesController : BaseAdminController
    {
        private readonly IConfiguration _configuration;

        public CategoriesController(IHttpClientFactory factory, IConfiguration configuration) : base(factory)
        {
            _configuration = configuration;
        }

        [HttpGet("")]
        [HttpGet("Index")]
        public IActionResult Index()
        {
            // Truyền URL API và Token xuống View để AJAX gọi
            ViewBag.ApiBaseUrl = _configuration.GetSection("ApiSettings:BaseUrl").Value;
            ViewBag.Token = HttpContext.Session.GetString(MvcConstants.SessionToken);
            return View();
        }
    }
}
