using Microsoft.EntityFrameworkCore;

namespace SHOP.CO.Infrastructure.Repositories
{
    /* public interface ICartRepository
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

    /// <summary>
    /// CartRepository - implements ICartRepository for cart data access
    /// </summary>
    */ public class CartRepository : ICartRepository
    {
        private readonly ShopCoDbContext _context;

        /// <summary>
        /// Constructor - Dependency injection of DbContext
        /// </summary>
        /// <param name="context">ShopCoDbContext instance</param>
        public CartRepository(ShopCoDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Add a new item to the shopping cart
        /// </summary>
        /// <param name="cartItem">The CartItem entity to add</param>
        /// <returns>Completed task</returns>
        public async Task AddToCartAsync(CartItem cartItem)
        {
            await _context.CartItems.AddAsync(cartItem);
            await SaveChangesAsync();
        }

        /// <summary>
        /// Retrieve all cart items for a specific user with related product variant data
        /// Includes: ProductVariant with related Product information
        /// </summary>
        /// <param name="userId">The user ID to filter by</param>
        /// <returns>List of CartItem entities with fully loaded navigation properties</returns>
        public async Task<List<CartItem>> GetCartByUserIdAsync(int userId)
        {
            var cartItems = await _context.CartItems
                .Where(c => c.UserId == userId)
                .Include(c => c.ProductVariant)
                    .ThenInclude(v => v.Product)
                        .ThenInclude(p => p.ProductImages)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            return cartItems;
        }

        /// <summary>
        /// Retrieve a specific cart item by user ID and product variant ID
        /// Useful for checking if an item already exists in cart before adding duplicate
        /// Includes: ProductVariant with related Product information
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <param name="variantId">The product variant ID</param>
        /// <returns>CartItem if found; null if not found</returns>
        public async Task<CartItem?> GetCartItemAsync(int userId, int variantId)
        {
            var cartItem = await _context.CartItems
                .Where(c => c.UserId == userId && c.VariantId == variantId)
                .Include(c => c.ProductVariant)
                    .ThenInclude(v => v.Product)
                        .ThenInclude(p => p.ProductImages)
                .Include(c => c.User)
                .FirstOrDefaultAsync();

            return cartItem;
        }

        /// <summary>
        /// Remove a cart item from the database by its ID
        /// </summary>
        /// <param name="cartItemId">The cart item ID to remove</param>
        /// <returns>Completed task</returns>
        public async Task RemoveCartItemAsync(int cartItemId)
        {
            var cartItem = await _context.CartItems.FindAsync(cartItemId);

            if (cartItem != null)
            {
                _context.CartItems.Remove(cartItem);
                await SaveChangesAsync();
            }
        }

        /// <summary>
        /// Retrieve product variant by ID with related product details
        /// </summary>
        public async Task<ProductVariant?> GetProductVariantByIdAsync(int variantId)
        {
            return await _context.ProductVariants
                .Include(pv => pv.Product)
                    .ThenInclude(p => p.ProductImages)
                .FirstOrDefaultAsync(pv => pv.VariantId == variantId);
        }

        /// <summary>
        /// Retrieve cart item by ID
        /// </summary>
        public async Task<CartItem?> GetCartItemByIdAsync(int cartItemId)
        {
            return await _context.CartItems.FindAsync(cartItemId);
        }

        /// <summary>
        /// Retrieve the first active variant of a product
        /// </summary>
        public async Task<ProductVariant?> GetFirstVariantByProductIdAsync(int productId)
        {
            return await _context.ProductVariants
                .Include(pv => pv.Product)
                .Where(pv => pv.ProductId == productId && pv.IsActive)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Save all pending changes to the database
        /// </summary>
        /// <returns>Completed task</returns>
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
        public async Task UpdateCartItemAsync(CartItem item)
        {
            _context.CartItems.Update(item);
            await SaveChangesAsync();
        }
    }
}


