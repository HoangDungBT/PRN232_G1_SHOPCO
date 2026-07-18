using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using SHOP.CO.MVC.Models;

namespace SHOP.CO.MVC.Services
{
    public class PaymentApiClient : IPaymentApiClient
    {
        private readonly HttpClient _httpClient;

        private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        public PaymentApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<PaymentResponseViewModel?> ProcessPaymentAsync(
            int orderId, string paymentMethod, string returnUrl)
        {
            var bodyObj = new
            {
                orderId,
                paymentMethod,
                returnUrl
            };

            var json = JsonSerializer.Serialize(bodyObj);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("api/payment/process", content);

            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                // Try to parse error message
                try
                {
                    var errorResponse = JsonSerializer.Deserialize<ApiResponse<object>>(responseContent, _jsonOptions);
                    throw new InvalidOperationException(errorResponse?.Message ?? "Payment processing failed.");
                }
                catch (JsonException)
                {
                    throw new InvalidOperationException($"Payment API error: {response.StatusCode}");
                }
            }

            var apiResponse = JsonSerializer.Deserialize<ApiResponse<PaymentResponseViewModel>>(responseContent, _jsonOptions);

            if (apiResponse == null)
                return null;

            // The API returns flat response (not wrapped in data), so map directly
            return new PaymentResponseViewModel
            {
                Success = apiResponse.Success,
                PaymentUrl = GetStringField(responseContent, "paymentUrl"),
                Message = apiResponse.Message,
                OrderCode = GetStringField(responseContent, "orderCode"),
                PaymentStatus = GetStringField(responseContent, "paymentStatus")
            };
        }

        /// <summary>
        /// Helper to parse a specific string field from raw JSON 
        /// (since API returns flat object, not wrapped in ApiResponse.Data)
        /// </summary>
        private static string? GetStringField(string json, string fieldName)
        {
            try
            {
                using var doc = JsonDocument.Parse(json);
                if (doc.RootElement.TryGetProperty(fieldName, out var elem))
                {
                    return elem.ValueKind == JsonValueKind.Null ? null : elem.GetString();
                }
            }
            catch { }
            return null;
        }
    }
}
