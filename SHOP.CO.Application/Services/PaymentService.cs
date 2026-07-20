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


using SHOP.CO.Infrastructure.Data;
using SHOP.CO.Domain.Entities;
using SHOP.CO.Application.Common;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
namespace SHOP.CO.Application.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly SHOP.CO.Infrastructure.Repositories.IOrderRepository _orderRepository;
        private readonly string _hashSecret;
        private readonly string _apiReturnUrl;
        private readonly string _tmnCode;
        private readonly string _vnpUrl;

        public PaymentService(SHOP.CO.Infrastructure.Repositories.IOrderRepository orderRepository, IConfiguration configuration)
        {
            _orderRepository = orderRepository;
            _hashSecret = configuration["VnPay:HashSecret"] ?? "KGJFM6ZNUK8LIJUW4NVC7RSCVKC5QPB6";
            _apiReturnUrl = configuration["VnPay:ReturnUrl"] ?? "http://localhost:5035/api/payment/callback";
            _tmnCode = configuration["VnPay:TmnCode"] ?? "YMFRE6R1";
            _vnpUrl = configuration["VnPay:BaseUrl"] ?? "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html";
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
            // Build a signed URL that points to the actual VNPay portal
            var createDate = DateTime.Now.ToString("yyyyMMddHHmmss");
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
                ["vnp_OrderInfo"]   = $"Thanh toan don hang {orderCode}",
                ["vnp_OrderType"]   = "other",
                ["vnp_ReturnUrl"]   = returnUrl,
                ["vnp_TmnCode"]     = _tmnCode,
                ["vnp_TxnRef"]      = orderCode,
                ["vnp_Version"]     = "2.1.0",
            };

            // Build raw string to sign (key=value& pairs, URL-encoded values)
            var rawBuilder = new StringBuilder();
            foreach (var kv in paramDict)
            {
                if (rawBuilder.Length > 0) rawBuilder.Append('&');
                rawBuilder.Append(System.Net.WebUtility.UrlEncode(kv.Key))
                          .Append('=')
                          .Append(System.Net.WebUtility.UrlEncode(kv.Value));
            }
            var rawData = rawBuilder.ToString();

            // HMAC-SHA512 signature
            var signature = ComputeHmacSha512(_hashSecret, rawData);

            // Build final URL (points to VNPay BaseUrl)
            var paymentUrl = $"{_vnpUrl}?{rawData}&vnp_SecureHash={signature}";

            return new PaymentResponseDto
            {
                Success = true,
                PaymentUrl = paymentUrl,
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
