using Microsoft.EntityFrameworkCore;
using System;

namespace SHOP.CO.Infrastructure.Data
{
    public static class ModelBuilderExtension
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            // Sử dụng ngày giờ cố định để tránh EF Core liên tục tạo migration mới do khác biệt tick time
            var fixedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            // 1. Users
            modelBuilder.Entity<User>().HasData(
                new User { UserId = 1, FullName = "Admin System", Email = "admin@shop.co", PasswordHash = "hashed123", Role = "Admin", Status = "Active", CreatedAt = fixedDate },
                new User { UserId = 2, FullName = "Nguyễn Văn A", Email = "nguyenvana@gmail.com", PasswordHash = "hashed123", Role = "Customer", Status = "Active", CreatedAt = fixedDate },
                new User { UserId = 3, FullName = "Trần Thị B", Email = "tranthib@gmail.com", PasswordHash = "hashed123", Role = "Customer", Status = "Active", CreatedAt = fixedDate }
            );

            // 2. UserAddresses (Bổ sung Province, District, Ward)
            modelBuilder.Entity<UserAddress>().HasData(
                new UserAddress { AddressId = 1, UserId = 2, ReceiverName = "Nguyễn Văn A", ReceiverPhone = "0901234567", Province = "TP.HCM", District = "Quận 1", Ward = "Phường Bến Nghé", StreetAddress = "123 Lê Lợi", IsDefault = true, CreatedAt = fixedDate },
                new UserAddress { AddressId = 2, UserId = 2, ReceiverName = "Nguyễn Văn A (Công ty)", ReceiverPhone = "0901234567", Province = "TP.HCM", District = "Quận 1", Ward = "Phường Bến Nghé", StreetAddress = "456 Nguyễn Huệ", IsDefault = false, CreatedAt = fixedDate },
                new UserAddress { AddressId = 3, UserId = 3, ReceiverName = "Trần Thị B", ReceiverPhone = "0987654321", Province = "TP.HCM", District = "Quận 3", Ward = "Phường 6", StreetAddress = "789 Hai Bà Trưng", IsDefault = true, CreatedAt = fixedDate }
            );

            // 3. Categories
            modelBuilder.Entity<Category>().HasData(
                new Category { CategoryId = 1, CategoryName = "Thời trang Nam", Slug = "thoi-trang-nam", IsActive = true, CreatedAt = fixedDate },
                new Category { CategoryId = 2, CategoryName = "Thời trang Nữ", Slug = "thoi-trang-nu", IsActive = true, CreatedAt = fixedDate },
                new Category { CategoryId = 3, ParentCategoryId = 1, CategoryName = "Áo Thun Nam", Slug = "ao-thun-nam", IsActive = true, CreatedAt = fixedDate }
            );

            // 4. Products
            modelBuilder.Entity<Product>().HasData(
                new Product { ProductId = 1, CategoryId = 3, ProductName = "Áo Thun Cổ Tròn Basic", Slug = "ao-thun-co-tron-basic", BasePrice = 150000, SalePrice = 120000, IsActive = true, CreatedAt = fixedDate },
                new Product { ProductId = 2, CategoryId = 2, ProductName = "Váy Hoa Mùa Hè", Slug = "vay-hoa-mua-he", BasePrice = 350000, SalePrice = 300000, IsActive = true, CreatedAt = fixedDate },
                new Product { ProductId = 3, CategoryId = 1, ProductName = "Áo Khoác Hoodie Unisex", Slug = "ao-khoac-hoodie-unisex", BasePrice = 400000, SalePrice = 400000, IsActive = true, CreatedAt = fixedDate }
            );

            // 5. ProductVariants (Bổ sung Sku)
            modelBuilder.Entity<ProductVariant>().HasData(
                new ProductVariant { VariantId = 1, ProductId = 1, Sku = "ATB-TR-M", Size = "M", Color = "Trắng", StockQuantity = 50, IsActive = true, CreatedAt = fixedDate },
                new ProductVariant { VariantId = 2, ProductId = 1, Sku = "ATB-DE-L", Size = "L", Color = "Đen", StockQuantity = 30, IsActive = true, CreatedAt = fixedDate },
                new ProductVariant { VariantId = 3, ProductId = 2, Sku = "VHM-DO-S", Size = "S", Color = "Đỏ", StockQuantity = 20, IsActive = true, CreatedAt = fixedDate },
                new ProductVariant { VariantId = 4, ProductId = 3, Sku = "AKH-XA-XL", Size = "XL", Color = "Xám", StockQuantity = 100, IsActive = true, CreatedAt = fixedDate }
            );

            // 6. ProductImages
            modelBuilder.Entity<ProductImage>().HasData(
                new ProductImage { ImageId = 1, ProductId = 1, ImageUrl = "/images/ao-thun-trang.jpg", IsThumbnail = true, CreatedAt = fixedDate },
                new ProductImage { ImageId = 2, ProductId = 2, ImageUrl = "/images/vay-hoa.jpg", IsThumbnail = true, CreatedAt = fixedDate },
                new ProductImage { ImageId = 3, ProductId = 3, ImageUrl = "/images/hoodie-xam.jpg", IsThumbnail = true, CreatedAt = fixedDate }
            );

            // 7. CartItems
            modelBuilder.Entity<CartItem>().HasData(
                new CartItem { CartItemId = 1, UserId = 2, VariantId = 1, Quantity = 2, UnitPrice = 120000, IsSelected = true, CreatedAt = fixedDate },
                new CartItem { CartItemId = 2, UserId = 2, VariantId = 3, Quantity = 1, UnitPrice = 300000, IsSelected = true, CreatedAt = fixedDate },
                new CartItem { CartItemId = 3, UserId = 3, VariantId = 4, Quantity = 1, UnitPrice = 400000, IsSelected = true, CreatedAt = fixedDate }
            );

            // 8. Orders (Bổ sung ReceiverName, ReceiverPhone, ShippingAddressText, Statuses)
            modelBuilder.Entity<Order>().HasData(
                new Order { OrderId = 1, UserId = 2, OrderCode = "ORD001", ReceiverName = "Nguyễn Văn A", ReceiverPhone = "0901234567", ShippingAddressText = "123 Lê Lợi, Quận 1, TP.HCM", OrderStatus = "Completed", PaymentStatus = "Paid", ShippingStatus = "Delivered", TotalAmount = 240000, CreatedAt = fixedDate },
                new Order { OrderId = 2, UserId = 2, OrderCode = "ORD002", ReceiverName = "Nguyễn Văn A", ReceiverPhone = "0901234567", ShippingAddressText = "456 Nguyễn Huệ, Quận 1, TP.HCM", OrderStatus = "Pending", PaymentStatus = "Unpaid", ShippingStatus = "NotShipped", TotalAmount = 300000, CreatedAt = fixedDate },
                new Order { OrderId = 3, UserId = 3, OrderCode = "ORD003", ReceiverName = "Trần Thị B", ReceiverPhone = "0987654321", ShippingAddressText = "789 Hai Bà Trưng, Quận 3, TP.HCM", OrderStatus = "Shipping", PaymentStatus = "Paid", ShippingStatus = "Shipping", TotalAmount = 400000, CreatedAt = fixedDate }
            );

            // 9. OrderItems (Bổ sung SkuSnapshot)
            modelBuilder.Entity<OrderItem>().HasData(
                new OrderItem { OrderItemId = 1, OrderId = 1, ProductId = 1, VariantId = 1, ProductNameSnapshot = "Áo Thun Cổ Tròn Basic", SkuSnapshot = "ATB-TR-M", Quantity = 2, UnitPrice = 150000, SalePrice = 120000, LineTotal = 240000, ReviewStatus = "NotReviewed", CreatedAt = fixedDate },
                new OrderItem { OrderItemId = 2, OrderId = 2, ProductId = 2, VariantId = 3, ProductNameSnapshot = "Váy Hoa Mùa Hè", SkuSnapshot = "VHM-DO-S", Quantity = 1, UnitPrice = 350000, SalePrice = 300000, LineTotal = 300000, ReviewStatus = "NotReviewed", CreatedAt = fixedDate },
                new OrderItem { OrderItemId = 3, OrderId = 3, ProductId = 3, VariantId = 4, ProductNameSnapshot = "Áo Khoác Hoodie Unisex", SkuSnapshot = "AKH-XA-XL", Quantity = 1, UnitPrice = 400000, SalePrice = 400000, LineTotal = 400000, ReviewStatus = "NotReviewed", CreatedAt = fixedDate }
            );

            // 10. CommerceRecords 
            modelBuilder.Entity<CommerceRecord>().HasData(
                new CommerceRecord { RecordId = 1, OrderId = 1, UserId = 2, RecordType = "Payment", Amount = 240000, Status = "Success", CreatedAt = fixedDate },
                new CommerceRecord { RecordId = 2, OrderId = 2, UserId = 2, RecordType = "VoucherUsage", Code = "GIAM10K", DiscountValue = 10000, Status = "Applied", CreatedAt = fixedDate },
                new CommerceRecord { RecordId = 3, OrderId = 3, UserId = 3, RecordType = "Payment", Amount = 400000, Status = "Success", CreatedAt = fixedDate }
            );

            // 11. CustomerActivities
            modelBuilder.Entity<CustomerActivity>().HasData(
                new CustomerActivity { ActivityId = 1, UserId = 2, ProductId = 1, ActivityType = "Review", Rating = 5, Comment = "Áo mặc mát mẻ, đẹp", IsActive = true, CreatedAt = fixedDate },
                new CustomerActivity { ActivityId = 2, UserId = 3, ProductId = 2, ActivityType = "Wishlist", IsActive = true, CreatedAt = fixedDate },
                new CustomerActivity { ActivityId = 3, UserId = 2, ActivityType = "Search", Keyword = "Áo khoác mùa đông", IsActive = true, CreatedAt = fixedDate }
            );

            // 12. InteractionLogs
            modelBuilder.Entity<InteractionLog>().HasData(
                new InteractionLog { LogId = 1, UserId = 2, LogType = "Chatbot", SenderType = "User", Message = "Cho tôi hỏi size áo thun", IsRead = true, CreatedAt = fixedDate },
                new InteractionLog { LogId = 2, UserId = 2, LogType = "Chatbot", SenderType = "Bot", Message = "Dạ, size M phù hợp với người từ 50-60kg ạ.", IsRead = true, CreatedAt = fixedDate },
                new InteractionLog { LogId = 3, UserId = 3, LogType = "Notification", Title = "Đơn hàng đang giao", Message = "Đơn hàng ORD003 của bạn đang được giao", IsRead = false, CreatedAt = fixedDate }
            );
        }
    }
}