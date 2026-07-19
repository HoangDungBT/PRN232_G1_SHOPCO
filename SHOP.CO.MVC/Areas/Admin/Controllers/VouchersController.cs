using Microsoft.AspNetCore.Mvc;
using SHOP.CO.MVC.Common;
namespace SHOP.CO.MVC.Areas.Admin.Controllers
{
    public class VouchersController : BaseAdminController
    {
        private readonly IConfiguration _configuration;

        public VouchersController(IHttpClientFactory factory, IConfiguration configuration) : base(factory)
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