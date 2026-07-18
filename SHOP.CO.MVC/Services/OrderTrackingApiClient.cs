using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using SHOP.CO.MVC.Models;

namespace SHOP.CO.MVC.Services
{
    public class OrderTrackingApiClient : IOrderTrackingApiClient
    {
        private readonly HttpClient _httpClient;

        public OrderTrackingApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<OrderTrackingViewModel?> GetTrackingAsync(int orderId)
        {
            var response = await _httpClient.GetAsync($"api/orders/{orderId}/tracking");

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Failed to load order tracking. Status: {response.StatusCode}");
            }

            var stream = await response.Content.ReadAsStreamAsync();
            var apiResponse = await JsonSerializer.DeserializeAsync<ApiResponse<OrderTrackingViewModel>>(
                stream,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (apiResponse == null || !apiResponse.Success || apiResponse.Data == null)
            {
                throw new InvalidOperationException(apiResponse?.Message ?? "Failed to parse order tracking data.");
            }

            return apiResponse.Data;
        }
    }
}
