using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SHOP.CO.Application.Services
{
    /// <summary>
    /// Interface for Product Service - defines contract for product operations
    /// </summary>
    public interface IProductService
    {
        Task<PagedResult<ProductDto>> GetProductsAsync(string? searchTerm, int pageNumber, int pageSize);
        IQueryable<ProductDto> GetProductsQuery();
        Task<ProductDto?> GetProductByIdAsync(int id);
        Task<List<ProductDto>> GetRelatedProductsAsync(int productId, int limit);
    }

    /// <summary>
    /// Interface for Cart Service - defines contract for shopping cart operations
    /// </summary>
    public interface ICartService
    {
        /// <summary>
        /// Add or update item in shopping cart
        /// Increases quantity if item already exists, creates new cart item if not
        /// </summary>
        /// <param name="userId">User ID who owns the cart</param>
        /// <param name="requestDto">Contains VariantId, Quantity, and optional Size/Color</param>
        /// <returns>Updated CartItemDto with current state</returns>
        Task<CartItemDto> AddToCartAsync(int userId, AddToCartRequestDto requestDto);

        /// <summary>
        /// Retrieve all items in a user's shopping cart
        /// Includes full product and variant information
        /// </summary>
        /// <param name="userId">User ID to fetch cart for</param>
        /// <returns>Complete cart with all items</returns>
        Task<CartDto> GetCartByUserIdAsync(int userId);

        /// <summary>
        /// Remove a specific item from the shopping cart
        /// </summary>
        /// <param name="cartItemId">Cart item ID to remove</param>
        /// <param name="userId">User ID for authorization (optional validation)</param>
        /// <returns>Task representing the asynchronous operation</returns>
        Task RemoveFromCartAsync(int cartItemId, int userId);

        /// <summary>
        /// Remove all items from a user's shopping cart
        /// </summary>
        /// <param name="userId">User ID whose cart to clear</param>
        /// <returns>Task representing the asynchronous operation</returns>
        Task ClearCartAsync(int userId);
    }
}
