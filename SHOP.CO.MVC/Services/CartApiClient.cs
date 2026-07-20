using System.Text.Json;
using SHOP.CO.MVC.Models;

namespace SHOP.CO.MVC.Services
{
    public class CartApiClient : ICartApiClient
    {
        private readonly HttpClient _httpClient;

        public CartApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<CartViewModel> GetCartAsync(int userId)
        {
            var response = await _httpClient.GetAsync($"api/cart/{userId}");
            
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException("Failed to load cart.");
            }

            var stream = await response.Content.ReadAsStreamAsync();
            var apiResponse = await JsonSerializer.DeserializeAsync<ApiResponse<CartViewModel>>(stream, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (apiResponse == null || !apiResponse.Success || apiResponse.Data == null)
            {
                throw new InvalidOperationException(apiResponse?.Message ?? "Failed to parse cart data.");
            }

            return apiResponse.Data;
        }

        public async Task<bool> RemoveFromCartAsync(int cartItemId, int userId)
        {
            var response = await _httpClient.DeleteAsync($"api/cart/{cartItemId}?userId={userId}");
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> AddToCartAsync(int userId, int variantId, int quantity)
        {
            var contentObject = new { variantId, quantity };
            var json = JsonSerializer.Serialize(contentObject);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"api/cart?userId={userId}", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateQuantityAsync(int cartItemId, int userId, int quantity)
        {
            var contentObject = new { quantity };
            var json = JsonSerializer.Serialize(contentObject);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"api/cart/{cartItemId}?userId={userId}", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<CheckoutResultViewModel?> CheckoutAsync(int userId, string? couponCode, int? addressId = null, string? customerNote = null, string? paymentMethod = null, string? otpCode = null)
        {
            var contentObject = new { userId, couponCode, addressId, customerNote, paymentMethod, otpCode };
            var json = JsonSerializer.Serialize(contentObject);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/orders/checkout", content);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                var errorResponse = JsonSerializer.Deserialize<ApiResponse<object>>(errorContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                throw new InvalidOperationException(errorResponse?.Message ?? "Checkout failed.");
            }

            var stream = await response.Content.ReadAsStreamAsync();
            var apiResponse = await JsonSerializer.DeserializeAsync<ApiResponse<CheckoutResultViewModel>>(stream, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (apiResponse == null || !apiResponse.Success || apiResponse.Data == null)
            {
                throw new InvalidOperationException(apiResponse?.Message ?? "Failed to parse checkout response.");
            }

            return apiResponse.Data;
        }

        public async Task<List<UserAddressViewModel>> GetUserAddressesAsync(int userId)
        {
            var response = await _httpClient.GetAsync($"api/users/{userId}/addresses");
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException("Failed to load user addresses.");
            }

            var stream = await response.Content.ReadAsStreamAsync();
            var apiResponse = await JsonSerializer.DeserializeAsync<ApiResponse<List<UserAddressViewModel>>>(stream, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (apiResponse == null || !apiResponse.Success || apiResponse.Data == null)
            {
                throw new InvalidOperationException(apiResponse?.Message ?? "Failed to parse user addresses.");
            }

            return apiResponse.Data;
        }
        public async Task<(bool success, string message)> SendCheckoutOtpAsync(int userId)
        {
            var response = await _httpClient.PostAsync($"api/orders/send-otp?userId={userId}", null);
            var content = await response.Content.ReadAsStringAsync();
            try
            {
                if (!string.IsNullOrWhiteSpace(content))
                {
                    var parsed = System.Text.Json.JsonSerializer.Deserialize<System.Text.Json.JsonElement>(content);
                    var msg = parsed.TryGetProperty("message", out var m) ? m.GetString() ?? "" : "";
                    return (response.IsSuccessStatusCode, msg);
                }
            }
            catch
            {
                // ignore parse errors
            }
            return (response.IsSuccessStatusCode, response.IsSuccessStatusCode ? "OTP sent successfully." : "Failed to send OTP.");
        }
    }
}




