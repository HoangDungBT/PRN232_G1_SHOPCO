using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SHOP.CO.MVC.Common;

namespace SHOP.CO.MVC.Areas.Admin.Controllers
{
    public class UsersController : BaseAdminController
    {
        private readonly IConfiguration _configuration;

        public UsersController(IHttpClientFactory factory, IConfiguration configuration) : base(factory)
        {
            _configuration = configuration;
        }

        [HttpGet("")]
        [HttpGet("Index")]
        public IActionResult Index()
        {
            ViewBag.ApiBaseUrl = _configuration.GetSection("ApiSettings:BaseUrl").Value;
            ViewBag.Token = HttpContext.Session.GetString(MvcConstants.SessionToken);
            ViewBag.Role = HttpContext.Session.GetString(MvcConstants.SessionRole);
            return View();
        }
    }
}
