using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Extensions.Configuration;
using SHOP.CO.Application.DTOs;
using SHOP.CO.Infrastructure.Repositories;

namespace SHOP.CO.Application.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly string _hashSecret;
        private readonly string _apiReturnUrl;

        public PaymentService(IOrderRepository orderRepository, IConfiguration configuration)
        {
            _orderRepository = orderRepository;
            _hashSecret = configuration["VnPay:HashSecret"] ?? "SHOPCODEMO1234567890ABCDEF12345678";
            _apiReturnUrl = configuration["VnPay:ReturnUrl"] ?? "http://localhost:5035/api/payment/callback";
        }

        // ─────────────────────────────────────────────────────────────
        // PUBLIC: ProcessPaymentAsync
        // ─────────────────────────────────────────────────────────────
        public async Task<PaymentResponseDto> ProcessPaymentAsync(PaymentRequestDto request)
        {
            // 1. Validate input
            if (request == null)
                throw new ArgumentException("Payment request is required.");

            if (string.IsNullOrWhiteSpace(request.PaymentMethod))
                throw new ArgumentException("Payment method is required.");

            // 2. Load order
            var order = await _orderRepository.GetOrderByIdAsync(request.OrderId);
            if (order == null)
                throw new KeyNotFoundException("Order not found.");

            // 3. Business rules
            if (string.Equals(order.OrderStatus, "Canceled", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Cannot pay for a canceled order.");

            if (string.Equals(order.OrderStatus, "Completed", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Cannot pay for a completed order.");

            if (string.Equals(order.PaymentStatus, "Paid", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Order has already been paid.");

            // 4. Route by payment method
            var method = request.PaymentMethod.Trim().ToUpperInvariant();

            if (method == "COD")
            {
                return ProcessCod(order.OrderCode, order.PaymentStatus ?? "Unpaid");
            }
            else if (method == "VNPAY")
            {
                return BuildVnpayUrl(order.OrderCode, order.TotalAmount, request.ReturnUrl);
            }
            else
            {
                throw new ArgumentException($"Unsupported payment method: {request.PaymentMethod}. Use 'COD' or 'VNPay'.");
            }
        }

        // ─────────────────────────────────────────────────────────────
        // PUBLIC: HandlePaymentCallbackAsync
        // ─────────────────────────────────────────────────────────────
        public async Task<PaymentResponseDto> HandlePaymentCallbackAsync(
            string orderCode,
            string responseCode,
            string secureHash,
            string rawQuery)
        {
            // 1. Verify HMAC signature
            if (!VerifySignature(rawQuery, secureHash))
                throw new InvalidOperationException("Invalid payment signature.");

            // 2. Find order by OrderCode
            var order = await _orderRepository.GetOrderByCodeAsync(orderCode);
            if (order == null)
                throw new KeyNotFoundException($"Order with code '{orderCode}' not found.");

            // 3. Already finalized — idempotent guard
            if (string.Equals(order.PaymentStatus, "Paid", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(order.PaymentStatus, "Failed", StringComparison.OrdinalIgnoreCase))
            {
                return new PaymentResponseDto
                {
                    Success = true,
                    PaymentUrl = null,
                    Message = $"Payment already processed: {order.PaymentStatus}",
                    OrderCode = order.OrderCode,
                    PaymentStatus = order.PaymentStatus
                };
            }

            // 4. Update PaymentStatus based on VNPay response code
            bool paymentSuccess = responseCode == "00";
            order.PaymentStatus = paymentSuccess ? "Paid" : "Failed";
            order.UpdatedAt = DateTime.UtcNow;

            await _orderRepository.SaveChangesAsync();

            return new PaymentResponseDto
            {
                Success = true,
                PaymentUrl = null,
                Message = paymentSuccess ? "Payment completed successfully." : "Payment failed.",
                OrderCode = order.OrderCode,
                PaymentStatus = order.PaymentStatus
            };
        }

        // ─────────────────────────────────────────────────────────────
        // PRIVATE HELPERS
        // ─────────────────────────────────────────────────────────────

        private static PaymentResponseDto ProcessCod(string orderCode, string currentPaymentStatus)
        {
            // COD: no DB change — PaymentStatus stays Unpaid, OrderStatus stays Pending
            return new PaymentResponseDto
            {
                Success = true,
                PaymentUrl = null,
                Message = "COD payment confirmed. Pay when your order is delivered.",
                OrderCode = orderCode,
                PaymentStatus = "Unpaid"
            };
        }

        private PaymentResponseDto BuildVnpayUrl(string orderCode, decimal amount, string? mvcReturnUrl)
        {
            // VNPay simulate: build a signed URL that points back to MVC VnpayReturn page
            var createDate = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            var returnUrl = string.IsNullOrWhiteSpace(mvcReturnUrl)
                ? _apiReturnUrl
                : mvcReturnUrl;

            // Build raw query params (sorted alphabetically per VNPay spec)
            var paramDict = new SortedDictionary<string, string>(StringComparer.Ordinal)
            {
                ["vnp_Amount"]      = ((long)(amount * 100)).ToString(),
                ["vnp_Command"]     = "pay",
                ["vnp_CreateDate"]  = createDate,
                ["vnp_CurrCode"]    = "VND",
                ["vnp_IpAddr"]      = "127.0.0.1",
                ["vnp_Locale"]      = "vn",
                ["vnp_OrderInfo"]   = Uri.EscapeDataString($"Thanh toan don hang {orderCode}"),
                ["vnp_OrderType"]   = "other",
                ["vnp_ReturnUrl"]   = Uri.EscapeDataString(returnUrl),
                ["vnp_TmnCode"]     = "SHOPCODEMO",
                ["vnp_TxnRef"]      = orderCode,
                ["vnp_Version"]     = "2.1.0",
            };

            // Build raw string to sign (key=value& pairs, NOT URL-encoded values for signature)
            var rawBuilder = new StringBuilder();
            foreach (var kv in paramDict)
            {
                if (rawBuilder.Length > 0) rawBuilder.Append('&');
                rawBuilder.Append(kv.Key).Append('=').Append(kv.Value);
            }
            var rawData = rawBuilder.ToString();

            // HMAC-SHA512 signature
            var signature = ComputeHmacSha512(_hashSecret, rawData);
            paramDict["vnp_SecureHash"] = signature;

            // Build final URL (simulate: point to MVC VnpayReturn with all params)
            var urlBuilder = new StringBuilder(returnUrl);
            urlBuilder.Append('?');
            bool first = true;
            foreach (var kv in paramDict)
            {
                if (!first) urlBuilder.Append('&');
                urlBuilder.Append(Uri.EscapeDataString(kv.Key))
                          .Append('=')
                          .Append(Uri.EscapeDataString(kv.Value));
                first = false;
            }

            return new PaymentResponseDto
            {
                Success = true,
                PaymentUrl = urlBuilder.ToString(),
                Message = "VNPay payment URL generated. Redirect customer to complete payment.",
                OrderCode = orderCode,
                PaymentStatus = "Unpaid"
            };
        }

        private bool VerifySignature(string rawQuery, string receivedHash)
        {
            // Strip vnp_SecureHash from rawQuery before verifying
            var queryParts = rawQuery.Split('&');
            var filtered = queryParts
                .Where(p => !p.StartsWith("vnp_SecureHash=", StringComparison.OrdinalIgnoreCase))
                .OrderBy(p => p, StringComparer.Ordinal);
            var rawData = string.Join("&", filtered);
            var expected = ComputeHmacSha512(_hashSecret, rawData);
            return string.Equals(expected, receivedHash, StringComparison.OrdinalIgnoreCase);
        }

        private static string ComputeHmacSha512(string secret, string data)
        {
            var keyBytes = Encoding.UTF8.GetBytes(secret);
            var dataBytes = Encoding.UTF8.GetBytes(data);
            using var hmac = new HMACSHA512(keyBytes);
            var hashBytes = hmac.ComputeHash(dataBytes);
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
        }

        public async Task<PaymentResponseDto> SimulateCallbackAsync(string orderCode, string responseCode)
        {
            var order = await _orderRepository.GetOrderByCodeAsync(orderCode);
            if (order == null)
                throw new KeyNotFoundException($"Order with code '{orderCode}' not found.");

            if (string.Equals(order.PaymentStatus, "Paid", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(order.PaymentStatus, "Failed", StringComparison.OrdinalIgnoreCase))
            {
                return new PaymentResponseDto
                {
                    Success = true,
                    PaymentUrl = null,
                    Message = $"Payment already processed: {order.PaymentStatus}",
                    OrderCode = order.OrderCode,
                    PaymentStatus = order.PaymentStatus
                };
            }

            bool paymentSuccess = responseCode == "00";
            order.PaymentStatus = paymentSuccess ? "Paid" : "Failed";
            order.UpdatedAt = DateTime.UtcNow;

            await _orderRepository.SaveChangesAsync();

            return new PaymentResponseDto
            {
                Success = true,
                PaymentUrl = null,
                Message = paymentSuccess ? "Payment completed successfully (Simulated)." : "Payment failed (Simulated).",
                OrderCode = order.OrderCode,
                PaymentStatus = order.PaymentStatus
            };
        }
    }
}
