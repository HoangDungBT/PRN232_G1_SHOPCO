using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SHOP.CO.Application.DTOs;
using SHOP.CO.Application.Services;

namespace SHOP.CO.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        /// <summary>
        /// POST /api/payment/process
        /// Process payment for an order (COD or VNPay).
        /// </summary>
        [HttpPost("process")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(typeof(object), 400)]
        public async Task<IActionResult> ProcessPayment([FromBody] PaymentRequestDto request)
        {
            try
            {
                if (request == null)
                    return BadRequest(new { success = false, message = "Request body is required." });

                var result = await _paymentService.ProcessPaymentAsync(request);
                return Ok(new
                {
                    success = result.Success,
                    paymentUrl = result.PaymentUrl,
                    message = result.Message,
                    orderCode = result.OrderCode,
                    paymentStatus = result.PaymentStatus
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An unexpected error occurred: " + ex.Message });
            }
        }

        /// <summary>
        /// GET /api/payment/callback
        /// VNPay payment gateway callback after user completes (or fails) payment.
        /// Updates PaymentStatus = Paid (code=00) or Failed (any other code).
        /// </summary>
        [HttpGet("callback")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(typeof(object), 400)]
        public async Task<IActionResult> VnpayCallback(
            [FromQuery(Name = "vnp_TxnRef")] string? txnRef,
            [FromQuery(Name = "vnp_ResponseCode")] string? responseCode,
            [FromQuery(Name = "vnp_SecureHash")] string? secureHash)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txnRef) ||
                    string.IsNullOrWhiteSpace(responseCode) ||
                    string.IsNullOrWhiteSpace(secureHash))
                {
                    return BadRequest(new { success = false, message = "Missing required VNPay callback parameters." });
                }

                // Build raw query string from all vnp_ params (for signature verification)
                var rawQuery = BuildRawQuery(Request.Query);

                var result = await _paymentService.HandlePaymentCallbackAsync(
                    txnRef, responseCode, secureHash, rawQuery);

                return Ok(new
                {
                    success = result.Success,
                    message = result.Message,
                    orderCode = result.OrderCode,
                    paymentStatus = result.PaymentStatus
                });
            }
            catch (KeyNotFoundException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An unexpected error occurred: " + ex.Message });
            }
        }

        /// <summary>
        /// Reconstruct raw query string (sorted, excluding vnp_SecureHash) for HMAC verification.
        /// </summary>
        private static string BuildRawQuery(IQueryCollection query)
        {
            var parts = new System.Collections.Generic.SortedDictionary<string, string>(StringComparer.Ordinal);
            foreach (var kv in query)
            {
                if (!string.IsNullOrEmpty(kv.Key) &&
                    kv.Key.StartsWith("vnp_", StringComparison.OrdinalIgnoreCase) &&
                    !kv.Key.Equals("vnp_SecureHash", StringComparison.OrdinalIgnoreCase) &&
                    !kv.Key.Equals("vnp_SecureHashType", StringComparison.OrdinalIgnoreCase))
                {
                    parts[kv.Key] = System.Net.WebUtility.UrlEncode(kv.Value.ToString());
                }
            }
            return string.Join("&", System.Linq.Enumerable.Select(parts, kv => $"{System.Net.WebUtility.UrlEncode(kv.Key)}={kv.Value}"));
        }

        /// <summary>
        /// GET /api/payment/vnpay-ipn
        /// VNPay IPN webhook endpoint
        /// </summary>
        [HttpGet("vnpay-ipn")]
        [Produces("application/json")]
        public async Task<IActionResult> VnpayIpn(
            [FromQuery(Name = "vnp_TxnRef")] string? txnRef,
            [FromQuery(Name = "vnp_ResponseCode")] string? responseCode,
            [FromQuery(Name = "vnp_SecureHash")] string? secureHash)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txnRef) ||
                    string.IsNullOrWhiteSpace(responseCode) ||
                    string.IsNullOrWhiteSpace(secureHash))
                {
                    return Ok(new { RspCode = "99", Message = "Unknown error" });
                }

                var rawQuery = BuildRawQuery(Request.Query);

                var result = await _paymentService.HandlePaymentCallbackAsync(
                    txnRef, responseCode, secureHash, rawQuery);

                return Ok(new { RspCode = "00", Message = "Confirm Success" });
            }
            catch (InvalidOperationException)
            {
                return Ok(new { RspCode = "97", Message = "Invalid signature" });
            }
            catch (KeyNotFoundException)
            {
                return Ok(new { RspCode = "01", Message = "Order not found" });
            }
            catch (Exception)
            {
                return Ok(new { RspCode = "99", Message = "Unknown error" });
            }
        }
    }
}
