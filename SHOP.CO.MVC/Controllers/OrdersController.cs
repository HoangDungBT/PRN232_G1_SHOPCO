using Microsoft.AspNetCore.Mvc;
using SHOP.CO.MVC.Models;
using SHOP.CO.MVC.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SHOP.CO.MVC.Controllers
{
    public class OrdersController : Controller
    {
        private readonly IOrderApiClient _orderApiClient;
        private readonly IOrderTrackingApiClient _orderTrackingApiClient;

        public OrdersController(IOrderApiClient orderApiClient, IOrderTrackingApiClient orderTrackingApiClient)
        {
            _orderApiClient = orderApiClient;
            _orderTrackingApiClient = orderTrackingApiClient;
        }

        // List user's orders
        public async Task<IActionResult> Index()
        {
            var sessionIdStr = HttpContext.Session.GetString(SHOP.CO.MVC.Common.MvcConstants.SessionUserId);
            if (string.IsNullOrEmpty(sessionIdStr) || !int.TryParse(sessionIdStr, out int userId) || userId <= 0)
            {
                TempData["Error"] = "Vui lòng đăng nhập để xem đơn hàng.";
                return RedirectToAction("Login", "Auth");
            }
            try
            {
                ViewBag.UserId = userId;
                var orders = await _orderApiClient.GetOrdersByUserIdAsync(userId);
                return View(orders);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message ?? "An error occurred while loading orders.";
                return View(new List<OrderViewModel>());
            }
        }

        // View single order details
        public async Task<IActionResult> Details(int orderId)
        {
            try
            {
                var order = await _orderApiClient.GetOrderByIdAsync(orderId);
                if (order == null)
                {
                    TempData["Error"] = "Order not found.";
                    return RedirectToAction("Index");
                }
                return View(order);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message ?? "An error occurred while loading order details.";
                return RedirectToAction("Index");
            }
        }

        // Cancel order post request
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int orderId, string cancelReason)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(cancelReason))
                {
                    TempData["Error"] = "Cancel reason is required.";
                    return RedirectToAction("Details", new { orderId });
                }

                var order = await _orderApiClient.GetOrderByIdAsync(orderId);
                if (order == null)
                {
                    TempData["Error"] = "Order not found.";
                    return RedirectToAction("Index");
                }

                var success = await _orderApiClient.CancelOrderAsync(orderId, order.UserId, cancelReason);
                if (success)
                {
                    TempData["Success"] = "Order canceled successfully.";
                }
                else
                {
                    TempData["Error"] = "Failed to cancel order.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message ?? "An error occurred during order cancellation.";
            }

            return RedirectToAction("Details", new { orderId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmReceived(int orderId)
        {
            try
            {
                var order = await _orderApiClient.GetOrderByIdAsync(orderId);
                if (order == null)
                {
                    TempData["Error"] = "Order not found.";
                    return RedirectToAction("Index");
                }

                var success = await _orderApiClient.ConfirmReceivedAsync(orderId, order.UserId);
                if (success)
                {
                    TempData["Success"] = "Thank you! Order confirmed as received.";
                }
                else
                {
                    TempData["Error"] = "Failed to confirm order receipt.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message ?? "An error occurred while confirming receipt.";
            }

            return RedirectToAction("Details", new { orderId });
        }

        // Track order — GET /Orders/Tracking?orderId={id}
        public async Task<IActionResult> Tracking(int orderId)
        {
            try
            {
                if (orderId <= 0)
                {
                    TempData["Error"] = "Invalid order ID.";
                    return RedirectToAction("Index");
                }

                var tracking = await _orderTrackingApiClient.GetTrackingAsync(orderId);
                if (tracking == null)
                {
                    TempData["Error"] = "Order not found.";
                    return RedirectToAction("Index");
                }

                return View(tracking);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message ?? "An error occurred while loading order tracking.";
                return RedirectToAction("Index");
            }
        }
        // Return request — POST /Orders/ReturnRequest
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReturnRequest(int orderId, string returnReason, string returnDescription)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(returnReason))
                {
                    TempData["Error"] = "Return reason is required.";
                    return RedirectToAction("Details", new { orderId });
                }

                if (string.IsNullOrWhiteSpace(returnDescription))
                {
                    TempData["Error"] = "Return description is required.";
                    return RedirectToAction("Details", new { orderId });
                }

                var order = await _orderApiClient.GetOrderByIdAsync(orderId);
                if (order == null)
                {
                    TempData["Error"] = "Order not found.";
                    return RedirectToAction("Index");
                }

                var success = await _orderApiClient.SubmitReturnRequestAsync(orderId, order.UserId, returnReason, returnDescription);
                if (success)
                {
                    TempData["Success"] = "Return request submitted successfully. We will contact you shortly.";
                }
                else
                {
                    TempData["Error"] = "Failed to submit return request.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message ?? "An error occurred while submitting the return request.";
            }

            return RedirectToAction("Details", new { orderId });
        }

        // Download Invoice - GET /Orders/Invoice?orderId={id}
        [HttpGet]
        public async Task<IActionResult> Invoice(int orderId)
        {
            try
            {
                var order = await _orderApiClient.GetOrderByIdAsync(orderId);
                if (order == null)
                {
                    TempData["Error"] = "Order not found.";
                    return RedirectToAction("Index");
                }

                var pdfBytes = await _orderApiClient.GetInvoiceAsync(orderId, order.UserId);
                if (pdfBytes == null || pdfBytes.Length == 0)
                {
                    TempData["Error"] = "Failed to generate invoice.";
                    return RedirectToAction("Details", new { orderId });
                }

                return File(pdfBytes, "application/pdf", $"Invoice-{order.OrderCode}.pdf");
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message ?? "An error occurred while generating the invoice.";
                return RedirectToAction("Details", new { orderId });
            }
        }
    }
}
