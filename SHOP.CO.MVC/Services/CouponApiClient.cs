using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using SHOP.CO.MVC.Models;

namespace SHOP.CO.MVC.Services
{
    public class CouponApiClient : ICouponApiClient
    {
        private readonly HttpClient _httpClient;

        public CouponApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<CouponResponseViewModel?> ApplyCouponAsync(int userId, string couponCode)
        {
            var contentObject = new { userId, couponCode };
            var json = JsonSerializer.Serialize(contentObject);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/coupon/apply", content);

            var responseContent = await response.Content.ReadAsStringAsync();

            var apiResponse = JsonSerializer.Deserialize<ApiResponse<CouponResponseViewModel>>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (apiResponse != null)
            {
                if (apiResponse.Success && apiResponse.Data != null)
                {
                    apiResponse.Data.Success = true;
                    apiResponse.Data.Message = apiResponse.Message ?? "Coupon applied";
                    return apiResponse.Data;
                }
                else
                {
                    return new CouponResponseViewModel
                    {
                        Success = false,
                        Message = apiResponse.Message ?? "Failed to apply coupon."
                    };
                }
            }

            return new CouponResponseViewModel
            {
                Success = false,
                Message = "Unable to contact API service."
            };
        }
    }
}
