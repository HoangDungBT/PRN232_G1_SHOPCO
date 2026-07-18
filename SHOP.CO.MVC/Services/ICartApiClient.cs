using SHOP.CO.MVC.Models;

namespace SHOP.CO.MVC.Services
{
    public interface ICartApiClient
    {
        Task<CartViewModel> GetCartAsync(int userId);
        Task<bool> RemoveFromCartAsync(int cartItemId, int userId);
        Task<bool> AddToCartAsync(int userId, int variantId, int quantity);
        Task<bool> UpdateQuantityAsync(int cartItemId, int userId, int quantity);
        Task<CheckoutResultViewModel?> CheckoutAsync(int userId, string? couponCode, int? addressId = null, string? customerNote = null, string? paymentMethod = null);
        Task<System.Collections.Generic.List<UserAddressViewModel>> GetUserAddressesAsync(int userId);
    }
}
