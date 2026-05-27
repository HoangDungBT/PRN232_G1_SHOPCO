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

            // ==========================================
            // 1. Users
            // ==========================================
            modelBuilder.Entity<User>().HasData(
                new User { UserId = 1, FullName = "Admin System", Email = "admin@shop.co", PasswordHash = "hashed123", Role = "Admin", Status = "Active", CreatedAt = fixedDate },
                new User { UserId = 2, FullName = "Nguyễn Văn A", Email = "nguyenvana@gmail.com", PasswordHash = "hashed123", Role = "Customer", Status = "Active", Gender = "Nam", CreatedAt = fixedDate },
                new User { UserId = 3, FullName = "Trần Thị B", Email = "tranthib@gmail.com", PasswordHash = "hashed123", Role = "Customer", Status = "Active", Gender = "Nữ", CreatedAt = fixedDate }
            );

            // ==========================================
            // 2. UserAddresses
            // ==========================================
            modelBuilder.Entity<UserAddress>().HasData(
                new UserAddress { AddressId = 1, UserId = 2, ReceiverName = "Nguyễn Văn A", ReceiverPhone = "0901234567", Province = "TP.HCM", District = "Quận 1", Ward = "Phường Bến Nghé", StreetAddress = "123 Lê Lợi", IsDefault = true, CreatedAt = fixedDate },
                new UserAddress { AddressId = 2, UserId = 2, ReceiverName = "Nguyễn Văn A (Công ty)", ReceiverPhone = "0901234567", Province = "TP.HCM", District = "Quận 1", Ward = "Phường Bến Nghé", StreetAddress = "456 Nguyễn Huệ", IsDefault = false, CreatedAt = fixedDate },
                new UserAddress { AddressId = 3, UserId = 3, ReceiverName = "Trần Thị B", ReceiverPhone = "0987654321", Province = "TP.HCM", District = "Quận 3", Ward = "Phường 6", StreetAddress = "789 Hai Bà Trưng", IsDefault = true, CreatedAt = fixedDate }
            );

            // ==========================================
            // 3. Categories
            // ==========================================
            modelBuilder.Entity<Category>().HasData(
                new Category { CategoryId = 1, CategoryName = "Thời trang Nam", Slug = "thoi-trang-nam", IsActive = true, CreatedAt = fixedDate },
                new Category { CategoryId = 2, CategoryName = "Thời trang Nữ", Slug = "thoi-trang-nu", IsActive = true, CreatedAt = fixedDate },
                new Category { CategoryId = 3, CategoryName = "Unisex", Slug = "thoi-trang-unisex", IsActive = true, CreatedAt = fixedDate },
                new Category { CategoryId = 4, ParentCategoryId = 1, CategoryName = "Áo Thun Nam", Slug = "ao-thun-nam", IsActive = true, CreatedAt = fixedDate },
                new Category { CategoryId = 5, ParentCategoryId = 1, CategoryName = "Quần Jeans Nam", Slug = "quan-jeans-nam", IsActive = true, CreatedAt = fixedDate },
                new Category { CategoryId = 6, ParentCategoryId = 2, CategoryName = "Váy Nữ", Slug = "vay-nu", IsActive = true, CreatedAt = fixedDate },
                new Category { CategoryId = 7, ParentCategoryId = 2, CategoryName = "Áo Sơ Mi Nữ", Slug = "ao-so-mi-nu", IsActive = true, CreatedAt = fixedDate }
            );

            // ==========================================
            // 4. Products (Tối thiểu 5 sản phẩm)
            // ==========================================
            modelBuilder.Entity<Product>().HasData(
                new Product { ProductId = 1, CategoryId = 4, ProductName = "Áo Thun Cổ Tròn Basic", Slug = "ao-thun-co-tron-basic", Brand = "Shop.Co", Material = "Cotton", BasePrice = 150000, SalePrice = 120000, AverageRating = 4.5m, ReviewCount = 10, IsActive = true, IsBestSeller = true, CreatedAt = fixedDate },
                new Product { ProductId = 2, CategoryId = 6, ProductName = "Váy Hoa Mùa Hè", Slug = "vay-hoa-mua-he", Brand = "Shop.Co", Material = "Voan", BasePrice = 350000, SalePrice = 300000, AverageRating = 4.8m, ReviewCount = 25, IsActive = true, IsNewArrival = true, CreatedAt = fixedDate },
                new Product { ProductId = 3, CategoryId = 3, ProductName = "Áo Khoác Hoodie Unisex", Slug = "ao-khoac-hoodie-unisex", Brand = "Shop.Co", Material = "Nỉ", BasePrice = 400000, SalePrice = 400000, AverageRating = 4.2m, ReviewCount = 5, IsActive = true, IsFeatured = true, CreatedAt = fixedDate },
                new Product { ProductId = 4, CategoryId = 5, ProductName = "Quần Jeans Nam Slimfit", Slug = "quan-jeans-nam-slimfit", Brand = "DenimX", Material = "Denim", BasePrice = 450000, SalePrice = 399000, AverageRating = 4.9m, ReviewCount = 50, IsActive = true, IsBestSeller = true, CreatedAt = fixedDate },
                new Product { ProductId = 5, CategoryId = 7, ProductName = "Áo Sơ Mi Lụa Công Sở", Slug = "ao-so-mi-lua-cong-so", Brand = "OfficeWear", Material = "Lụa", BasePrice = 250000, SalePrice = null, AverageRating = 4.0m, ReviewCount = 2, IsActive = true, CreatedAt = fixedDate }
            );

            // ==========================================
            // 5. ProductVariants
            // ==========================================
            modelBuilder.Entity<ProductVariant>().HasData(
                // SP 1: Áo thun (2 biến thể)
                new ProductVariant { VariantId = 1, ProductId = 1, Sku = "ATB-TR-M", Size = "M", Color = "Trắng", ColorHex = "#FFFFFF", StockQuantity = 50, LowStockThreshold = 5, IsActive = true, CreatedAt = fixedDate },
                new ProductVariant { VariantId = 2, ProductId = 1, Sku = "ATB-DE-L", Size = "L", Color = "Đen", ColorHex = "#000000", StockQuantity = 30, LowStockThreshold = 5, IsActive = true, CreatedAt = fixedDate },
                // SP 2: Váy (1 biến thể)
                new ProductVariant { VariantId = 3, ProductId = 2, Sku = "VHM-DO-S", Size = "S", Color = "Đỏ", ColorHex = "#FF0000", StockQuantity = 20, LowStockThreshold = 2, IsActive = true, CreatedAt = fixedDate },
                // SP 3: Áo Khoác (1 biến thể)
                new ProductVariant { VariantId = 4, ProductId = 3, Sku = "AKH-XA-XL", Size = "XL", Color = "Xám", ColorHex = "#808080", StockQuantity = 100, LowStockThreshold = 10, IsActive = true, CreatedAt = fixedDate },
                // SP 4: Quần Jeans (2 biến thể)
                new ProductVariant { VariantId = 5, ProductId = 4, Sku = "QJN-XANH-30", Size = "30", Color = "Xanh Denim", ColorHex = "#1560BD", StockQuantity = 40, LowStockThreshold = 5, IsActive = true, CreatedAt = fixedDate },
                new ProductVariant { VariantId = 6, ProductId = 4, Sku = "QJN-XANH-31", Size = "31", Color = "Xanh Đậm", ColorHex = "#00008B", StockQuantity = 3, LowStockThreshold = 5, IsActive = true, CreatedAt = fixedDate },
                // SP 5: Áo sơ mi (1 biến thể)
                new ProductVariant { VariantId = 7, ProductId = 5, Sku = "ASM-TR-M", Size = "M", Color = "Trắng", ColorHex = "#FFFFFF", StockQuantity = 15, LowStockThreshold = 5, IsActive = true, CreatedAt = fixedDate }
            );

            // ==========================================
            // 6. ProductImages
            // ==========================================
            modelBuilder.Entity<ProductImage>().HasData(
                new ProductImage { ImageId = 1, ProductId = 1, VariantId = 1, ImageUrl = "https://via.placeholder.com/600x800.png?text=Ao+Thun+Trang", IsThumbnail = true, SortOrder = 1, CreatedAt = fixedDate },
                new ProductImage { ImageId = 2, ProductId = 1, VariantId = 2, ImageUrl = "https://via.placeholder.com/600x800.png?text=Ao+Thun+Den", IsThumbnail = false, SortOrder = 2, CreatedAt = fixedDate },
                new ProductImage { ImageId = 3, ProductId = 2, ImageUrl = "https://via.placeholder.com/600x800.png?text=Vay+Hoa", IsThumbnail = true, SortOrder = 1, CreatedAt = fixedDate },
                new ProductImage { ImageId = 4, ProductId = 3, ImageUrl = "https://via.placeholder.com/600x800.png?text=Hoodie+Xam", IsThumbnail = true, SortOrder = 1, CreatedAt = fixedDate },
                new ProductImage { ImageId = 5, ProductId = 4, VariantId = 5, ImageUrl = "https://via.placeholder.com/600x800.png?text=Jeans+Denim", IsThumbnail = true, SortOrder = 1, CreatedAt = fixedDate },
                new ProductImage { ImageId = 6, ProductId = 4, VariantId = 6, ImageUrl = "https://via.placeholder.com/600x800.png?text=Jeans+Dam", IsThumbnail = false, SortOrder = 2, CreatedAt = fixedDate },
                new ProductImage { ImageId = 7, ProductId = 5, ImageUrl = "https://via.placeholder.com/600x800.png?text=So+Mi+Trang", IsThumbnail = true, SortOrder = 1, CreatedAt = fixedDate }
            );

            // ==========================================
            // 7. CartItems
            // ==========================================
            modelBuilder.Entity<CartItem>().HasData(
                // User 2 có 2 sản phẩm trong giỏ
                new CartItem { CartItemId = 1, UserId = 2, VariantId = 1, Quantity = 2, UnitPrice = 120000, SelectedSize = "M", SelectedColor = "Trắng", IsSelected = true, CreatedAt = fixedDate },
                new CartItem { CartItemId = 2, UserId = 2, VariantId = 5, Quantity = 1, UnitPrice = 399000, SelectedSize = "30", SelectedColor = "Xanh Denim", IsSelected = true, CreatedAt = fixedDate },
                // User 3 có 1 sản phẩm chưa chọn checkout
                new CartItem { CartItemId = 3, UserId = 3, VariantId = 7, Quantity = 1, UnitPrice = 250000, SelectedSize = "M", SelectedColor = "Trắng", IsSelected = false, CreatedAt = fixedDate }
            );

            // ==========================================
            // 8. Orders
            // ==========================================
            modelBuilder.Entity<Order>().HasData(
                new Order { OrderId = 1, UserId = 2, OrderCode = "ORD-0001", AddressId = 1, ReceiverName = "Nguyễn Văn A", ReceiverPhone = "0901234567", ShippingAddressText = "123 Lê Lợi, Quận 1, TP.HCM", OrderStatus = "Completed", PaymentStatus = "Paid", ShippingStatus = "Completed", SubtotalAmount = 240000, TotalAmount = 240000, CreatedAt = fixedDate },
                new Order { OrderId = 2, UserId = 2, OrderCode = "ORD-0002", AddressId = 2, ReceiverName = "Nguyễn Văn A (Công ty)", ReceiverPhone = "0901234567", ShippingAddressText = "456 Nguyễn Huệ, Quận 1, TP.HCM", OrderStatus = "Pending", PaymentStatus = "Unpaid", ShippingStatus = "NotShipped", SubtotalAmount = 399000, DiscountAmount = 19000, TotalAmount = 380000, CreatedAt = fixedDate },
                new Order { OrderId = 3, UserId = 3, OrderCode = "ORD-0003", AddressId = 3, ReceiverName = "Trần Thị B", ReceiverPhone = "0987654321", ShippingAddressText = "789 Hai Bà Trưng, Quận 3, TP.HCM", OrderStatus = "Shipping", PaymentStatus = "Paid", ShippingStatus = "Shipping", SubtotalAmount = 300000, TotalAmount = 300000, CreatedAt = fixedDate },
                new Order { OrderId = 4, UserId = 3, OrderCode = "ORD-0004", AddressId = 3, ReceiverName = "Trần Thị B", ReceiverPhone = "0987654321", ShippingAddressText = "789 Hai Bà Trưng, Quận 3, TP.HCM", OrderStatus = "Canceled", PaymentStatus = "Refunded", ShippingStatus = "NotShipped", SubtotalAmount = 400000, TotalAmount = 400000, CancelReason = "Đổi ý", CreatedAt = fixedDate }
            );

            // ==========================================
            // 9. OrderItems
            // ==========================================
            modelBuilder.Entity<OrderItem>().HasData(
                new OrderItem { OrderItemId = 1, OrderId = 1, ProductId = 1, VariantId = 1, ProductNameSnapshot = "Áo Thun Cổ Tròn Basic", SkuSnapshot = "ATB-TR-M", SizeSnapshot = "M", ColorSnapshot = "Trắng", ImageUrlSnapshot = "https://via.placeholder.com/600x800.png?text=Ao+Thun+Trang", Quantity = 2, UnitPrice = 150000, SalePrice = 120000, LineTotal = 240000, ReviewStatus = "Reviewed", CreatedAt = fixedDate },
                new OrderItem { OrderItemId = 2, OrderId = 2, ProductId = 4, VariantId = 5, ProductNameSnapshot = "Quần Jeans Nam Slimfit", SkuSnapshot = "QJN-XANH-30", SizeSnapshot = "30", ColorSnapshot = "Xanh Denim", ImageUrlSnapshot = "https://via.placeholder.com/600x800.png?text=Jeans+Denim", Quantity = 1, UnitPrice = 450000, SalePrice = 399000, LineTotal = 399000, ReviewStatus = "NotReviewed", CreatedAt = fixedDate },
                new OrderItem { OrderItemId = 3, OrderId = 3, ProductId = 2, VariantId = 3, ProductNameSnapshot = "Váy Hoa Mùa Hè", SkuSnapshot = "VHM-DO-S", SizeSnapshot = "S", ColorSnapshot = "Đỏ", ImageUrlSnapshot = "https://via.placeholder.com/600x800.png?text=Vay+Hoa", Quantity = 1, UnitPrice = 350000, SalePrice = 300000, LineTotal = 300000, ReviewStatus = "NotReviewed", CreatedAt = fixedDate },
                new OrderItem { OrderItemId = 4, OrderId = 4, ProductId = 3, VariantId = 4, ProductNameSnapshot = "Áo Khoác Hoodie Unisex", SkuSnapshot = "AKH-XA-XL", SizeSnapshot = "XL", ColorSnapshot = "Xám", ImageUrlSnapshot = "https://via.placeholder.com/600x800.png?text=Hoodie+Xam", Quantity = 1, UnitPrice = 400000, SalePrice = 400000, LineTotal = 400000, ReviewStatus = "NotReviewed", CreatedAt = fixedDate }
            );

            // ==========================================
            // 10. CommerceRecords (Payment, Voucher...)
            // ==========================================
            modelBuilder.Entity<CommerceRecord>().HasData(
                new CommerceRecord { RecordId = 1, OrderId = 1, UserId = 2, RecordType = "Payment", PaymentMethod = "Online", PaymentProvider = "VNPay", TransactionCode = "VNP123456", Amount = 240000, Status = "Success", CreatedAt = fixedDate },
                new CommerceRecord { RecordId = 2, OrderId = 2, UserId = 2, RecordType = "VoucherUsage", Code = "FREESHIP19K", DiscountType = "Fixed", DiscountValue = 19000, Status = "Applied", CreatedAt = fixedDate },
                new CommerceRecord { RecordId = 3, OrderId = 3, UserId = 3, RecordType = "Payment", PaymentMethod = "COD", Amount = 300000, Status = "Pending", CreatedAt = fixedDate },
                new CommerceRecord { RecordId = 4, RecordType = "Voucher", Code = "WELCOME50", Name = "Giảm 50K cho thành viên mới", DiscountType = "Fixed", DiscountValue = 50000, MinOrderAmount = 200000, UsageLimit = 1000, UsedCount = 150, Status = "Active", StartAt = fixedDate, EndAt = fixedDate.AddMonths(1), CreatedAt = fixedDate },
                new CommerceRecord { RecordId = 5, RecordType = "FlashSale", Name = "Sale 11.11", Status = "Active", PayloadJson = "{\"discountPercent\": 10}", StartAt = fixedDate, EndAt = fixedDate.AddDays(1), CreatedAt = fixedDate }
            );

            // ==========================================
            // 11. CustomerActivities
            // ==========================================
            modelBuilder.Entity<CustomerActivity>().HasData(
                new CustomerActivity { ActivityId = 1, UserId = 2, ProductId = 1, OrderItemId = 1, ActivityType = "Review", Rating = 5, Comment = "Áo chất lượng rất tốt, form chuẩn.", IsActive = true, CreatedAt = fixedDate },
                new CustomerActivity { ActivityId = 2, UserId = 3, ProductId = 5, ActivityType = "Wishlist", IsActive = true, CreatedAt = fixedDate },
                new CustomerActivity { ActivityId = 3, UserId = 2, ActivityType = "Search", Keyword = "quần jeans nam", IsActive = true, CreatedAt = fixedDate },
                new CustomerActivity { ActivityId = 4, UserId = 3, ProductId = 2, ActivityType = "RecentlyViewed", IsActive = true, CreatedAt = fixedDate },
                new CustomerActivity { ActivityId = 5, UserId = 3, ActivityType = "AiColorSearch", InputJson = "{\"imageUrl\": \"/uploads/user-search-1.jpg\"}", ResultJson = "{\"dominantColor\": \"#FF0000\", \"matchScore\": 95}", IsActive = true, CreatedAt = fixedDate }
            );

            // ==========================================
            // 12. InteractionLogs
            // ==========================================
            modelBuilder.Entity<InteractionLog>().HasData(
                new InteractionLog { LogId = 1, UserId = 2, SessionId = "sess_123", LogType = "Chatbot", SenderType = "User", Message = "Cho tôi hỏi quần jeans size 30 còn hàng không?", IsRead = true, CreatedAt = fixedDate },
                new InteractionLog { LogId = 2, UserId = 2, SessionId = "sess_123", LogType = "Chatbot", SenderType = "Bot", Message = "Dạ, quần jeans nam slimfit size 30 hiện còn 40 sản phẩm ạ.", IntentName = "check_stock", EntitiesJson = "{\"product\": \"quần jeans\", \"size\": \"30\"}", IsRead = true, CreatedAt = fixedDate },
                new InteractionLog { LogId = 3, UserId = 3, OrderId = 3, LogType = "Notification", Title = "Đơn hàng đang giao", Message = "Đơn hàng ORD-0003 của bạn đã được giao cho đơn vị vận chuyển.", IsRead = false, Status = "Sent", CreatedAt = fixedDate },
                new InteractionLog { LogId = 4, UserId = 1, ProductId = 4, VariantId = 6, LogType = "StockAlert", Title = "Cảnh báo tồn kho thấp", Message = "Biến thể QJN-XANH-31 chỉ còn 3 sản phẩm trong kho.", SenderType = "System", IsRead = false, CreatedAt = fixedDate },
                new InteractionLog { LogId = 5, UserId = 1, LogType = "Audit", ActionName = "UpdateProductPrice", Title = "Cập nhật giá sản phẩm", OldValueJson = "{\"price\": 400000}", NewValueJson = "{\"price\": 399000}", SenderType = "Admin", CreatedAt = fixedDate }
            );
        }
    }
}
