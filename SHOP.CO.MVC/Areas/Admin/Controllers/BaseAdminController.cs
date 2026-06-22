using Microsoft.AspNetCore.Mvc;
using SHOP.CO.MVC.Common;  // Chứa filter AdminOnlyAttribute 
using SHOP.CO.MVC.Controllers; // Chứa ABaseController dùng chung

namespace SHOP.CO.MVC.Areas.Admin.Controllers
{
    [Area("Admin")] // Khai báo cho hệ thống biết Controller này thuộc Area Admin
    [Route("Admin/[controller]")] // tránh trùng lặp 
    [AdminOnly]     // Filter chặn người dùng thường (chỉ cho Admin, Staff)
    public class BaseAdminController : ABaseController
    {
        public BaseAdminController(IHttpClientFactory factory) : base(factory)
        {
        }

        // Thêm hàm tiện ích dùng chung GetApiAsync để gọi API phương thức GET
        protected async Task<SHOP.CO.MVC.Models.ApiResponse<TResponse>?> GetApiAsync<TResponse>(string endpoint)
        {
            try
            {
                // Bắt buộc phải gắn JWT Token vào Header trước khi gọi API Admin
                var token = HttpContext.Session.GetString(SHOP.CO.MVC.Common.MvcConstants.SessionToken);
                if (!string.IsNullOrEmpty(token))
                {
                    _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                }

                var response = await _client.GetAsync(endpoint);
                var jsonString = await response.Content.ReadAsStringAsync();

                if (string.IsNullOrWhiteSpace(jsonString))
                    return new SHOP.CO.MVC.Models.ApiResponse<TResponse> { IsSuccess = false, Message = "Không có dữ liệu" };

                var options = new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                return System.Text.Json.JsonSerializer.Deserialize<SHOP.CO.MVC.Models.ApiResponse<TResponse>>(jsonString, options);
            }
            catch (Exception ex)
            {
                return new SHOP.CO.MVC.Models.ApiResponse<TResponse> { IsSuccess = false, Message = ex.Message };
            }
        }
    }
}
