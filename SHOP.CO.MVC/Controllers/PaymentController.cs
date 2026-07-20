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
        /// Handles the return redirect from VNPay portal.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> VnpayReturn(
            int orderId,
            string orderCode,
            decimal totalAmount,
            [FromQuery(Name = "vnp_ResponseCode")] string? responseCode = null,
            [FromQuery(Name = "vnp_TxnRef")] string? txnRef = null)
        {
            var actualOrderCode = !string.IsNullOrWhiteSpace(txnRef) ? txnRef : orderCode;

            // If we have a response code, process it via API callback endpoint
            if (!string.IsNullOrWhiteSpace(responseCode))
            {
                // In a real scenario, we should pass all vnp_ params to the API to verify the signature.
                // However, the IPN might have already processed it.
                // Let's call the API callback to process it, and it will safely ignore if already paid.
                var queryParams = Request.QueryString.Value;
                using var httpClient = new HttpClient { BaseAddress = new Uri(_apiBaseUrl) };
                
                // Forward the exact query string to our API callback to verify HMAC and update DB
                var callbackUrl = $"api/payment/callback{queryParams}";
                var response = await httpClient.GetAsync(callbackUrl);
                
                if (!response.IsSuccessStatusCode)
                {
                    // API failed to validate or update the DB (e.g. HMAC mismatch)
                    return RedirectToAction("Success", new { sorderCode = actualOrderCode, paymentStatus = "Failed", paymentMethod = "VNPay" });
                }

                // "24" means user canceled on VNPay
                if (responseCode == "24")
                {
                    // Call API to cancel order if needed or just show failed
                    return RedirectToAction("Success", new { orderCode = actualOrderCode, paymentStatus = "Failed", paymentMethod = "VNPay" });
                }

                // If responseCode is "00", it's a success
                if (responseCode == "00")
                {
                    return RedirectToAction("Success", new { orderCode = actualOrderCode, paymentStatus = "Paid", paymentMethod = "VNPay" });
                }

                // Other error codes
                return RedirectToAction("Success", new { orderCode = actualOrderCode, paymentStatus = "Failed", paymentMethod = "VNPay" });
            }

            // If there's no response code (e.g. they opened VNPay QR and returned to our site manually),
            // show the Pending waiting screen
            return RedirectToAction("Pending", new { orderId, orderCode = actualOrderCode, totalAmount });
        }

        /// <summary>
        /// GET /Payment/Pending
        /// Shows the waiting screen that polls for payment status.
        /// </summary>
        [HttpGet]
        public IActionResult Pending(int orderId, string orderCode, decimal totalAmount)
        {
            ViewBag.OrderId = orderId;
            ViewBag.OrderCode = orderCode;
            ViewBag.TotalAmount = totalAmount;
            return View();
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
