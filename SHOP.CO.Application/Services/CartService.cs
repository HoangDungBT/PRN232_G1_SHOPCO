namespace SHOP.CO.Application.Services
{
    public interface ICartService
    {
        Task<CartItemDto> AddToCartAsync(int userId, AddToCartRequestDto requestDto);
        Task<CartDto> GetCartByUserIdAsync(int userId);
        Task RemoveFromCartAsync(int cartItemId, int userId);
        Task ClearCartAsync(int userId);
    }

    /// <summary>
    /// CartService - implements ICartService for shopping cart business logic
    /// Handles add, remove, and retrieval of cart items with business rules
    /// </summary>
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;

        /// <summary>
        /// Constructor - Dependency injection of cart repository
        /// </summary>
        /// <param name="cartRepository">Repository for cart data access</param>
        public CartService(ICartRepository cartRepository)
        {
            _cartRepository = cartRepository;
        }

        /// <summary>
        /// Add or update item in shopping cart with business rule validation
        /// 
        /// Business Rules:
        /// 1. Check if ProductVariant exists and is active
        /// 2. Check if stock is available
        /// 3. If item already exists in cart: increase quantity
        /// 4. If item doesn't exist: create new cart item
        /// 5. Ensure quantity doesn't exceed available stock
        /// </summary>
        /// <param name="userId">User ID adding to cart</param>
        /// <param name="requestDto">Request containing VariantId, Quantity, Size, Color</param>
        /// <returns>CartItemDto with updated cart item information</returns>
        /// <exception cref="InvalidOperationException">Thrown when variant doesn't exist, is inactive, or stock insufficient</exception>
        public async Task<CartItemDto> AddToCartAsync(int userId, AddToCartRequestDto requestDto)
        {
            if (userId <= 0)
            {
                throw new ArgumentException("Invalid userId.");
            }

            if (requestDto == null)
            {
                throw new ArgumentException("Request body is required.");
            }

            if (requestDto.Quantity <= 0)
            {
                throw new ArgumentException("Quantity must be greater than zero.");
            }

            // Fetch product variant with related product details
            var variant = await _cartRepository.GetProductVariantByIdAsync(requestDto.VariantId);
            if (variant == null)
            {
                // Fallback: Check if requestDto.VariantId is actually a Product ID
                variant = await _cartRepository.GetFirstVariantByProductIdAsync(requestDto.VariantId);
                if (variant == null)
                {
                    throw new InvalidOperationException("Product variant does not exist.");
                }
            }

            if (!variant.IsActive || !variant.Product.IsActive)
            {
                throw new InvalidOperationException("Product variant is inactive.");
            }

            // Calculate UnitPrice based on: (variant.OriginalPrice ?? (variant.Product.SalePrice ?? variant.Product.BasePrice)) + variant.ExtraPrice
            decimal unitPrice = (variant.OriginalPrice ?? (variant.Product.SalePrice ?? variant.Product.BasePrice)) + variant.ExtraPrice;

            var existingCartItem = await _cartRepository.GetCartItemAsync(userId, variant.VariantId);

            if (existingCartItem != null)
            {
                int newQuantity = existingCartItem.Quantity + requestDto.Quantity;

                if (newQuantity > variant.StockQuantity)
                {
                    throw new InvalidOperationException(
                        $"Cannot add {requestDto.Quantity} units. Only {variant.StockQuantity - existingCartItem.Quantity} units available.");
                }

                existingCartItem.Quantity = newQuantity;
                existingCartItem.UnitPrice = unitPrice;
                existingCartItem.UpdatedAt = DateTime.UtcNow;

                await _cartRepository.SaveChangesAsync();

                return MapCartItemToDto(existingCartItem);
            }
            else
            {
                if (requestDto.Quantity > variant.StockQuantity)
                {
                    throw new InvalidOperationException(
                        $"Cannot add {requestDto.Quantity} units. Only {variant.StockQuantity} units available.");
                }

                var cartItem = new CartItem
                {
                    UserId = userId,
                    VariantId = variant.VariantId,
                    Quantity = requestDto.Quantity,
                    UnitPrice = unitPrice,
                    SelectedSize = requestDto.SelectedSize ?? variant.Size,
                    SelectedColor = requestDto.SelectedColor ?? variant.Color,
                    IsSelected = true,
                    CreatedAt = DateTime.UtcNow
                };

                await _cartRepository.AddToCartAsync(cartItem);

                var addedCartItem = await _cartRepository.GetCartItemAsync(userId, variant.VariantId);

                if (addedCartItem == null)
                {
                    throw new InvalidOperationException("Failed to retrieve newly added cart item.");
                }

                return MapCartItemToDto(addedCartItem);
            }
        }

        /// <summary>
        /// Returns:
        /// - All cart items with full product and variant information
        /// - Item count and subtotal calculations
        /// - Timestamp of last update
        /// </summary>
        /// <param name="userId">User ID to fetch cart for</param>
        /// <returns>CartDto containing all items and summary information</returns>
        public async Task<CartDto> GetCartByUserIdAsync(int userId)
        {
            if (userId <= 0)
            {
                throw new ArgumentException("Invalid userId.");
            }

            // Fetch all cart items for user
            var cartItems = await _cartRepository.GetCartByUserIdAsync(userId);

            // Map cart items to DTOs
            var cartItemDtos = cartItems.Select(MapCartItemToDto).ToList();

            // Create and return CartDto
            var cartDto = new CartDto
            {
                UserId = userId,
                Items = cartItemDtos,
                LastUpdated = cartItems.Any() ? cartItems.Max(c => c.UpdatedAt ?? c.CreatedAt) : DateTime.UtcNow
            };

            return cartDto;
        }

        /// <summary>
        /// Remove a specific item from the shopping cart by cart item ID
        /// 
        /// Business Rule 6: Delete product from cart
        /// </summary>
        /// <param name="cartItemId">ID of cart item to remove</param>
        /// <param name="userId">User ID (for authorization in production)</param>
        /// <returns>Task representing the asynchronous operation</returns>
        /// <exception cref="InvalidOperationException">Thrown if cart item not found</exception>
        public async Task RemoveFromCartAsync(int cartItemId, int userId)
        {
            if (cartItemId <= 0)
            {
                throw new ArgumentException("Invalid cartItemId.");
            }

            if (userId <= 0)
            {
                throw new ArgumentException("Invalid userId.");
            }

            var cartItem = await _cartRepository.GetCartItemByIdAsync(cartItemId);
            if (cartItem == null)
            {
                throw new InvalidOperationException("Cart item not found.");
            }

            if (cartItem.UserId != userId)
            {
                throw new UnauthorizedAccessException("You are not authorized to remove this item from the cart.");
            }

            await _cartRepository.RemoveCartItemAsync(cartItemId);
        }

        /// <summary>
        /// Clear all items from a user's shopping cart
        /// </summary>
        /// <param name="userId">User ID whose cart to clear</param>
        /// <returns>Task representing the asynchronous operation</returns>
        public async Task ClearCartAsync(int userId)
        {
            // Fetch all cart items for user
            var cartItems = await _cartRepository.GetCartByUserIdAsync(userId);

            // Remove each item
            foreach (var item in cartItems)
            {
                await _cartRepository.RemoveCartItemAsync(item.CartItemId);
            }
        }

        /// <summary>
        /// Map CartItem entity to CartItemDto for API response
        /// </summary>
        /// <param name="cartItem">CartItem entity from database</param>
        /// <returns>CartItemDto for client response</returns>
        private CartItemDto MapCartItemToDto(CartItem cartItem)
        {
            return new CartItemDto
            {
                CartItemId = cartItem.CartItemId,
                VariantId = cartItem.VariantId,
                ProductId = cartItem.ProductVariant.ProductId,
                ProductName = cartItem.ProductVariant.Product.ProductName,
                Sku = cartItem.ProductVariant.Sku,
                Size = cartItem.ProductVariant.Size,
                Color = cartItem.ProductVariant.Color,
                ColorHex = cartItem.ProductVariant.ColorHex,
                Quantity = cartItem.Quantity,
                UnitPrice = cartItem.UnitPrice,
                SelectedSize = cartItem.SelectedSize,
                SelectedColor = cartItem.SelectedColor,
                IsSelected = cartItem.IsSelected,
                AvailableStock = cartItem.ProductVariant.StockQuantity,
                CreatedAt = cartItem.CreatedAt,
                UpdatedAt = cartItem.UpdatedAt
            };
        }
    }
}
