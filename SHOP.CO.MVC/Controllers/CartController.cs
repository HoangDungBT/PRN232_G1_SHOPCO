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

        public CartController(ICartApiClient cartApiClient)
        {
            _cartApiClient = cartApiClient;
        }

        /// <summary>
        /// Display user's shopping cart by calling GET /api/cart/{userId}
        /// </summary>
        /// <param name="userId">Optional user id; defaults to 1 for demo</param>
        public async Task<IActionResult> Index(int userId = 1)
        {
            try
            {
                var cart = await _cartApiClient.GetCartAsync(userId);
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
    }
}


