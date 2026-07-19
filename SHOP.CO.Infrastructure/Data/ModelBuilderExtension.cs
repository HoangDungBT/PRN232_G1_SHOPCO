using Microsoft.EntityFrameworkCore;

﻿using Microsoft.EntityFrameworkCore;
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
                new Category { CategoryId = 7, ParentCategoryId = 2, CategoryName = "Áo Sơ Mi Nữ", Slug = "ao-so-mi-nu", IsActive = true, CreatedAt = fixedDate },
                new Category { CategoryId = 8, ParentCategoryId = 3, CategoryName = "Giày Thể Thao", Slug = "giay-the-thao", IsActive = true, CreatedAt = fixedDate }
            );

            // ==========================================
            // 4. Products (Tối thiểu 5 sản phẩm)
            // ==========================================
            modelBuilder.Entity<Product>().HasData(
                new Product { ProductId = 1, CategoryId = 4, ProductName = "Áo Thun Cổ Tròn Basic", Slug = "ao-thun-co-tron-basic", Brand = "Shop.Co", Material = "Cotton", BasePrice = 150000, SalePrice = 120000, AverageRating = 4.5m, ReviewCount = 10, IsActive = true, IsBestSeller = true, CreatedAt = fixedDate },
                new Product { ProductId = 2, CategoryId = 6, ProductName = "Váy Hoa Mùa Hè", Slug = "vay-hoa-mua-he", Brand = "Shop.Co", Material = "Voan", BasePrice = 350000, SalePrice = 300000, AverageRating = 4.8m, ReviewCount = 25, IsActive = true, IsNewArrival = true, CreatedAt = fixedDate },
                new Product { ProductId = 3, CategoryId = 3, ProductName = "Áo Khoác Hoodie Unisex", Slug = "ao-khoac-hoodie-unisex", Brand = "Shop.Co", Material = "Nỉ", BasePrice = 400000, SalePrice = 400000, AverageRating = 4.2m, ReviewCount = 5, IsActive = true, IsFeatured = true, CreatedAt = fixedDate },
                new Product { ProductId = 4, CategoryId = 5, ProductName = "Quần Jeans Nam Slimfit", Slug = "quan-jeans-nam-slimfit", Brand = "DenimX", Material = "Denim", BasePrice = 450000, SalePrice = 399000, AverageRating = 4.9m, ReviewCount = 50, IsActive = true, IsBestSeller = true, CreatedAt = fixedDate },
                new Product { ProductId = 5, CategoryId = 7, ProductName = "Áo Sơ Mi Lụa Công Sở", Slug = "ao-so-mi-lua-cong-so", Brand = "OfficeWear", Material = "Lụa", BasePrice = 250000, SalePrice = null, AverageRating = 4.0m, ReviewCount = 2, IsActive = true, CreatedAt = fixedDate },
                
                new Product { ProductId = 6, CategoryId = 8, ProductName = "Giày Thể Thao Nữ Slim Đế 2cm", Slug = "giay-the-thao-nu-slim", Brand = "SlimSport", Material = "Lưới/Da", BasePrice = 450000, SalePrice = 390000, AverageRating = 4.7m, ReviewCount = 14, IsActive = true, IsFeatured = true, CreatedAt = fixedDate },
                new Product { ProductId = 7, CategoryId = 8, ProductName = "Giày Bóng Rổ Cao Cấp Yin Yang", Slug = "giay-bong-ro-yin-yang", Brand = "YinYang", Material = "Lưới thoáng khí", BasePrice = 850000, SalePrice = 790000, AverageRating = 4.9m, ReviewCount = 38, IsActive = true, IsBestSeller = true, CreatedAt = fixedDate },
                new Product { ProductId = 8, CategoryId = 7, ProductName = "Áo Sơ Mi Sọc Xanh Plaid", Slug = "ao-so-mi-soc-xanh-plaid", Brand = "PlaidWear", Material = "Cotton/Plaid", BasePrice = 280000, SalePrice = 250000, AverageRating = 4.4m, ReviewCount = 8, IsActive = true, CreatedAt = fixedDate },
                new Product { ProductId = 9, CategoryId = 4, ProductName = "Áo Phông H&M Pride Collection", Slug = "ao-phong-hm-pride", Brand = "H&M", Material = "Cotton", BasePrice = 190000, SalePrice = null, AverageRating = 4.6m, ReviewCount = 12, IsActive = true, IsNewArrival = true, CreatedAt = fixedDate },
                new Product { ProductId = 10, CategoryId = 6, ProductName = "Váy Hoa Looks Beautiful", Slug = "vay-hoa-looks-beautiful", Brand = "BeautifulWear", Material = "Chiffon", BasePrice = 320000, SalePrice = 299000, AverageRating = 4.8m, ReviewCount = 21, IsActive = true, CreatedAt = fixedDate },
                new Product { ProductId = 11, CategoryId = 4, ProductName = "Áo Len Cổ Cao Half Zip Pink", Slug = "ao-len-co-cao-half-zip", Brand = "CrewClothing", Material = "Len/Acrylic", BasePrice = 480000, SalePrice = 420000, AverageRating = 4.5m, ReviewCount = 7, IsActive = true, CreatedAt = fixedDate },
                new Product { ProductId = 12, CategoryId = 2, ProductName = "Quần Ống Rộng Nanushka Cleo", Slug = "quan-ong-rong-nanushka", Brand = "Nanushka", Material = "Crepe", BasePrice = 520000, SalePrice = null, AverageRating = 4.2m, ReviewCount = 4, IsActive = true, CreatedAt = fixedDate },
                new Product { ProductId = 13, CategoryId = 2, ProductName = "Quần Palazzo Cạp Cao Womens Wide", Slug = "quan-palazzo-cap-cao", Brand = "Palazzo", Material = "Tuyết mưa", BasePrice = 360000, SalePrice = 320000, AverageRating = 4.5m, ReviewCount = 19, IsActive = true, IsBestSeller = true, CreatedAt = fixedDate },
                new Product { ProductId = 14, CategoryId = 5, ProductName = "Quần Jean Basic Ống Rộng Cá Tính", Slug = "quan-jean-basic-ong-rong", Brand = "DenimBasic", Material = "Denim", BasePrice = 390000, SalePrice = 350000, AverageRating = 4.8m, ReviewCount = 31, IsActive = true, IsBestSeller = true, CreatedAt = fixedDate },
                new Product { ProductId = 15, CategoryId = 4, ProductName = "Áo Thun T-Shirt Basic", Slug = "ao-thun-t-shirt-basic", Brand = "Shop.Co", Material = "Cotton", BasePrice = 120000, SalePrice = 99000, AverageRating = 4.3m, ReviewCount = 5, IsActive = true, CreatedAt = fixedDate },
                new Product { ProductId = 16, CategoryId = 6, ProductName = "Váy Đầm SHEIN", Slug = "vay-dam-shein", Brand = "SHEIN", Material = "Polyester", BasePrice = 290000, SalePrice = null, AverageRating = 4.1m, ReviewCount = 3, IsActive = true, CreatedAt = fixedDate },
                new Product { ProductId = 17, CategoryId = 5, ProductName = "Quần Nỉ calca moletom", Slug = "quan-ni-moletom", Brand = "Moletom", Material = "Nỉ da cá", BasePrice = 250000, SalePrice = 199000, AverageRating = 4.6m, ReviewCount = 9, IsActive = true, CreatedAt = fixedDate }
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
                new ProductVariant { VariantId = 7, ProductId = 5, Sku = "ASM-TR-M", Size = "M", Color = "Trắng", ColorHex = "#FFFFFF", StockQuantity = 15, LowStockThreshold = 5, IsActive = true, CreatedAt = fixedDate },
                
                // SP 6: Giày Slim (2 biến thể)
                new ProductVariant { VariantId = 8, ProductId = 6, Sku = "GTT-SLIM-38", Size = "38", Color = "Sọc", ColorHex = "#FF4500", StockQuantity = 50, LowStockThreshold = 5, IsActive = true, CreatedAt = fixedDate },
                new ProductVariant { VariantId = 9, ProductId = 6, Sku = "GTT-SLIM-39", Size = "39", Color = "Sọc", ColorHex = "#FF4500", StockQuantity = 50, LowStockThreshold = 5, IsActive = true, CreatedAt = fixedDate },
                // SP 7: Giày Yin Yang (2 biến thể)
                new ProductVariant { VariantId = 10, ProductId = 7, Sku = "GBR-YY-40", Size = "40", Color = "Yin Yang", ColorHex = "#000000", StockQuantity = 40, LowStockThreshold = 5, IsActive = true, CreatedAt = fixedDate },
                new ProductVariant { VariantId = 11, ProductId = 7, Sku = "GBR-YY-41", Size = "41", Color = "Yin Yang", ColorHex = "#FFFFFF", StockQuantity = 40, LowStockThreshold = 5, IsActive = true, CreatedAt = fixedDate },
                // SP 8: Áo sơ mi Plaid (2 biến thể)
                new ProductVariant { VariantId = 12, ProductId = 8, Sku = "ASM-PL-M", Size = "M", Color = "Kẻ sọc xanh", ColorHex = "#4682B4", StockQuantity = 50, LowStockThreshold = 5, IsActive = true, CreatedAt = fixedDate },
                new ProductVariant { VariantId = 13, ProductId = 8, Sku = "ASM-PL-L", Size = "L", Color = "Kẻ sọc xanh", ColorHex = "#4682B4", StockQuantity = 50, LowStockThreshold = 5, IsActive = true, CreatedAt = fixedDate },
                // SP 9: Áo H&M (2 biến thể)
                new ProductVariant { VariantId = 14, ProductId = 9, Sku = "AT-HM-M", Size = "M", Color = "Pride", ColorHex = "#FF1493", StockQuantity = 60, LowStockThreshold = 5, IsActive = true, CreatedAt = fixedDate },
                new ProductVariant { VariantId = 15, ProductId = 9, Sku = "AT-HM-L", Size = "L", Color = "Pride", ColorHex = "#FF1493", StockQuantity = 60, LowStockThreshold = 5, IsActive = true, CreatedAt = fixedDate },
                // SP 10: Váy Looks Beautiful (2 biến thể)
                new ProductVariant { VariantId = 16, ProductId = 10, Sku = "V-LB-S", Size = "S", Color = "Đỏ", ColorHex = "#8B0000", StockQuantity = 30, LowStockThreshold = 5, IsActive = true, CreatedAt = fixedDate },
                new ProductVariant { VariantId = 17, ProductId = 10, Sku = "V-LB-M", Size = "M", Color = "Đỏ", ColorHex = "#8B0000", StockQuantity = 30, LowStockThreshold = 5, IsActive = true, CreatedAt = fixedDate },
                // SP 11: Áo len (2 biến thể)
                new ProductVariant { VariantId = 18, ProductId = 11, Sku = "AL-HZ-M", Size = "M", Color = "Hồng", ColorHex = "#FFB6C1", StockQuantity = 40, LowStockThreshold = 5, IsActive = true, CreatedAt = fixedDate },
                new ProductVariant { VariantId = 19, ProductId = 11, Sku = "AL-HZ-L", Size = "L", Color = "Hồng", ColorHex = "#FFB6C1", StockQuantity = 40, LowStockThreshold = 5, IsActive = true, CreatedAt = fixedDate },
                // SP 12: Quần Nanushka (2 biến thể)
                new ProductVariant { VariantId = 20, ProductId = 12, Sku = "Q-NS-M", Size = "M", Color = "Be", ColorHex = "#F5F5DC", StockQuantity = 30, LowStockThreshold = 5, IsActive = true, CreatedAt = fixedDate },
                new ProductVariant { VariantId = 21, ProductId = 12, Sku = "Q-NS-L", Size = "L", Color = "Be", ColorHex = "#F5F5DC", StockQuantity = 30, LowStockThreshold = 5, IsActive = true, CreatedAt = fixedDate },
                // SP 13: Quần Palazzo (2 biến thể)
                new ProductVariant { VariantId = 22, ProductId = 13, Sku = "Q-PZ-M", Size = "M", Color = "Đen", ColorHex = "#000000", StockQuantity = 50, LowStockThreshold = 5, IsActive = true, CreatedAt = fixedDate },
                new ProductVariant { VariantId = 23, ProductId = 13, Sku = "Q-PZ-L", Size = "L", Color = "Đen", ColorHex = "#000000", StockQuantity = 50, LowStockThreshold = 5, IsActive = true, CreatedAt = fixedDate },
                // SP 14: Quần Jean Basic (2 biến thể)
                new ProductVariant { VariantId = 24, ProductId = 14, Sku = "QJ-BS-30", Size = "30", Color = "Xanh nhạt", ColorHex = "#ADD8E6", StockQuantity = 40, LowStockThreshold = 5, IsActive = true, CreatedAt = fixedDate },
                new ProductVariant { VariantId = 25, ProductId = 14, Sku = "QJ-BS-31", Size = "31", Color = "Xanh nhạt", ColorHex = "#ADD8E6", StockQuantity = 40, LowStockThreshold = 5, IsActive = true, CreatedAt = fixedDate },
                // SP 15: Áo thun basic (2 biến thể)
                new ProductVariant { VariantId = 26, ProductId = 15, Sku = "AT-BS-M", Size = "M", Color = "Trắng", ColorHex = "#FFFFFF", StockQuantity = 100, LowStockThreshold = 5, IsActive = true, CreatedAt = fixedDate },
                new ProductVariant { VariantId = 27, ProductId = 15, Sku = "AT-BS-L", Size = "L", Color = "Trắng", ColorHex = "#FFFFFF", StockQuantity = 100, LowStockThreshold = 5, IsActive = true, CreatedAt = fixedDate },
                // SP 16: Váy SHEIN (2 biến thể)
                new ProductVariant { VariantId = 28, ProductId = 16, Sku = "V-SN-S", Size = "S", Color = "Đen", ColorHex = "#000000", StockQuantity = 30, LowStockThreshold = 5, IsActive = true, CreatedAt = fixedDate },
                new ProductVariant { VariantId = 29, ProductId = 16, Sku = "V-SN-M", Size = "M", Color = "Đen", ColorHex = "#000000", StockQuantity = 30, LowStockThreshold = 5, IsActive = true, CreatedAt = fixedDate },
                // SP 17: Quần nỉ (2 biến thể)
                new ProductVariant { VariantId = 30, ProductId = 17, Sku = "Q-MT-M", Size = "M", Color = "Xám", ColorHex = "#808080", StockQuantity = 40, LowStockThreshold = 5, IsActive = true, CreatedAt = fixedDate },
                new ProductVariant { VariantId = 31, ProductId = 17, Sku = "Q-MT-L", Size = "L", Color = "Xám", ColorHex = "#808080", StockQuantity = 40, LowStockThreshold = 5, IsActive = true, CreatedAt = fixedDate }
            );

            // ==========================================
            // 6. ProductImages
            // ==========================================
            modelBuilder.Entity<ProductImage>().HasData(
                new ProductImage { ImageId = 1, ProductId = 1, VariantId = 1, ImageUrl = "/images/dressstyleimg1.png", IsThumbnail = true, SortOrder = 1, CreatedAt = fixedDate },
                new ProductImage { ImageId = 2, ProductId = 1, VariantId = 2, ImageUrl = "/images/newarrivalimg1.png", IsThumbnail = false, SortOrder = 2, CreatedAt = fixedDate },
                new ProductImage { ImageId = 3, ProductId = 2, ImageUrl = "/images/dressstyleimg3.png", IsThumbnail = true, SortOrder = 1, CreatedAt = fixedDate },
                new ProductImage { ImageId = 4, ProductId = 3, ImageUrl = "/images/heroimg.png", IsThumbnail = true, SortOrder = 1, CreatedAt = fixedDate },
                new ProductImage { ImageId = 5, ProductId = 4, VariantId = 5, ImageUrl = "/images/newarrivalimg2.png", IsThumbnail = true, SortOrder = 1, CreatedAt = fixedDate },
                new ProductImage { ImageId = 6, ProductId = 4, VariantId = 6, ImageUrl = "/images/topsellingimg4.png", IsThumbnail = false, SortOrder = 2, CreatedAt = fixedDate },
                new ProductImage { ImageId = 7, ProductId = 5, ImageUrl = "/images/topsellingimg1.png", IsThumbnail = true, SortOrder = 1, CreatedAt = fixedDate },
                new ProductImage { ImageId = 8, ProductId = 1, VariantId = 1, ImageUrl = "https://via.placeholder.com/600x800.png?text=Ao+Thun+Trang", IsThumbnail = true, SortOrder = 1, CreatedAt = fixedDate },
                new ProductImage { ImageId = 9, ProductId = 1, VariantId = 2, ImageUrl = "https://via.placeholder.com/600x800.png?text=Ao+Thun+Den", IsThumbnail = false, SortOrder = 2, CreatedAt = fixedDate },
                new ProductImage { ImageId = 10, ProductId = 2, ImageUrl = "https://via.placeholder.com/600x800.png?text=Vay+Hoa", IsThumbnail = true, SortOrder = 1, CreatedAt = fixedDate },
                new ProductImage { ImageId = 11, ProductId = 3, ImageUrl = "https://via.placeholder.com/600x800.png?text=Hoodie+Xam", IsThumbnail = true, SortOrder = 1, CreatedAt = fixedDate },
                new ProductImage { ImageId = 12, ProductId = 4, VariantId = 5, ImageUrl = "https://via.placeholder.com/600x800.png?text=Jeans+Denim", IsThumbnail = true, SortOrder = 1, CreatedAt = fixedDate },
                new ProductImage { ImageId = 13, ProductId = 4, VariantId = 6, ImageUrl = "https://via.placeholder.com/600x800.png?text=Jeans+Dam", IsThumbnail = false, SortOrder = 2, CreatedAt = fixedDate },
                new ProductImage { ImageId = 14, ProductId = 5, ImageUrl = "https://via.placeholder.com/600x800.png?text=So+Mi+Trang", IsThumbnail = true, SortOrder = 1, CreatedAt = fixedDate },
                
                // Các ảnh cho sản phẩm mới
                new ProductImage { ImageId = 15, ProductId = 6, ImageUrl = "/images/giay-the-thao-nu-slim.jpg", IsThumbnail = true, SortOrder = 1, CreatedAt = fixedDate },
                new ProductImage { ImageId = 16, ProductId = 7, ImageUrl = "/images/giay-bong-ro-yin-yang.jpg", IsThumbnail = true, SortOrder = 1, CreatedAt = fixedDate },
                new ProductImage { ImageId = 17, ProductId = 8, ImageUrl = "/images/blue-plaid-oversized-jeans.jpg", IsThumbnail = true, SortOrder = 1, CreatedAt = fixedDate },
                new ProductImage { ImageId = 18, ProductId = 9, ImageUrl = "/images/hm-pride-collection.jpg", IsThumbnail = true, SortOrder = 1, CreatedAt = fixedDate },
                new ProductImage { ImageId = 19, ProductId = 10, ImageUrl = "/images/looks-beautiful-dress.jpg", IsThumbnail = true, SortOrder = 1, CreatedAt = fixedDate },
                new ProductImage { ImageId = 20, ProductId = 11, ImageUrl = "/images/mens-pink-half-zip-jumper.jpg", IsThumbnail = true, SortOrder = 1, CreatedAt = fixedDate },
                new ProductImage { ImageId = 21, ProductId = 12, ImageUrl = "/images/nanushka-wide-leg-pants.jpg", IsThumbnail = true, SortOrder = 1, CreatedAt = fixedDate },
                new ProductImage { ImageId = 22, ProductId = 13, ImageUrl = "/images/womens-palazzo-pants.jpg", IsThumbnail = true, SortOrder = 1, CreatedAt = fixedDate },
                new ProductImage { ImageId = 23, ProductId = 14, ImageUrl = "/images/quan-jean-basic-ong-rong.jpg", IsThumbnail = true, SortOrder = 1, CreatedAt = fixedDate },
                new ProductImage { ImageId = 24, ProductId = 15, ImageUrl = "/images/t-shirt-basic.webp", IsThumbnail = true, SortOrder = 1, CreatedAt = fixedDate },
                new ProductImage { ImageId = 25, ProductId = 16, ImageUrl = "/images/shein-dress.webp", IsThumbnail = true, SortOrder = 1, CreatedAt = fixedDate },
                new ProductImage { ImageId = 26, ProductId = 17, ImageUrl = "/images/calca-moletom-pants.jpg", IsThumbnail = true, SortOrder = 1, CreatedAt = fixedDate },
                
                // Ảnh phụ bổ sung
                new ProductImage { ImageId = 27, ProductId = 6, ImageUrl = "/images/zapatillas-sport-casual.jpg", IsThumbnail = false, SortOrder = 2, CreatedAt = fixedDate },
                new ProductImage { ImageId = 28, ProductId = 8, ImageUrl = "/images/quan-jean-1.jpg", IsThumbnail = false, SortOrder = 2, CreatedAt = fixedDate },
                new ProductImage { ImageId = 29, ProductId = 11, ImageUrl = "/images/mens-new-arrivals.jpg", IsThumbnail = false, SortOrder = 2, CreatedAt = fixedDate },
                new ProductImage { ImageId = 30, ProductId = 10, ImageUrl = "/images/spring-summer-25.jpg", IsThumbnail = false, SortOrder = 2, CreatedAt = fixedDate }
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
                new OrderItem { OrderItemId = 1, OrderId = 1, ProductId = 1, VariantId = 1, ProductNameSnapshot = "Áo Thun Cổ Tròn Basic", SkuSnapshot = "ATB-TR-M", SizeSnapshot = "M", ColorSnapshot = "Trắng", ImageUrlSnapshot = "/images/dressstyleimg1.png", Quantity = 2, UnitPrice = 150000, SalePrice = 120000, LineTotal = 240000, ReviewStatus = "Reviewed", CreatedAt = fixedDate },
                new OrderItem { OrderItemId = 2, OrderId = 2, ProductId = 4, VariantId = 5, ProductNameSnapshot = "Quần Jeans Nam Slimfit", SkuSnapshot = "QJN-XANH-30", SizeSnapshot = "30", ColorSnapshot = "Xanh Denim", ImageUrlSnapshot = "/images/newarrivalimg2.png", Quantity = 1, UnitPrice = 450000, SalePrice = 399000, LineTotal = 399000, ReviewStatus = "NotReviewed", CreatedAt = fixedDate },
                new OrderItem { OrderItemId = 3, OrderId = 3, ProductId = 2, VariantId = 3, ProductNameSnapshot = "Váy Hoa Mùa Hè", SkuSnapshot = "VHM-DO-S", SizeSnapshot = "S", ColorSnapshot = "Đỏ", ImageUrlSnapshot = "/images/dressstyleimg3.png", Quantity = 1, UnitPrice = 350000, SalePrice = 300000, LineTotal = 300000, ReviewStatus = "NotReviewed", CreatedAt = fixedDate },
                new OrderItem { OrderItemId = 4, OrderId = 4, ProductId = 3, VariantId = 4, ProductNameSnapshot = "Áo Khoác Hoodie Unisex", SkuSnapshot = "AKH-XA-XL", SizeSnapshot = "XL", ColorSnapshot = "Xám", ImageUrlSnapshot = "/images/heroimg.png", Quantity = 1, UnitPrice = 400000, SalePrice = 400000, LineTotal = 400000, ReviewStatus = "NotReviewed", CreatedAt = fixedDate }
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
