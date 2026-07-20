using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using SHOP.CO.MVC.Models;

namespace SHOP.CO.MVC.Services
{
    public class OrderApiClient : IOrderApiClient
    {
        private readonly HttpClient _httpClient;

        public OrderApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<OrderViewModel>> GetOrdersByUserIdAsync(int userId)
        {
            var response = await _httpClient.GetAsync($"api/orders/user/{userId}");
            
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException("Failed to load orders.");
            }

            var stream = await response.Content.ReadAsStreamAsync();
            var apiResponse = await JsonSerializer.DeserializeAsync<ApiResponse<List<OrderViewModel>>>(stream, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (apiResponse == null || !apiResponse.Success || apiResponse.Data == null)
            {
                throw new InvalidOperationException(apiResponse?.Message ?? "Failed to parse orders data.");
            }

            return apiResponse.Data;
        }

        public async Task<OrderViewModel?> GetOrderByIdAsync(int orderId)
        {
            var response = await _httpClient.GetAsync($"api/orders/{orderId}");
            
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException("Failed to load order details.");
            }

            var stream = await response.Content.ReadAsStreamAsync();
            var apiResponse = await JsonSerializer.DeserializeAsync<ApiResponse<OrderViewModel>>(stream, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (apiResponse == null || !apiResponse.Success || apiResponse.Data == null)
            {
                throw new InvalidOperationException(apiResponse?.Message ?? "Failed to parse order details.");
            }

            return apiResponse.Data;
        }

        public async Task<bool> CancelOrderAsync(int orderId, int userId, string cancelReason)
        {
            var contentObject = new { cancelReason };
            var json = JsonSerializer.Serialize(contentObject);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"api/orders/{orderId}/cancel?userId={userId}", content);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                var errorResponse = JsonSerializer.Deserialize<ApiResponse<object>>(errorContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                throw new InvalidOperationException(errorResponse?.Message ?? "Cancellation failed.");
            }

            var stream = await response.Content.ReadAsStreamAsync();
            var apiResponse = await JsonSerializer.DeserializeAsync<ApiResponse<bool>>(stream, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return apiResponse != null && apiResponse.Success && apiResponse.Data;
        }

        public async Task<bool> ConfirmReceivedAsync(int orderId, int userId)
        {
            var content = new StringContent("{}", System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"api/orders/{orderId}/confirm-received?userId={userId}", content);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                var errorResponse = JsonSerializer.Deserialize<ApiResponse<object>>(errorContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                throw new InvalidOperationException(errorResponse?.Message ?? "Confirmation failed.");
            }

            var stream = await response.Content.ReadAsStreamAsync();
            var apiResponse = await JsonSerializer.DeserializeAsync<ApiResponse<bool>>(stream, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return apiResponse != null && apiResponse.Success && apiResponse.Data;
        }
        public async Task<bool> SubmitReturnRequestAsync(int orderId, int userId, string reason, string description)
        {
            var contentObject = new { reason, description };
            var json = JsonSerializer.Serialize(contentObject);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"api/orders/{orderId}/return-request?userId={userId}", content);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                var errorResponse = JsonSerializer.Deserialize<ApiResponse<object>>(errorContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                throw new InvalidOperationException(errorResponse?.Message ?? "Return request failed.");
            }

            var stream = await response.Content.ReadAsStreamAsync();
            var apiResponse = await JsonSerializer.DeserializeAsync<ApiResponse<bool>>(stream, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return apiResponse != null && apiResponse.Success && apiResponse.Data;
        }

        public async Task<byte[]?> GetInvoiceAsync(int orderId, int userId)
        {
            var response = await _httpClient.GetAsync($"api/orders/{orderId}/invoice?userId={userId}");
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                try
                {
                    var errorResponse = JsonSerializer.Deserialize<ApiResponse<object>>(errorContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    if (errorResponse != null && !string.IsNullOrEmpty(errorResponse.Message))
                    {
                        throw new InvalidOperationException(errorResponse.Message);
                    }
                }
                catch (JsonException)
                {
                    // Fallback
                }
                throw new InvalidOperationException($"Failed to get invoice. Status code: {response.StatusCode}");
            }

            return await response.Content.ReadAsByteArrayAsync();
        }
    }
}
