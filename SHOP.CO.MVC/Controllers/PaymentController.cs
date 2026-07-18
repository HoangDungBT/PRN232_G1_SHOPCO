using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SHOP.CO.MVC.Models;
using SHOP.CO.MVC.Services;

namespace SHOP.CO.MVC.Controllers
{
    public class PaymentController : Controller
    {
        private readonly IPaymentApiClient _paymentApiClient;
        private readonly IOrderApiClient _orderApiClient;
        private readonly string _apiBaseUrl;

        public PaymentController(IPaymentApiClient paymentApiClient, IOrderApiClient orderApiClient, IConfiguration configuration)
        {
            _paymentApiClient = paymentApiClient;
            _orderApiClient = orderApiClient;
            _apiBaseUrl = configuration["ApiSettings:BaseUrl"] ?? "http://localhost:5035/";
        }

        /// <summary>
        /// GET /Payment/Index?orderId=X&orderCode=ORD-XXX&totalAmount=Y
        /// Show payment method selection page.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Index(int orderId, string orderCode, decimal totalAmount)
        {
            try
            {
                var order = await _orderApiClient.GetOrderByIdAsync(orderId);
                var model = new PaymentIndexViewModel
                {
                    OrderId = orderId,
                    OrderCode = order?.OrderCode ?? orderCode,
                    TotalAmount = order?.TotalAmount ?? totalAmount,
                    OrderStatus = order?.OrderStatus ?? "Pending",
                    PaymentStatus = order?.PaymentStatus ?? "Unpaid"
                };
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message ?? "Failed to load order details for payment.";
                return View(new PaymentIndexViewModel { OrderId = orderId, OrderCode = orderCode, TotalAmount = totalAmount, OrderStatus = "Pending", PaymentStatus = "Unpaid" });
            }
        }

        /// <summary>
        /// POST /Payment/ProcessPayment
        /// Handle payment method selection (COD or VNPay).
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcessPayment(int orderId, string orderCode, decimal totalAmount, string paymentMethod)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(paymentMethod))
                {
                    TempData["Error"] = "Please select a payment method.";
                    return RedirectToAction("Index", new { orderId, orderCode, totalAmount });
                }

                // Build return URL for VNPay to redirect back to
                var returnUrl = $"{Request.Scheme}://{Request.Host}/Payment/VnpayReturn?orderId={orderId}&orderCode={Uri.EscapeDataString(orderCode)}&totalAmount={totalAmount}";

                var result = await _paymentApiClient.ProcessPaymentAsync(orderId, paymentMethod, returnUrl);

                if (result == null || !result.Success)
                {
                    TempData["Error"] = result?.Message ?? "Payment processing failed. Please try again.";
                    return RedirectToAction("Index", new { orderId, orderCode, totalAmount });
                }

                if (paymentMethod.Equals("COD", StringComparison.OrdinalIgnoreCase))
                {
                    // COD: go to success page directly
                    return RedirectToAction("Success", new
                    {
                        orderCode = result.OrderCode ?? orderCode,
                        paymentStatus = result.PaymentStatus ?? "Unpaid",
                        paymentMethod = "COD"
                    });
                }
                else
                {
                    // VNPay: redirect to VnpayReturn simulation page (sandbox)
                    // The paymentUrl already contains all needed params
                    if (!string.IsNullOrWhiteSpace(result.PaymentUrl))
                    {
                        return Redirect(result.PaymentUrl);
                    }
                    TempData["Error"] = "Failed to generate VNPay payment URL.";
                    return RedirectToAction("Index", new { orderId, orderCode, totalAmount });
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message ?? "An error occurred during payment.";
                return RedirectToAction("Index", new { orderId, orderCode, totalAmount });
            }
        }

        /// <summary>
        /// GET /Payment/VnpayReturn
        /// Sandbox simulation page: shows order info and lets user simulate success/failure.
        /// In real VNPay integration this would receive VNPay callback parameters.
        /// </summary>
        [HttpGet]
        public IActionResult VnpayReturn(
            int orderId,
            string orderCode,
            decimal totalAmount,
            [FromQuery(Name = "vnp_Amount")] string? vnpAmount = null,
            [FromQuery(Name = "vnp_TxnRef")] string? txnRef = null)
        {
            // Show the simulation page with order details
            ViewBag.OrderId = orderId;
            ViewBag.OrderCode = !string.IsNullOrWhiteSpace(txnRef) ? txnRef : orderCode;
            ViewBag.TotalAmount = totalAmount;
            ViewBag.AllParams = Request.QueryString.Value ?? "";
            return View();
        }

        /// <summary>
        /// POST /Payment/SimulateVnpay
        /// Simulate VNPay callback by calling the API callback endpoint directly.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SimulateVnpay(string orderCode, decimal totalAmount, bool simulateSuccess)
        {
            try
            {
                // Build callback query to API
                var responseCode = simulateSuccess ? "00" : "99";

                // Build the raw query the API expects (without SecureHash for simulate — API will skip verify in simulate mode)
                // For simulation we call the API callback endpoint directly with a pre-computed hash
                using var httpClient = new HttpClient { BaseAddress = new Uri(_apiBaseUrl) };

                // Compute HMAC signature for the simulation request
                var paramString = $"vnp_ResponseCode={responseCode}&vnp_TxnRef={orderCode}";
                var hashSecret = "SHOPCODEMO1234567890ABCDEF12345678";
                var signature = ComputeHmacSha512(hashSecret, paramString);

                var callbackUrl = $"api/payment/callback?vnp_TxnRef={Uri.EscapeDataString(orderCode)}&vnp_ResponseCode={responseCode}&vnp_SecureHash={Uri.EscapeDataString(signature)}";
                var response = await httpClient.GetAsync(callbackUrl);
                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var paymentStatus = simulateSuccess ? "Paid" : "Failed";
                    return RedirectToAction("Success", new
                    {
                        orderCode,
                        paymentStatus,
                        paymentMethod = "VNPay"
                    });
                }
                else
                {
                    // Parse error and try to show success page anyway in simulate mode
                    var paymentStatus = simulateSuccess ? "Paid" : "Failed";
                    TempData["Warning"] = $"Callback note: {content}";
                    return RedirectToAction("Success", new
                    {
                        orderCode,
                        paymentStatus,
                        paymentMethod = "VNPay"
                    });
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message ?? "Simulation failed.";
                return RedirectToAction("Index");
            }
        }

        /// <summary>
        /// GET /Payment/Success
        /// Final payment success/confirmation page.
        /// </summary>
        [HttpGet]
        public IActionResult Success(string orderCode, string paymentStatus, string paymentMethod)
        {
            ViewBag.OrderCode = orderCode;
            ViewBag.PaymentStatus = paymentStatus;
            ViewBag.PaymentMethod = paymentMethod;
            return View();
        }

        // ─── HMAC helper (mirrors PaymentService for simulate) ─────────────────
        private static string ComputeHmacSha512(string secret, string data)
        {
            var keyBytes = System.Text.Encoding.UTF8.GetBytes(secret);
            var dataBytes = System.Text.Encoding.UTF8.GetBytes(data);
            using var hmac = new System.Security.Cryptography.HMACSHA512(keyBytes);
            var hashBytes = hmac.ComputeHash(dataBytes);
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
        }
    }
}
