using Microsoft.AspNetCore.Mvc;
using SHOP.CO.MVC.Models;
using System.Text.Json;
namespace SHOP.CO.MVC.Controllers
{
    public class ABaseController : Controller
    {
        protected readonly HttpClient _client;

        public ABaseController(IHttpClientFactory factory)
        {
            _client = factory.CreateClient("ShopCoApi");
        }
        /// <summary>
        /// Hàm tiện ích dùng chung cho các request POST
        /// Tự động đọc JSON và ép kiểu thành ApiResponse<TResponse>
        /// </summary>
        protected async Task<ApiResponse<TResponse>?> PostApiAsync<TResponse>(string endpoint, object payload)
        {
            try
            {
                var response = await _client.PostAsJsonAsync(endpoint, payload);
                // 1. Đọc nội dung API trả về dưới dạng chuỗi (string) trước
                var jsonString = await response.Content.ReadAsStringAsync();

                // 2. Kiểm tra nếu API trả về chuỗi rỗng (Lỗi 404 Not Found hoặc API chưa chạy)
                if (string.IsNullOrWhiteSpace(jsonString))
                {
                    return new ApiResponse<TResponse>
                    {
                        IsSuccess = false,
                        Message = $"Lỗi kết nối API: Máy chủ trả về mã {response.StatusCode} nhưng không có dữ liệu."
                    };
                }

                // 3. Nếu có dữ liệu, mới tiến hành ép kiểu (Parse) sang JSON
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                return JsonSerializer.Deserialize<ApiResponse<TResponse>>(jsonString, options);

            }
            catch (Exception ex)
            {
                return new ApiResponse<TResponse>
                {
                    IsSuccess = false,
                    // Bắt lỗi rớt mạng, sập server...
                    Message = $"Lỗi kết nối đến máy chủ API: {ex.Message}",
                };
            }
        }
    }
}
