using SHOP.CO.MVC.Models;

namespace SHOP.CO.MVC.Services
{
    public interface ICartApiClient
    {
        Task<CartViewModel> GetCartAsync(int userId);
        Task<bool> RemoveFromCartAsync(int cartItemId, int userId);
        Task<bool> AddToCartAsync(int userId, int variantId, int quantity);
    }
}
