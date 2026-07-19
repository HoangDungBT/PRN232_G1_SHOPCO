using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace SHOP.CO.MVC.Common
{
    public class AdminOnlyAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // Đọc Role từ Session
            var role = context.HttpContext.Session.GetString(MvcConstants.SessionRole);

            // Nếu không phải Admin hoặc Staff -> Đuổi về trang chủ hoặc báo lỗi 403
            if (role != "Admin" && role != "Staff")
            {
                context.Result = new RedirectToActionResult("Index", "Home", new { area = "" });
                // Hoặc: context.Result = new ForbidResult();
            }

            base.OnActionExecuting(context);

            //  [AdminOnly] 
            //public class AdminController : Controller


        }
    }
}
