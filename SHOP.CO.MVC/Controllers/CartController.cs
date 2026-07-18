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

        /// <summary>
        /// Display user's shopping cart by calling GET /api/cart/{userId}
        /// </summary>
        /// <param name="userId">Optional user id; defaults to 1 for demo</param>
        public async Task<IActionResult> Index(int userId = 1, string? couponCode = null)
        {
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
                        cart.FinalAmount = cart.TotalAmount + cart.ShippingFee;
                        TempData["Error"] = couponResult?.Message ?? "Invalid coupon";
                    }
                }
                else
                {
                    cart.DiscountAmount = 0m;
                    cart.FinalAmount = cart.TotalAmount + cart.ShippingFee;
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
        public async Task<IActionResult> AddToCart(int id, string name, decimal price, string image, int userId = 1)
        {
            try
            {
                var success = await _cartApiClient.AddToCartAsync(userId, id, 1);
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
            try
            {
                var cart = await _cartApiClient.GetCartAsync(userId);
                if (cart == null || cart.Items == null || !cart.Items.Any())
                {
                    TempData["Error"] = "Your cart is empty. Please add items to checkout.";
                    return RedirectToAction("Index", new { userId });
                }

                // Check for stock warning
                foreach (var item in cart.Items)
                {
                    item.ImageUrl = GetProductImageUrl(item.ProductName);
                    if (item.Quantity > item.AvailableStock)
                    {
                        TempData["Error"] = $"Some items in your cart exceed available stock. Please update quantity before checking out.";
                        return RedirectToAction("Index", new { userId, couponCode });
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
                        cart.FinalAmount = cart.TotalAmount + cart.ShippingFee;
                        TempData["Error"] = couponResult?.Message ?? "Invalid coupon";
                    }
                }
                else
                {
                    cart.DiscountAmount = 0m;
                    cart.FinalAmount = cart.TotalAmount + cart.ShippingFee;
                }

                var addresses = await _cartApiClient.GetUserAddressesAsync(userId);
                foreach (var a in addresses)
                {
                    if (a.IsDefault)
                    {
                        a.AddressType = "Home (Default)";
                    }
                    else if (a.ReceiverName.ToLower().Contains("công ty") || a.ReceiverName.ToLower().Contains("office") || a.ReceiverName.ToLower().Contains("co") || a.StreetAddress.ToLower().Contains("lầu") || a.StreetAddress.ToLower().Contains("tầng"))
                    {
                        a.AddressType = "Office";
                    }
                    else
                    {
                        a.AddressType = "Other";
                    }
                }

                ViewBag.Addresses = addresses;
                return View(cart);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message ?? "An error occurred while loading checkout page.";
                return RedirectToAction("Index", new { userId, couponCode });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceOrder(int userId, string? couponCode, int addressId, string? customerNote, string paymentMethod)
        {
            try
            {
                if (addressId <= 0)
                {
                    TempData["Error"] = "Please select a shipping address.";
                    return RedirectToAction("Checkout", new { userId, couponCode });
                }

                if (string.IsNullOrWhiteSpace(paymentMethod))
                {
                    TempData["Error"] = "Please select a payment method.";
                    return RedirectToAction("Checkout", new { userId, couponCode });
                }

                var result = await _cartApiClient.CheckoutAsync(userId, couponCode, addressId, customerNote, paymentMethod);
                if (result == null)
                {
                    TempData["Error"] = "Checkout failed. Order could not be created.";
                    return RedirectToAction("Checkout", new { userId, couponCode });
                }

                var returnUrl = $"{Request.Scheme}://{Request.Host}/Payment/VnpayReturn?orderId={result.OrderId}&orderCode={Uri.EscapeDataString(result.OrderCode)}&totalAmount={result.TotalAmount}";
                var payResult = await _paymentApiClient.ProcessPaymentAsync(result.OrderId, paymentMethod, returnUrl);

                if (payResult == null || !payResult.Success)
                {
                    TempData["Error"] = payResult?.Message ?? "Payment processing failed.";
                    return RedirectToAction("Index", "Payment", new { orderId = result.OrderId, orderCode = result.OrderCode, totalAmount = result.TotalAmount });
                }

                if (paymentMethod.Equals("COD", StringComparison.OrdinalIgnoreCase))
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
                    {
                        return Redirect(payResult.PaymentUrl);
                    }
                    TempData["Error"] = "Failed to generate VNPay payment URL.";
                    return RedirectToAction("Index", "Payment", new { orderId = result.OrderId, orderCode = result.OrderCode, totalAmount = result.TotalAmount });
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message ?? "An error occurred during order placement.";
                return RedirectToAction("Checkout", new { userId, couponCode });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ApplyCoupon(string couponCode, int userId = 1)
        {
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




