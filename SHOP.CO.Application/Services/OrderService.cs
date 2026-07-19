using System;
using System.Linq;
using System.Threading.Tasks;
using SHOP.CO.Application.DTOs;
using SHOP.CO.Domain.Entities;
using SHOP.CO.Infrastructure.Repositories;

namespace SHOP.CO.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly SHOP.CO.Infrastructure.Repositories.IOrderRepository _orderRepository;
        private readonly ICartRepository _cartRepository;
        private readonly ICommerceRecordRepository _commerceRecordRepository;

        public OrderService(SHOP.CO.Infrastructure.Repositories.IOrderRepository orderRepository, ICartRepository cartRepository, ICommerceRecordRepository commerceRecordRepository)
        {
            _orderRepository = orderRepository;
            _cartRepository = cartRepository;
            _commerceRecordRepository = commerceRecordRepository;
        }

        public async Task<CheckoutResponseDto> CheckoutAsync(CheckoutRequestDto requestDto)
        {
            if (requestDto == null || requestDto.UserId <= 0)
            {
                throw new ArgumentException("Invalid user details.");
            }

            // 1. Get user details
            var user = await _orderRepository.GetUserByIdAsync(requestDto.UserId);
            if (user == null)
            {
                throw new InvalidOperationException("User not found.");
            }

            // 2. Load all CartItems for user
            var cartItems = await _cartRepository.GetCartByUserIdAsync(requestDto.UserId);

            // 3. Validate cart is not empty
            if (cartItems == null || !cartItems.Any())
            {
                throw new InvalidOperationException("Cart is empty.");
            }

            // 4. Determine shipping details from AddressId or default
            UserAddress? selectedAddress = null;
            if (requestDto.AddressId.HasValue && requestDto.AddressId.Value > 0)
            {
                selectedAddress = user.UserAddresses?.FirstOrDefault(a => a.AddressId == requestDto.AddressId.Value && a.DeletedAt == null);
                if (selectedAddress == null)
                {
                    throw new InvalidOperationException("Selected shipping address not found.");
                }
            }
            else
            {
                selectedAddress = user.UserAddresses?.FirstOrDefault(a => a.IsDefault && a.DeletedAt == null)
                                  ?? user.UserAddresses?.FirstOrDefault(a => a.DeletedAt == null);
            }

            if (selectedAddress == null)
            {
                throw new InvalidOperationException("Shipping address is required to place an order.");
            }

            string receiverName = selectedAddress.ReceiverName;
            string receiverPhone = selectedAddress.ReceiverPhone;
            string shippingAddressText = $"{selectedAddress.StreetAddress}, {selectedAddress.Ward}, {selectedAddress.District}, {selectedAddress.Province}";

            // 5. Calculate subtotal
            decimal subtotal = cartItems.Sum(item => item.Quantity * item.UnitPrice);
            if (subtotal < 0m)
            {
                throw new InvalidOperationException("Subtotal cannot be negative.");
            }

            // 6. Calculate coupon discount
            decimal discount = 0m;
            CommerceRecord? coupon = null;

            if (!string.IsNullOrWhiteSpace(requestDto.CouponCode))
            {
                coupon = await _commerceRecordRepository.GetCouponByCodeAsync(requestDto.CouponCode);
                if (coupon == null)
                {
                    throw new InvalidOperationException("Coupon not found");
                }

                if (!string.Equals(coupon.Status, "Active", StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException("Coupon inactive");
                }

                var now = DateTime.UtcNow;
                if (coupon.StartAt.HasValue && now < coupon.StartAt.Value)
                {
                    throw new InvalidOperationException("Coupon not started yet");
                }

                if (coupon.EndAt.HasValue && now > coupon.EndAt.Value)
                {
                    throw new InvalidOperationException("Coupon expired");
                }

                if (coupon.UsageLimit.HasValue && coupon.UsedCount >= coupon.UsageLimit.Value)
                {
                    throw new InvalidOperationException("Coupon usage limit reached");
                }

                if (coupon.MinOrderAmount.HasValue && subtotal < coupon.MinOrderAmount.Value)
                {
                    throw new InvalidOperationException($"Minimum order amount not satisfied. Required: {coupon.MinOrderAmount.Value:N0}");
                }

                // Calculate discount value
                if (string.Equals(coupon.DiscountType, "PERCENT", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(coupon.DiscountType, "Percent", StringComparison.OrdinalIgnoreCase))
                {
                    if (coupon.DiscountValue.HasValue)
                    {
                        discount = subtotal * coupon.DiscountValue.Value / 100m;
                    }
                    if (coupon.MaxDiscountAmount.HasValue && coupon.MaxDiscountAmount.Value > 0)
                    {
                        discount = Math.Min(discount, coupon.MaxDiscountAmount.Value);
                    }
                }
                else if (string.Equals(coupon.DiscountType, "FIXED", StringComparison.OrdinalIgnoreCase) ||
                         string.Equals(coupon.DiscountType, "Fixed", StringComparison.OrdinalIgnoreCase))
                {
                    if (coupon.DiscountValue.HasValue)
                    {
                        discount = coupon.DiscountValue.Value;
                    }
                }

                discount = Math.Min(discount, subtotal);
                if (discount < 0m)
                {
                    discount = 0m;
                }
            }

            // 7. Shipping fee calculation
            decimal shippingFee = 0m;
            if (subtotal < 500000m)
            {
                if (selectedAddress.Province.Contains("Hồ Chí Minh") || selectedAddress.Province.Contains("Hà Nội"))
                    shippingFee = 30000m;
                else
                    shippingFee = 50000m;
            }

            if (shippingFee < 0m)
            {
                throw new InvalidOperationException("Shipping fee cannot be negative.");
            }

            decimal totalAmount = subtotal - discount + shippingFee;
            if (totalAmount < 0m)
            {
                totalAmount = 0m;
            }

            // 8. Generate order code and validate uniqueness
            string orderCode = "ORD-" + Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();
            var existingOrder = await _orderRepository.GetOrderByCodeAsync(orderCode);
            if (existingOrder != null)
            {
                throw new InvalidOperationException("Generated order code is not unique, please try again.");
            }

            // 9. Construct Order entity
            var order = new Order
            {
                UserId = user.UserId,
                OrderCode = orderCode,
                AddressId = selectedAddress.AddressId,
                ReceiverName = receiverName,
                ReceiverPhone = receiverPhone,
                ShippingAddressText = shippingAddressText,
                OrderStatus = "Pending",
                PaymentStatus = "Unpaid",
                ShippingStatus = "NotShipped",
                SubtotalAmount = subtotal,
                DiscountAmount = discount,
                ShippingFee = shippingFee,
                TotalAmount = totalAmount,
                CustomerNote = requestDto.CustomerNote,
                CreatedAt = DateTime.UtcNow
            };

            // 10. Execute validation and DB persistence inside a single strict transaction block
            await _orderRepository.ExecuteInTransactionAsync(async () =>
            {
                // Re-validate variant activity & stock under transaction to avoid race conditions
                foreach (var item in cartItems)
                {
                    var variant = await _cartRepository.GetProductVariantByIdAsync(item.VariantId);
                    if (variant == null)
                    {
                        throw new InvalidOperationException("Product variant details are missing or variant has been deleted.");
                    }

                    if (!variant.IsActive || variant.Product == null || !variant.Product.IsActive)
                    {
                        throw new InvalidOperationException($"Product variant for '{variant.Sku}' is inactive.");
                    }

                    if (item.Quantity > variant.StockQuantity)
                    {
                        throw new InvalidOperationException($"Insufficient stock for '{variant.Product.ProductName}'. Available: {variant.StockQuantity}, requested: {item.Quantity}.");
                    }

                    var orderItem = new OrderItem
                    {
                        ProductId = variant.ProductId,
                        VariantId = item.VariantId,
                        ProductNameSnapshot = variant.Product.ProductName,
                        SkuSnapshot = variant.Sku,
                        SizeSnapshot = item.SelectedSize ?? variant.Size,
                        ColorSnapshot = item.SelectedColor ?? variant.Color,
                        ImageUrlSnapshot = "",
                        UnitPrice = item.UnitPrice,
                        Quantity = item.Quantity,
                        DiscountAmount = 0m,
                        LineTotal = item.Quantity * item.UnitPrice,
                        ReviewStatus = "NotReviewed",
                        CreatedAt = DateTime.UtcNow
                    };

                    order.OrderItems.Add(orderItem);

                    // Reduce variant stock
                    variant.StockQuantity -= item.Quantity;
                    variant.UpdatedAt = DateTime.UtcNow;
                    await _cartRepository.UpdateCartItemAsync(item); // triggers state change tracking for variant in DbContext
                }

                // Add order to database
                await _orderRepository.AddOrderAsync(order);
                await _orderRepository.SaveChangesAsync();

                // Clear cart items
                foreach (var item in cartItems)
                {
                    await _cartRepository.RemoveCartItemAsync(item.CartItemId);
                }
                await _cartRepository.SaveChangesAsync();

                // Increment coupon usage
                if (coupon != null)
                {
                    coupon.UsedCount += 1;
                    coupon.UpdatedAt = DateTime.UtcNow;
                    await _commerceRecordRepository.SaveChangesAsync();
                }
            });

            return new CheckoutResponseDto
            {
                OrderId = order.OrderId,
                OrderCode = order.OrderCode,
                OrderStatus = order.OrderStatus,
                TotalAmount = order.TotalAmount,
                CreatedAt = order.CreatedAt
            };
        }

        public async Task<List<OrderDto>> GetOrdersByUserIdAsync(int userId, string? status = null, string? search = null)
        {
            var orders = await _orderRepository.GetOrdersByUserIdAsync(userId, status, search);
            var result = new List<OrderDto>();
            foreach (var o in orders)
            {
                result.Add(MapToDto(o));
            }
            return result;
        }

        public async Task<OrderDto?> GetOrderByIdAsync(int orderId)
        {
            var order = await _orderRepository.GetOrderByIdAsync(orderId);
            if (order == null) return null;
            return MapToDto(order);
        }

        public async Task<bool> CancelOrderAsync(int orderId, int userId, string cancelReason)
        {
            // 1. Order must exist
            var order = await _orderRepository.GetOrderWithItemsAndVariantsByIdAsync(orderId);
            if (order == null)
            {
                throw new KeyNotFoundException("Order not found.");
            }

            // 2. User can only cancel own order
            if (order.UserId != userId)
            {
                throw new UnauthorizedAccessException("You are not authorized to cancel this order.");
            }

            // 3. Only Pending / Confirmed / Processing orders can be canceled (not Shipping/Delivered/Canceled)
            var cancellableStatuses = new[] { "Pending", "Confirmed", "Processing" };
            bool isCancellable = Array.Exists(
                cancellableStatuses,
                s => string.Equals(s, order.OrderStatus, StringComparison.OrdinalIgnoreCase));

            if (!isCancellable)
            {
                throw new InvalidOperationException("Only orders before shipping can be canceled.");
            }

            // 4. When cancel, restore ProductVariant.StockQuantity
            // 5. Update order status: Pending -> Canceled
            // 6. Use transaction
            await _orderRepository.ExecuteInTransactionAsync(async () =>
            {
                order.OrderStatus = "Canceled";
                order.CancelReason = cancelReason;
                order.CanceledAt = DateTime.UtcNow;
                order.UpdatedAt = DateTime.UtcNow;

                foreach (var item in order.OrderItems)
                {
                    if (item.ProductVariant != null)
                    {
                        item.ProductVariant.StockQuantity += item.Quantity;
                        item.ProductVariant.UpdatedAt = DateTime.UtcNow;
                    }
                }

                await _orderRepository.SaveChangesAsync();
            });

            return true;
        }

        public async Task<OrderTrackingDto?> GetOrderTrackingAsync(int orderId)
        {
            var order = await _orderRepository.GetOrderByIdAsync(orderId);
            if (order == null) return null;

            var status = order.OrderStatus ?? string.Empty;
            var steps = GenerateTrackingSteps(status, order.CreatedAt, order.CanceledAt, order.CompletedAt);

            return new OrderTrackingDto
            {
                OrderId = order.OrderId,
                OrderCode = order.OrderCode ?? string.Empty,
                OrderStatus = order.OrderStatus ?? string.Empty,
                PaymentStatus = order.PaymentStatus ?? string.Empty,
                TotalAmount = order.TotalAmount,
                CreatedAt = order.CreatedAt,
                CompletedAt = order.CompletedAt,
                Steps = steps
            };
        }

        private List<OrderTrackingStepDto> GenerateTrackingSteps(
            string status,
            DateTime createdAt,
            DateTime? canceledAt,
            DateTime? completedAt)
        {
            var normalized = status.Trim().ToLower();

            // Canceled flow: Order Created → Canceled
            if (normalized == "canceled")
            {
                return new List<OrderTrackingStepDto>
                {
                    new OrderTrackingStepDto
                    {
                        Status = "Order Created",
                        Description = "Your order has been placed successfully.",
                        Completed = true,
                        Time = createdAt
                    },
                    new OrderTrackingStepDto
                    {
                        Status = "Canceled",
                        Description = "The order has been canceled.",
                        Completed = true,
                        Time = canceledAt
                    }
                };
            }

            // Normal flow: Order Created → Confirmed → Processing → Shipping → Delivered
            var flowStatuses = new[]
            {
                "order created",
                "confirmed",
                "processing",
                "shipping",
                "delivered"
            };

            // Map order status to index in the flow
            var currentIndex = normalized switch
            {
                "pending"   => 0,
                "confirmed" => 1,
                "processing"=> 2,
                "shipping"  => 3,
                "delivered" => 4,
                _           => 0
            };

            var descriptions = new[]
            {
                "Your order has been placed successfully.",
                "Your order has been confirmed by the seller.",
                "Your order is being packed and prepared.",
                "Your order is on its way to you.",
                "Your order has been delivered successfully."
            };

            var steps = new List<OrderTrackingStepDto>();
            for (int i = 0; i < flowStatuses.Length; i++)
            {
                bool completed = i <= currentIndex;
                DateTime? time = null;

                // Assign timestamps based on position
                if (i == 0) time = createdAt;
                else if (i == 4 && completed) time = completedAt;

                steps.Add(new OrderTrackingStepDto
                {
                    Status = flowStatuses[i] == "order created" ? "Order Created" :
                             char.ToUpper(flowStatuses[i][0]) + flowStatuses[i].Substring(1),
                    Description = descriptions[i],
                    Completed = completed,
                    Time = time
                });
            }

            return steps;
        }

        private OrderDto MapToDto(Order o)
        {
            var dto = new OrderDto
            {
                OrderId = o.OrderId,
                UserId = o.UserId,
                OrderCode = o.OrderCode,
                AddressId = o.AddressId,
                ReceiverName = o.ReceiverName,
                ReceiverPhone = o.ReceiverPhone,
                ShippingAddressText = o.ShippingAddressText,
                OrderStatus = o.OrderStatus,
                PaymentStatus = o.PaymentStatus,
                ShippingStatus = o.ShippingStatus,
                SubtotalAmount = o.SubtotalAmount,
                DiscountAmount = o.DiscountAmount,
                ShippingFee = o.ShippingFee,
                TotalAmount = o.TotalAmount,
                CustomerNote = o.CustomerNote,
                StaffNote = o.StaffNote,
                CancelReason = o.CancelReason,
                CreatedAt = o.CreatedAt,
                UpdatedAt = o.UpdatedAt,
                CompletedAt = o.CompletedAt,
                CanceledAt = o.CanceledAt,
                OrderItems = new List<OrderItemDto>()
            };

            if (o.OrderItems != null)
            {
                foreach (var item in o.OrderItems)
                {
                    dto.OrderItems.Add(new OrderItemDto
                    {
                        OrderItemId = item.OrderItemId,
                        OrderId = item.OrderId,
                        ProductId = item.ProductId,
                        VariantId = item.VariantId,
                        ProductNameSnapshot = item.ProductNameSnapshot,
                        SkuSnapshot = item.SkuSnapshot,
                        SizeSnapshot = item.SizeSnapshot,
                        ColorSnapshot = item.ColorSnapshot,
                        ImageUrlSnapshot = item.ImageUrlSnapshot,
                        UnitPrice = item.UnitPrice,
                        SalePrice = item.SalePrice,
                        Quantity = item.Quantity,
                        DiscountAmount = item.DiscountAmount,
                        LineTotal = item.LineTotal,
                        ReviewStatus = item.ReviewStatus,
                        CreatedAt = item.CreatedAt
                    });
                }
            }

            return dto;
        }
        public async Task<bool> SubmitReturnRequestAsync(int orderId, int userId, string reason, string description)
        {
            // 1. Order must exist
            var order = await _orderRepository.GetOrderByIdAsync(orderId);
            if (order == null)
            {
                throw new KeyNotFoundException("Order not found.");
            }

            // 2. User can only request return for own order
            if (order.UserId != userId)
            {
                throw new UnauthorizedAccessException("You are not authorized to request a return for this order.");
            }

            // 3. Only Completed (Delivered) orders can be returned
            if (!string.Equals(order.OrderStatus, "Completed", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Only completed orders can be returned.");
            }

            // 4. Reason & description must not be empty
            if (string.IsNullOrWhiteSpace(reason))
            {
                throw new ArgumentException("Return reason is required.");
            }
            if (string.IsNullOrWhiteSpace(description))
            {
                throw new ArgumentException("Return description is required.");
            }

            // 5. Database limitation: no ReturnRequests table / no ReturnStatus column.
            //    Record the return reason and description in CustomerNote (the only available nullable text field)
            //    as a best-effort approach within the existing schema.
            await _orderRepository.ExecuteInTransactionAsync(async () =>
            {
                order.CustomerNote = $"[RETURN REQUEST] {DateTime.UtcNow:yyyy-MM-dd HH:mm} UTC — Reason: {reason}. Description: {description}";
                order.UpdatedAt = DateTime.UtcNow;
                await _orderRepository.SaveChangesAsync();
            });

            return true;
        }

        public async Task<List<UserAddressDto>> GetUserAddressesAsync(int userId)
        {
            if (userId <= 0)
            {
                throw new ArgumentException("Invalid user ID.");
            }

            var user = await _orderRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found.");
            }

            var result = new List<UserAddressDto>();
            if (user.UserAddresses != null)
            {
                foreach (var a in user.UserAddresses.Where(a => a.DeletedAt == null))
                {
                    result.Add(new UserAddressDto
                    {
                        AddressId = a.AddressId,
                        UserId = a.UserId,
                        ReceiverName = a.ReceiverName,
                        ReceiverPhone = a.ReceiverPhone,
                        Province = a.Province,
                        District = a.District,
                        Ward = a.Ward,
                        StreetAddress = a.StreetAddress,
                        PostalCode = a.PostalCode,
                        IsDefault = a.IsDefault,
                        CreatedAt = a.CreatedAt
                    });
                }
            }

            return result;
        }

    public Task<IEnumerable<Order>> GetOrderHistoryAsync(int userId) => throw new NotImplementedException();
    public Task<Order> GetOrderDetailAsync(int orderId, int userId) => throw new NotImplementedException();
    }
}
