using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SHOP.CO.Domain.Entities;

namespace SHOP.CO.Infrastructure.Repositories
{
    public interface IProductRepository
    {
        Task<(List<Product> Items, int TotalCount)> GetPagedProductAsync(string? searchTerm, int pageNumber, int pageSize);
        IQueryable<Product> GetProductsQuery();
        Task<Product?> GetProductByIdAsync(int id);
        Task<List<Product>> GetRelatedProductsAsync(int categoryId, int excludeProductId, int limit);
        Task<List<CustomerActivity>> GetReviewsByProductIdAsync(int productId);
        Task AddReviewAsync(CustomerActivity review);
    }

    public interface ICartRepository
    {
        Task AddToCartAsync(CartItem cartItem);
        Task<List<CartItem>> GetCartByUserIdAsync(int userId);
        Task<CartItem?> GetCartItemAsync(int userId, int variantId);
        Task RemoveCartItemAsync(int cartItemId);
        Task<ProductVariant?> GetProductVariantByIdAsync(int variantId);
        Task<CartItem?> GetCartItemByIdAsync(int cartItemId);
        Task<ProductVariant?> GetFirstVariantByProductIdAsync(int productId);
        Task SaveChangesAsync();
    }
}
