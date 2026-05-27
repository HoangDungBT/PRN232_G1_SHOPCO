using Microsoft.AspNetCore.Mvc;
using SHOP.CO.MVC.Models;
using System.Text.Json;

namespace SHOP.CO.MVC.Controllers
{
    public class CartController : Controller
    {
        public IActionResult Index()
        {
            var cart = GetCart();

            return View(cart);
        }

        public IActionResult AddToCart(
            int id,
            string name,
            decimal price,
            string image)
        {
            var cart = GetCart();

            var existingItem = cart.FirstOrDefault(x => x.Id == id);

            if (existingItem != null)
            {
                existingItem.Quantity++;
            }
            else
            {
                cart.Add(new CartItemVM
                {
                    Id = id,
                    Name = name,
                    Price = price,
                    Image = image,
                    Quantity = 1
                });
            }

            SaveCart(cart);

            return RedirectToAction("Index");
        }

        public IActionResult Remove(int id)
        {
            var cart = GetCart();

            var item = cart.FirstOrDefault(x => x.Id == id);

            if (item != null)
            {
                cart.Remove(item);
            }

            SaveCart(cart);

            return RedirectToAction("Index");
        }

        private List<CartItemVM> GetCart()
        {
            var session = HttpContext.Session.GetString("CART");

            if (session == null)
            {
                return new List<CartItemVM>();
            }

            return JsonSerializer.Deserialize<List<CartItemVM>>(session);
        }

        private void SaveCart(List<CartItemVM> cart)
        {
            HttpContext.Session.SetString(
                "CART",
                JsonSerializer.Serialize(cart));
        }
    }
}