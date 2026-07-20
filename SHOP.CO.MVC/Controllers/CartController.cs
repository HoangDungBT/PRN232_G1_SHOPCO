using Microsoft.AspNetCore.Mvc;
using SHOP.CO.MVC.Models;
using SHOP.CO.MVC.Services;

namespace SHOP.CO.MVC.Controllers
{
    /// <summary>
    /// MVC controller for shopping cart pages.
    /// Uses ICartApiClient to call SHOP.CO.API endpoints.
    /// </summary>
    public class CartController : Controller
    {
        private readonly ICartApiClient _cartApiClient;
        private readonly ICouponApiClient _couponApiClient;
        private readonly IPaymentApiClient _paymentApiClient;

        public CartController(ICartApiClient cartApiClient, ICouponApiClient couponApiClient, IPaymentApiClient paymentApiClient)
        {
            _cartApiClient = cartApiClient;
            _couponApiClient = couponApiClient;
            _paymentApiClient = paymentApiClient;
        }

        private int GetUserIdOrDefault(int providedUserId)
        {
            var sessionUserIdStr = HttpContext.Session.GetString(SHOP.CO.MVC.Common.MvcConstants.SessionUserId);
            if (!string.IsNullOrEmpty(sessionUserIdStr) && int.TryParse(sessionUserIdStr, out int sessionUserId))
            {
                return sessionUserId;
            }
            return providedUserId > 0 ? providedUserId : 1;
        }

        /// <summary>
        /// Display user's shopping cart by calling GET /api/cart/{userId}
        /// </summary>
        /// <param name="userId">Optional user id; defaults to 1 for demo</param>
        public async Task<IActionResult> Index(int userId = 1, string? couponCode = null)
        {
            userId = GetUserIdOrDefault(userId);
            try
            {
                var cart = await _cartApiClient.GetCartAsync(userId);
                if (cart == null)
                {
                    cart = new CartViewModel { UserId = userId };
                }

                if (cart.Items != null)
                {
                    foreach (var item in cart.Items)
                    {
                        item.ImageUrl = GetProductImageUrl(item.ProductName);
                    }
                }

                if (!string.IsNullOrWhiteSpace(couponCode))
                {
                    cart.CouponCode = couponCode;
                    var couponResult = await _couponApiClient.ApplyCouponAsync(userId, couponCode);
                    if (couponResult != null && couponResult.Success)
                    {
                        cart.DiscountAmount = couponResult.DiscountAmount;
                        cart.FinalAmount = couponResult.FinalAmount + cart.ShippingFee;
                        TempData["CouponSuccess"] = couponResult.Message ?? "Coupon applied successfully.";
                    }
                    else
                    {
                        cart.DiscountAmount = 0m;
                        cart.FinalAmount = cart.SubtotalAmount + cart.ShippingFee;
                        TempData["Error"] = couponResult?.Message ?? "Invalid coupon";
                    }
                }
                else
                {
                    cart.DiscountAmount = 0m;
                    cart.FinalAmount = cart.SubtotalAmount + cart.ShippingFee;
                }

                return View(cart);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message ?? "An error occurred while loading the cart.";
                return View(new CartViewModel { UserId = userId });
            }
        }

        /// <summary>
        /// Remove a cart item by calling DELETE /api/cart/{cartItemId}?userId={userId}
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(int cartItemId, int userId = 1)
        {
            userId = GetUserIdOrDefault(userId);
            try
            {
                var success = await _cartApiClient.RemoveFromCartAsync(cartItemId, userId);

                if (!success)
                {
                    TempData["Error"] = "Failed to remove item from cart.";
                }
            }
            catch
            {
                TempData["Error"] = "An unexpected error occurred while removing the item.";
            }

            return RedirectToAction("Index", new { userId });
        }

        /// <summary>
        /// Add a product to the cart and redirect to cart index
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> AddToCart(int id, string name, decimal price, string image, int quantity = 1, int userId = 1)
        {
            userId = GetUserIdOrDefault(userId);
            try
            {
                var success = await _cartApiClient.AddToCartAsync(userId, id, quantity);
                if (!success)
                {
                    TempData["Error"] = "Failed to add item to cart.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message ?? "An error occurred while adding the item.";
            }

            return RedirectToAction("Index", new { userId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateQuantity(int cartItemId, int quantity, int userId = 1)
        {
            userId = GetUserIdOrDefault(userId);
            try
            {
                if (quantity <= 0)
                {
                    TempData["Error"] = "Quantity must be greater than zero.";
                    return RedirectToAction("Index", new { userId });
                }

                var success = await _cartApiClient.UpdateQuantityAsync(cartItemId, userId, quantity);
                if (!success)
                {
                    TempData["Error"] = "Failed to update quantity (out of stock or variant inactive).";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message ?? "An error occurred while updating the item.";
            }

            return RedirectToAction("Index", new { userId });
        }

        [HttpGet]
        public async Task<IActionResult> Checkout(int userId = 1, string? couponCode = null)
        {
            userId = GetUserIdOrDefault(userId);
            try
            {
                var cart = await _cartApiClient.GetCartAsync(userId);
                if (cart == null || cart.Items == null || !cart.Items.Any())
                {
                    TempData["Error"] = "Giỏ hàng trống. Vui lòng thêm sản phẩm trước khi thanh toán.";
                    return RedirectToAction("Index", new { userId });
                }

                foreach (var item in cart.Items)
                {
                    item.ImageUrl = GetProductImageUrl(item.ProductName);
                    if (item.Quantity > item.AvailableStock)
                    {
                        TempData["Error"] = "Một số sản phẩm đã vượt số lượng tồn kho. Vui lòng cập nhật trước khi thanh toán.";
                        return RedirectToAction("Index", new { userId });
                    }
                }

                var addresses = await _cartApiClient.GetUserAddressesAsync(userId);
                var defaultAddress = addresses?.FirstOrDefault(a => a.IsDefault) ?? addresses?.FirstOrDefault();

                decimal shippingFee = 0m;
                if (cart.SubtotalAmount < 500000m)
                {
                    if (defaultAddress != null && (defaultAddress.Province.Contains("Hồ Chí Minh") || defaultAddress.Province.Contains("Hà Nội")))
                        shippingFee = 30000m;
                    else
                        shippingFee = 50000m;
                }
                cart.ShippingFee = shippingFee;

                // Apply coupon if provided
                if (!string.IsNullOrWhiteSpace(couponCode))
                {
                    cart.CouponCode = couponCode;
                    var couponResult = await _couponApiClient.ApplyCouponAsync(userId, couponCode);
                    if (couponResult != null && couponResult.Success)
                    {
                        cart.DiscountAmount = couponResult.DiscountAmount;
                        cart.FinalAmount = cart.SubtotalAmount - cart.DiscountAmount + cart.ShippingFee;
                        TempData["CouponSuccess"] = couponResult.Message ?? "Áp mã giảm giá thành công!";
                    }
                    else
                    {
                        cart.DiscountAmount = 0m;
                        cart.FinalAmount = cart.SubtotalAmount + cart.ShippingFee;
                        TempData["Error"] = couponResult?.Message ?? "Mã giảm giá không hợp lệ";
                        cart.CouponCode = null;
                    }
                }
                else
                {
                    cart.DiscountAmount = 0m;
                    cart.FinalAmount = cart.SubtotalAmount + cart.ShippingFee;
                }

                foreach (var a in addresses ?? new List<SHOP.CO.MVC.Models.UserAddressViewModel>())
                {
                    a.AddressType = a.IsDefault ? "Home (Default)" :
                        (a.ReceiverName.ToLower().Contains("công ty") || a.StreetAddress.ToLower().Contains("lầu") || a.StreetAddress.ToLower().Contains("tầng")) ? "Office" : "Other";
                }

                ViewBag.Addresses = addresses;
                return View(cart);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message ?? "Đã xảy ra lỗi khi tải trang thanh toán.";
                return RedirectToAction("Index", new { userId });
            }
        }

        /// <summary>
        /// Step 1 of checkout: Validate inputs, send OTP, store pending checkout info in Session.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceOrder(int userId, string? couponCode, int addressId, string? customerNote, string paymentMethod)
        {
            userId = GetUserIdOrDefault(userId);
            try
            {
                if (addressId <= 0)
                {
                    TempData["Error"] = "Vui lòng chọn địa chỉ giao hàng.";
                    return RedirectToAction("Checkout", new { userId, couponCode });
                }

                if (string.IsNullOrWhiteSpace(paymentMethod))
                {
                    TempData["Error"] = "Vui lòng chọn phương thức thanh toán.";
                    return RedirectToAction("Checkout", new { userId, couponCode });
                }

                // Store pending checkout data in Session
                HttpContext.Session.SetString("PendingAddressId", addressId.ToString());
                HttpContext.Session.SetString("PendingCouponCode", couponCode ?? "");
                HttpContext.Session.SetString("PendingCustomerNote", customerNote ?? "");
                HttpContext.Session.SetString("PendingPaymentMethod", paymentMethod);

                // Send OTP to user's email
                var (otpSent, otpMsg) = await _cartApiClient.SendCheckoutOtpAsync(userId);
                if (!otpSent)
                {
                    TempData["Error"] = otpMsg.Length > 0 ? otpMsg : "Không thể gửi mã OTP. Vui lòng thử lại.";
                    return RedirectToAction("Checkout", new { userId, couponCode });
                }

                TempData["OtpInfo"] = "Mã OTP đã được gửi vào email của bạn. Mã có hiệu lực trong 5 phút.";
                return RedirectToAction("VerifyOTP", new { userId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message ?? "Đã xảy ra lỗi trong quá trình đặt hàng.";
                return RedirectToAction("Checkout", new { userId, couponCode });
            }
        }

        /// <summary>
        /// Show OTP verification page.
        /// </summary>
        [HttpGet]
        public IActionResult VerifyOTP(int userId = 1)
        {
            userId = GetUserIdOrDefault(userId);
            // Check if pending checkout exists
            var pendingAddress = HttpContext.Session.GetString("PendingAddressId");
            if (string.IsNullOrEmpty(pendingAddress))
            {
                TempData["Error"] = "Phiên đặt hàng đã hết hạn. Vui lòng thực hiện lại.";
                return RedirectToAction("Checkout", new { userId });
            }
            ViewBag.UserId = userId;
            return View();
        }

        /// <summary>
        /// Step 2 of checkout: Verify OTP, then call checkout API to create order.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmOrder(int userId, string otpCode)
        {
            userId = GetUserIdOrDefault(userId);
            try
            {
                if (string.IsNullOrWhiteSpace(otpCode) || otpCode.Trim().Length != 6)
                {
                    TempData["Error"] = "Mã OTP không hợp lệ. Vui lòng nhập đủ 6 chữ số.";
                    return RedirectToAction("VerifyOTP", new { userId });
                }

                // Retrieve pending checkout from Session
                var pendingAddressStr = HttpContext.Session.GetString("PendingAddressId");
                var couponCode = HttpContext.Session.GetString("PendingCouponCode");
                var customerNote = HttpContext.Session.GetString("PendingCustomerNote");
                var paymentMethod = HttpContext.Session.GetString("PendingPaymentMethod");

                if (string.IsNullOrEmpty(pendingAddressStr) || !int.TryParse(pendingAddressStr, out int addressId))
                {
                    TempData["Error"] = "Phiên đặt hàng đã hết hạn. Vui lòng thực hiện lại từ đầu.";
                    return RedirectToAction("Checkout", new { userId });
                }

                // Clear session
                HttpContext.Session.Remove("PendingAddressId");
                HttpContext.Session.Remove("PendingCouponCode");
                HttpContext.Session.Remove("PendingCustomerNote");
                HttpContext.Session.Remove("PendingPaymentMethod");

                // Call checkout API with OTP
                var result = await _cartApiClient.CheckoutAsync(
                    userId,
                    string.IsNullOrWhiteSpace(couponCode) ? null : couponCode,
                    addressId,
                    customerNote,
                    paymentMethod,
                    otpCode.Trim());

                if (result == null)
                {
                    TempData["Error"] = "Đặt hàng thất bại. Không thể tạo đơn hàng.";
                    return RedirectToAction("Checkout", new { userId });
                }

                // Process payment
                var returnUrl = $"{Request.Scheme}://{Request.Host}/Payment/VnpayReturn?orderId={result.OrderId}&orderCode={Uri.EscapeDataString(result.OrderCode)}&totalAmount={result.TotalAmount}";
                var payResult = await _paymentApiClient.ProcessPaymentAsync(result.OrderId, paymentMethod ?? "COD", returnUrl);

                if (payResult == null || !payResult.Success)
                {
                    TempData["Error"] = payResult?.Message ?? "Xử lý thanh toán thất bại.";
                    return RedirectToAction("Index", "Payment", new { orderId = result.OrderId, orderCode = result.OrderCode, totalAmount = result.TotalAmount });
                }

                if ((paymentMethod ?? "").Equals("COD", StringComparison.OrdinalIgnoreCase))
                {
                    return RedirectToAction("Success", "Payment", new
                    {
                        orderCode = payResult.OrderCode ?? result.OrderCode,
                        paymentStatus = payResult.PaymentStatus ?? "Unpaid",
                        paymentMethod = "COD"
                    });
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(payResult.PaymentUrl))
                        return Redirect(payResult.PaymentUrl);

                    TempData["Error"] = "Không thể tạo URL thanh toán VNPay.";
                    return RedirectToAction("Index", "Payment", new { orderId = result.OrderId, orderCode = result.OrderCode, totalAmount = result.TotalAmount });
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message ?? "Đã xảy ra lỗi khi xác nhận đơn hàng.";
                return RedirectToAction("VerifyOTP", new { userId });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ApplyCoupon(string couponCode, int userId = 1)
        {
            userId = GetUserIdOrDefault(userId);
            return RedirectToAction("Index", new { userId, couponCode });
        }

        [HttpGet]
        public IActionResult Success(int orderId, string orderCode, string orderStatus, decimal totalAmount, DateTime createdAt)
        {
            var model = new CheckoutResultViewModel
            {
                OrderId = orderId,
                OrderCode = orderCode,
                OrderStatus = orderStatus,
                TotalAmount = totalAmount,
                CreatedAt = createdAt
            };
            return View(model);
        }

        private string GetProductImageUrl(string productName)
        {
            if (string.IsNullOrWhiteSpace(productName)) return "/images/p2.jpg";
            var name = productName.ToLower();
            if (name.Contains("thun") || name.Contains("t-shirt")) return "/images/topsellingimg1.png";
            if (name.Contains("váy") || name.Contains("dress")) return "/images/newarrivalimg2.png";
            if (name.Contains("hoodie") || name.Contains("khoác")) return "/images/newarrivalimg3.png";
            if (name.Contains("jeans") || name.Contains("quần")) return "/images/p2.jpg";
            if (name.Contains("sơ mi") || name.Contains("shirt")) return "/images/topsellingimg4.png";
            return "/images/p2.jpg";
        }
    }
}




