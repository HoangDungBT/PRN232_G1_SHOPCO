-- ==========================================================================
-- SCRIPT SQL: CHÈN SẢN PHẨM MỚI VÀO DATABASE SHOP.CO
-- Hướng dẫn: Chạy script này trực tiếp trên cơ sở dữ liệu SQL Server của bạn.
-- ==========================================================================

BEGIN TRANSACTION;

BEGIN TRY
    -- Sử dụng múi giờ cố định tương ứng với seed data
    DECLARE @fixedDate DATETIME2 = '2024-01-01 00:00:00';

    -- 1. Sửa lỗi trùng khóa ImageId = 7 (Placeholder của sản phẩm 5) trong database cũ nếu có
    IF EXISTS (SELECT 1 FROM ProductImages WHERE ImageId = 7 AND ProductId = 5 AND ImageUrl LIKE '%placeholder%')
    BEGIN
        SET IDENTITY_INSERT ProductImages ON;
        -- Xóa dòng cũ và chèn lại với ID = 14 để giải phóng khóa 7 cho Topselling image
        DELETE FROM ProductImages WHERE ImageId = 7 AND ProductId = 5 AND ImageUrl LIKE '%placeholder%';
        INSERT INTO ProductImages (ImageId, ProductId, VariantId, ImageUrl, AltText, IsThumbnail, SortOrder, CreatedAt)
        VALUES (14, 5, NULL, 'https://via.placeholder.com/600x800.png?text=So+Mi+Trang', NULL, 1, 1, @fixedDate);
        SET IDENTITY_INSERT ProductImages OFF;
        PRINT 'Fixed duplicate ImageId 7 bug in existing data.';
    END

    -- 2. Thêm danh mục mới (Giày Thể Thao)
    SET IDENTITY_INSERT Categories ON;
    IF NOT EXISTS (SELECT 1 FROM Categories WHERE CategoryId = 8)
    BEGIN
        INSERT INTO Categories (CategoryId, ParentCategoryId, CategoryName, Slug, Description, ImageUrl, SortOrder, IsActive, CreatedAt)
        VALUES (8, 3, N'Giày Thể Thao', 'giay-the-thao', NULL, NULL, 0, 1, @fixedDate);
        PRINT 'Inserted Category 8 (Giày Thể Thao).';
    END
    SET IDENTITY_INSERT Categories OFF;

    -- 3. Thêm các sản phẩm mới (ProductId từ 6 đến 17)
    SET IDENTITY_INSERT Products ON;
    
    -- SP 6: Giày Thể Thao Nữ Slim Đế 2cm
    IF NOT EXISTS (SELECT 1 FROM Products WHERE ProductId = 6)
    BEGIN
        INSERT INTO Products (ProductId, CategoryId, ProductName, Slug, Brand, Material, GenderTarget, BasePrice, SalePrice, AverageRating, ReviewCount, ViewCount, IsFeatured, IsBestSeller, IsNewArrival, IsActive, CreatedAt)
        VALUES (6, 8, N'Giày Thể Thao Nữ Slim Đế 2cm', 'giay-the-thao-nu-slim', N'SlimSport', N'Lưới/Da', N'Nữ', 450000.00, 390000.00, 4.70, 14, 120, 1, 0, 0, 1, @fixedDate);
    END

    -- SP 7: Giày Bóng Rổ Cao Cấp Yin Yang
    IF NOT EXISTS (SELECT 1 FROM Products WHERE ProductId = 7)
    BEGIN
        INSERT INTO Products (ProductId, CategoryId, ProductName, Slug, Brand, Material, GenderTarget, BasePrice, SalePrice, AverageRating, ReviewCount, ViewCount, IsFeatured, IsBestSeller, IsNewArrival, IsActive, CreatedAt)
        VALUES (7, 8, N'Giày Bóng Rổ Cao Cấp Yin Yang', 'giay-bong-ro-yin-yang', N'YinYang', N'Lưới thoáng khí', N'Nam/Unisex', 850000.00, 790000.00, 4.90, 38, 250, 0, 1, 0, 1, @fixedDate);
    END

    -- SP 8: Áo Sơ Mi Sọc Xanh Plaid
    IF NOT EXISTS (SELECT 1 FROM Products WHERE ProductId = 8)
    BEGIN
        INSERT INTO Products (ProductId, CategoryId, ProductName, Slug, Brand, Material, GenderTarget, BasePrice, SalePrice, AverageRating, ReviewCount, ViewCount, IsFeatured, IsBestSeller, IsNewArrival, IsActive, CreatedAt)
        VALUES (8, 7, N'Áo Sơ Mi Sọc Xanh Plaid', 'ao-so-mi-soc-xanh-plaid', N'PlaidWear', N'Cotton/Plaid', N'Nữ', 280000.00, 250000.00, 4.40, 8, 45, 0, 0, 0, 1, @fixedDate);
    END

    -- SP 9: Áo Phông H&M Pride Collection
    IF NOT EXISTS (SELECT 1 FROM Products WHERE ProductId = 9)
    BEGIN
        INSERT INTO Products (ProductId, CategoryId, ProductName, Slug, Brand, Material, GenderTarget, BasePrice, SalePrice, AverageRating, ReviewCount, ViewCount, IsFeatured, IsBestSeller, IsNewArrival, IsActive, CreatedAt)
        VALUES (9, 4, N'Áo Phông H&M Pride Collection', 'ao-phong-hm-pride', N'H&M', N'Cotton', N'Nam/Unisex', 190000.00, NULL, 4.60, 12, 95, 0, 0, 1, 1, @fixedDate);
    END

    -- SP 10: Váy Hoa Looks Beautiful
    IF NOT EXISTS (SELECT 1 FROM Products WHERE ProductId = 10)
    BEGIN
        INSERT INTO Products (ProductId, CategoryId, ProductName, Slug, Brand, Material, GenderTarget, BasePrice, SalePrice, AverageRating, ReviewCount, ViewCount, IsFeatured, IsBestSeller, IsNewArrival, IsActive, CreatedAt)
        VALUES (10, 6, N'Váy Hoa Looks Beautiful', 'vay-hoa-looks-beautiful', N'BeautifulWear', N'Chiffon', N'Nữ', 320000.00, 299000.00, 4.80, 21, 150, 0, 0, 0, 1, @fixedDate);
    END

    -- SP 11: Áo Len Cổ Cao Half Zip Pink
    IF NOT EXISTS (SELECT 1 FROM Products WHERE ProductId = 11)
    BEGIN
        INSERT INTO Products (ProductId, CategoryId, ProductName, Slug, Brand, Material, GenderTarget, BasePrice, SalePrice, AverageRating, ReviewCount, ViewCount, IsFeatured, IsBestSeller, IsNewArrival, IsActive, CreatedAt)
        VALUES (11, 4, N'Áo Len Cổ Cao Half Zip Pink', 'ao-len-co-cao-half-zip', N'CrewClothing', N'Len/Acrylic', N'Nam', 480000.00, 420000.00, 4.50, 7, 30, 0, 0, 0, 1, @fixedDate);
    END

    -- SP 12: Quần Ống Rộng Nanushka Cleo
    IF NOT EXISTS (SELECT 1 FROM Products WHERE ProductId = 12)
    BEGIN
        INSERT INTO Products (ProductId, CategoryId, ProductName, Slug, Brand, Material, GenderTarget, BasePrice, SalePrice, AverageRating, ReviewCount, ViewCount, IsFeatured, IsBestSeller, IsNewArrival, IsActive, CreatedAt)
        VALUES (12, 2, N'Quần Ống Rộng Nanushka Cleo', 'quan-ong-rong-nanushka', N'Nanushka', N'Crepe', N'Nữ', 520000.00, NULL, 4.20, 4, 18, 0, 0, 0, 1, @fixedDate);
    END

    -- SP 13: Quần Palazzo Cạp Cao Womens Wide
    IF NOT EXISTS (SELECT 1 FROM Products WHERE ProductId = 13)
    BEGIN
        INSERT INTO Products (ProductId, CategoryId, ProductName, Slug, Brand, Material, GenderTarget, BasePrice, SalePrice, AverageRating, ReviewCount, ViewCount, IsFeatured, IsBestSeller, IsNewArrival, IsActive, CreatedAt)
        VALUES (13, 2, N'Quần Palazzo Cạp Cao Womens Wide', 'quan-palazzo-cap-cao', N'Palazzo', N'Tuyết mưa', N'Nữ', 360000.00, 320000.00, 4.50, 19, 85, 0, 1, 0, 1, @fixedDate);
    END

    -- SP 14: Quần Jean Basic Ống Rộng Cá Tính
    IF NOT EXISTS (SELECT 1 FROM Products WHERE ProductId = 14)
    BEGIN
        INSERT INTO Products (ProductId, CategoryId, ProductName, Slug, Brand, Material, GenderTarget, BasePrice, SalePrice, AverageRating, ReviewCount, ViewCount, IsFeatured, IsBestSeller, IsNewArrival, IsActive, CreatedAt)
        VALUES (14, 5, N'Quần Jean Basic Ống Rộng Cá Tính', 'quan-jean-basic-ong-rong', N'DenimBasic', N'Denim', N'Nam/Unisex', 390000.00, 350000.00, 4.80, 31, 140, 0, 1, 0, 1, @fixedDate);
    END

    -- SP 15: Áo Thun T-Shirt Basic
    IF NOT EXISTS (SELECT 1 FROM Products WHERE ProductId = 15)
    BEGIN
        INSERT INTO Products (ProductId, CategoryId, ProductName, Slug, Brand, Material, GenderTarget, BasePrice, SalePrice, AverageRating, ReviewCount, ViewCount, IsFeatured, IsBestSeller, IsNewArrival, IsActive, CreatedAt)
        VALUES (15, 4, N'Áo Thun T-Shirt Basic', 'ao-thun-t-shirt-basic', N'Shop.Co', N'Cotton', N'Nam/Unisex', 120000.00, 99000.00, 4.30, 5, 200, 0, 0, 0, 1, @fixedDate);
    END

    -- SP 16: Váy Đầm SHEIN
    IF NOT EXISTS (SELECT 1 FROM Products WHERE ProductId = 16)
    BEGIN
        INSERT INTO Products (ProductId, CategoryId, ProductName, Slug, Brand, Material, GenderTarget, BasePrice, SalePrice, AverageRating, ReviewCount, ViewCount, IsFeatured, IsBestSeller, IsNewArrival, IsActive, CreatedAt)
        VALUES (16, 6, N'Váy Đầm SHEIN', 'vay-dam-shein', N'SHEIN', N'Polyester', N'Nữ', 290000.00, NULL, 4.10, 3, 22, 0, 0, 0, 1, @fixedDate);
    END

    -- SP 17: Quần Nỉ calca moletom
    IF NOT EXISTS (SELECT 1 FROM Products WHERE ProductId = 17)
    BEGIN
        INSERT INTO Products (ProductId, CategoryId, ProductName, Slug, Brand, Material, GenderTarget, BasePrice, SalePrice, AverageRating, ReviewCount, ViewCount, IsFeatured, IsBestSeller, IsNewArrival, IsActive, CreatedAt)
        VALUES (17, 5, N'Quần Nỉ calca moletom', 'quan-ni-moletom', N'Moletom', N'Nỉ da cá', N'Nam/Unisex', 250000.00, 199000.00, 4.60, 9, 36, 0, 0, 0, 1, @fixedDate);
    END

    SET IDENTITY_INSERT Products OFF;
    PRINT 'Inserted new Products.';

    -- 4. Thêm các biến thể sản phẩm mới (ProductVariants từ VariantId = 8 đến 31)
    SET IDENTITY_INSERT ProductVariants ON;

    -- SP 6: Giày Slim
    IF NOT EXISTS (SELECT 1 FROM ProductVariants WHERE VariantId = 8)
        INSERT INTO ProductVariants (VariantId, ProductId, Sku, Size, Color, ColorHex, ExtraPrice, StockQuantity, LowStockThreshold, IsActive, CreatedAt)
        VALUES (8, 6, 'GTT-SLIM-38', '38', N'Sọc', '#FF4500', 0.00, 50, 5, 1, @fixedDate);
    IF NOT EXISTS (SELECT 1 FROM ProductVariants WHERE VariantId = 9)
        INSERT INTO ProductVariants (VariantId, ProductId, Sku, Size, Color, ColorHex, ExtraPrice, StockQuantity, LowStockThreshold, IsActive, CreatedAt)
        VALUES (9, 6, 'GTT-SLIM-39', '39', N'Sọc', '#FF4500', 0.00, 50, 5, 1, @fixedDate);

    -- SP 7: Giày Yin Yang
    IF NOT EXISTS (SELECT 1 FROM ProductVariants WHERE VariantId = 10)
        INSERT INTO ProductVariants (VariantId, ProductId, Sku, Size, Color, ColorHex, ExtraPrice, StockQuantity, LowStockThreshold, IsActive, CreatedAt)
        VALUES (10, 7, 'GBR-YY-40', '40', N'Yin Yang', '#000000', 0.00, 40, 5, 1, @fixedDate);
    IF NOT EXISTS (SELECT 1 FROM ProductVariants WHERE VariantId = 11)
        INSERT INTO ProductVariants (VariantId, ProductId, Sku, Size, Color, ColorHex, ExtraPrice, StockQuantity, LowStockThreshold, IsActive, CreatedAt)
        VALUES (11, 7, 'GBR-YY-41', '41', N'Yin Yang', '#FFFFFF', 0.00, 40, 5, 1, @fixedDate);

    -- SP 8: Áo sơ mi Plaid
    IF NOT EXISTS (SELECT 1 FROM ProductVariants WHERE VariantId = 12)
        INSERT INTO ProductVariants (VariantId, ProductId, Sku, Size, Color, ColorHex, ExtraPrice, StockQuantity, LowStockThreshold, IsActive, CreatedAt)
        VALUES (12, 8, 'ASM-PL-M', 'M', N'Kẻ sọc xanh', '#4682B4', 0.00, 50, 5, 1, @fixedDate);
    IF NOT EXISTS (SELECT 1 FROM ProductVariants WHERE VariantId = 13)
        INSERT INTO ProductVariants (VariantId, ProductId, Sku, Size, Color, ColorHex, ExtraPrice, StockQuantity, LowStockThreshold, IsActive, CreatedAt)
        VALUES (13, 8, 'ASM-PL-L', 'L', N'Kẻ sọc xanh', '#4682B4', 0.00, 50, 5, 1, @fixedDate);

    -- SP 9: Áo phông H&M
    IF NOT EXISTS (SELECT 1 FROM ProductVariants WHERE VariantId = 14)
        INSERT INTO ProductVariants (VariantId, ProductId, Sku, Size, Color, ColorHex, ExtraPrice, StockQuantity, LowStockThreshold, IsActive, CreatedAt)
        VALUES (14, 9, 'AT-HM-M', 'M', N'Pride', '#FF1493', 0.00, 60, 5, 1, @fixedDate);
    IF NOT EXISTS (SELECT 1 FROM ProductVariants WHERE VariantId = 15)
        INSERT INTO ProductVariants (VariantId, ProductId, Sku, Size, Color, ColorHex, ExtraPrice, StockQuantity, LowStockThreshold, IsActive, CreatedAt)
        VALUES (15, 9, 'AT-HM-L', 'L', N'Pride', '#FF1493', 0.00, 60, 5, 1, @fixedDate);

    -- SP 10: Váy Looks Beautiful
    IF NOT EXISTS (SELECT 1 FROM ProductVariants WHERE VariantId = 16)
        INSERT INTO ProductVariants (VariantId, ProductId, Sku, Size, Color, ColorHex, ExtraPrice, StockQuantity, LowStockThreshold, IsActive, CreatedAt)
        VALUES (16, 10, 'V-LB-S', 'S', N'Đỏ', '#8B0000', 0.00, 30, 5, 1, @fixedDate);
    IF NOT EXISTS (SELECT 1 FROM ProductVariants WHERE VariantId = 17)
        INSERT INTO ProductVariants (VariantId, ProductId, Sku, Size, Color, ColorHex, ExtraPrice, StockQuantity, LowStockThreshold, IsActive, CreatedAt)
        VALUES (17, 10, 'V-LB-M', 'M', N'Đỏ', '#8B0000', 0.00, 30, 5, 1, @fixedDate);

    -- SP 11: Áo len cổ cao
    IF NOT EXISTS (SELECT 1 FROM ProductVariants WHERE VariantId = 18)
        INSERT INTO ProductVariants (VariantId, ProductId, Sku, Size, Color, ColorHex, ExtraPrice, StockQuantity, LowStockThreshold, IsActive, CreatedAt)
        VALUES (18, 11, 'AL-HZ-M', 'M', N'Hồng', '#FFB6C1', 0.00, 40, 5, 1, @fixedDate);
    IF NOT EXISTS (SELECT 1 FROM ProductVariants WHERE VariantId = 19)
        INSERT INTO ProductVariants (VariantId, ProductId, Sku, Size, Color, ColorHex, ExtraPrice, StockQuantity, LowStockThreshold, IsActive, CreatedAt)
        VALUES (19, 11, 'AL-HZ-L', 'L', N'Hồng', '#FFB6C1', 0.00, 40, 5, 1, @fixedDate);

    -- SP 12: Quần Nanushka
    IF NOT EXISTS (SELECT 1 FROM ProductVariants WHERE VariantId = 20)
        INSERT INTO ProductVariants (VariantId, ProductId, Sku, Size, Color, ColorHex, ExtraPrice, StockQuantity, LowStockThreshold, IsActive, CreatedAt)
        VALUES (20, 12, 'Q-NS-M', 'M', N'Be', '#F5F5DC', 0.00, 30, 5, 1, @fixedDate);
    IF NOT EXISTS (SELECT 1 FROM ProductVariants WHERE VariantId = 21)
        INSERT INTO ProductVariants (VariantId, ProductId, Sku, Size, Color, ColorHex, ExtraPrice, StockQuantity, LowStockThreshold, IsActive, CreatedAt)
        VALUES (21, 12, 'Q-NS-L', 'L', N'Be', '#F5F5DC', 0.00, 30, 5, 1, @fixedDate);

    -- SP 13: Quần Palazzo
    IF NOT EXISTS (SELECT 1 FROM ProductVariants WHERE VariantId = 22)
        INSERT INTO ProductVariants (VariantId, ProductId, Sku, Size, Color, ColorHex, ExtraPrice, StockQuantity, LowStockThreshold, IsActive, CreatedAt)
        VALUES (22, 13, 'Q-PZ-M', 'M', N'Đen', '#000000', 0.00, 50, 5, 1, @fixedDate);
    IF NOT EXISTS (SELECT 1 FROM ProductVariants WHERE VariantId = 23)
        INSERT INTO ProductVariants (VariantId, ProductId, Sku, Size, Color, ColorHex, ExtraPrice, StockQuantity, LowStockThreshold, IsActive, CreatedAt)
        VALUES (23, 13, 'Q-PZ-L', 'L', N'Đen', '#000000', 0.00, 50, 5, 1, @fixedDate);

    -- SP 14: Quần Jean Basic
    IF NOT EXISTS (SELECT 1 FROM ProductVariants WHERE VariantId = 24)
        INSERT INTO ProductVariants (VariantId, ProductId, Sku, Size, Color, ColorHex, ExtraPrice, StockQuantity, LowStockThreshold, IsActive, CreatedAt)
        VALUES (24, 14, 'QJ-BS-30', '30', N'Xanh nhạt', '#ADD8E6', 0.00, 40, 5, 1, @fixedDate);
    IF NOT EXISTS (SELECT 1 FROM ProductVariants WHERE VariantId = 25)
        INSERT INTO ProductVariants (VariantId, ProductId, Sku, Size, Color, ColorHex, ExtraPrice, StockQuantity, LowStockThreshold, IsActive, CreatedAt)
        VALUES (25, 14, 'QJ-BS-31', '31', N'Xanh nhạt', '#ADD8E6', 0.00, 40, 5, 1, @fixedDate);

    -- SP 15: Áo thun basic
    IF NOT EXISTS (SELECT 1 FROM ProductVariants WHERE VariantId = 26)
        INSERT INTO ProductVariants (VariantId, ProductId, Sku, Size, Color, ColorHex, ExtraPrice, StockQuantity, LowStockThreshold, IsActive, CreatedAt)
        VALUES (26, 15, 'AT-BS-M', 'M', N'Trắng', '#FFFFFF', 0.00, 100, 5, 1, @fixedDate);
    IF NOT EXISTS (SELECT 1 FROM ProductVariants WHERE VariantId = 27)
        INSERT INTO ProductVariants (VariantId, ProductId, Sku, Size, Color, ColorHex, ExtraPrice, StockQuantity, LowStockThreshold, IsActive, CreatedAt)
        VALUES (27, 15, 'AT-BS-L', 'L', N'Trắng', '#FFFFFF', 0.00, 100, 5, 1, @fixedDate);

    -- SP 16: Váy SHEIN
    IF NOT EXISTS (SELECT 1 FROM ProductVariants WHERE VariantId = 28)
        INSERT INTO ProductVariants (VariantId, ProductId, Sku, Size, Color, ColorHex, ExtraPrice, StockQuantity, LowStockThreshold, IsActive, CreatedAt)
        VALUES (28, 16, 'V-SN-S', 'S', N'Đen', '#000000', 0.00, 30, 5, 1, @fixedDate);
    IF NOT EXISTS (SELECT 1 FROM ProductVariants WHERE VariantId = 29)
        INSERT INTO ProductVariants (VariantId, ProductId, Sku, Size, Color, ColorHex, ExtraPrice, StockQuantity, LowStockThreshold, IsActive, CreatedAt)
        VALUES (29, 16, 'V-SN-M', 'M', N'Đen', '#000000', 0.00, 30, 5, 1, @fixedDate);

    -- SP 17: Quần nỉ
    IF NOT EXISTS (SELECT 1 FROM ProductVariants WHERE VariantId = 30)
        INSERT INTO ProductVariants (VariantId, ProductId, Sku, Size, Color, ColorHex, ExtraPrice, StockQuantity, LowStockThreshold, IsActive, CreatedAt)
        VALUES (30, 17, 'Q-MT-M', 'M', N'Xám', '#808080', 0.00, 40, 5, 1, @fixedDate);
    IF NOT EXISTS (SELECT 1 FROM ProductVariants WHERE VariantId = 31)
        INSERT INTO ProductVariants (VariantId, ProductId, Sku, Size, Color, ColorHex, ExtraPrice, StockQuantity, LowStockThreshold, IsActive, CreatedAt)
        VALUES (31, 17, 'Q-MT-L', 'L', N'Xám', '#808080', 0.00, 40, 5, 1, @fixedDate);

    SET IDENTITY_INSERT ProductVariants OFF;
    PRINT 'Inserted new ProductVariants.';

    -- 5. Thêm hình ảnh sản phẩm mới (ProductImages từ ImageId = 15 đến 30)
    SET IDENTITY_INSERT ProductImages ON;

    -- Sản phẩm mới Thumbnails
    IF NOT EXISTS (SELECT 1 FROM ProductImages WHERE ImageId = 15)
        INSERT INTO ProductImages (ImageId, ProductId, VariantId, ImageUrl, AltText, IsThumbnail, SortOrder, CreatedAt)
        VALUES (15, 6, NULL, '/images/giay-the-thao-nu-slim.jpg', N'Giày Slim', 1, 1, @fixedDate);

    IF NOT EXISTS (SELECT 1 FROM ProductImages WHERE ImageId = 16)
        INSERT INTO ProductImages (ImageId, ProductId, VariantId, ImageUrl, AltText, IsThumbnail, SortOrder, CreatedAt)
        VALUES (16, 7, NULL, '/images/giay-bong-ro-yin-yang.jpg', N'Giày Yin Yang', 1, 1, @fixedDate);

    IF NOT EXISTS (SELECT 1 FROM ProductImages WHERE ImageId = 17)
        INSERT INTO ProductImages (ImageId, ProductId, VariantId, ImageUrl, AltText, IsThumbnail, SortOrder, CreatedAt)
        VALUES (17, 8, NULL, '/images/blue-plaid-oversized-jeans.jpg', N'Sơ mi Plaid', 1, 1, @fixedDate);

    IF NOT EXISTS (SELECT 1 FROM ProductImages WHERE ImageId = 18)
        INSERT INTO ProductImages (ImageId, ProductId, VariantId, ImageUrl, AltText, IsThumbnail, SortOrder, CreatedAt)
        VALUES (18, 9, NULL, '/images/hm-pride-collection.jpg', N'Áo H&M Pride', 1, 1, @fixedDate);

    IF NOT EXISTS (SELECT 1 FROM ProductImages WHERE ImageId = 19)
        INSERT INTO ProductImages (ImageId, ProductId, VariantId, ImageUrl, AltText, IsThumbnail, SortOrder, CreatedAt)
        VALUES (19, 10, NULL, '/images/looks-beautiful-dress.jpg', N'Váy Looks Beautiful', 1, 1, @fixedDate);

    IF NOT EXISTS (SELECT 1 FROM ProductImages WHERE ImageId = 20)
        INSERT INTO ProductImages (ImageId, ProductId, VariantId, ImageUrl, AltText, IsThumbnail, SortOrder, CreatedAt)
        VALUES (20, 11, NULL, '/images/mens-pink-half-zip-jumper.jpg', N'Áo Len Half Zip Pink', 1, 1, @fixedDate);

    IF NOT EXISTS (SELECT 1 FROM ProductImages WHERE ImageId = 21)
        INSERT INTO ProductImages (ImageId, ProductId, VariantId, ImageUrl, AltText, IsThumbnail, SortOrder, CreatedAt)
        VALUES (21, 12, NULL, '/images/nanushka-wide-leg-pants.jpg', N'Quần Nanushka Cleo', 1, 1, @fixedDate);

    IF NOT EXISTS (SELECT 1 FROM ProductImages WHERE ImageId = 22)
        INSERT INTO ProductImages (ImageId, ProductId, VariantId, ImageUrl, AltText, IsThumbnail, SortOrder, CreatedAt)
        VALUES (22, 13, NULL, '/images/womens-palazzo-pants.jpg', N'Quần Palazzo', 1, 1, @fixedDate);

    IF NOT EXISTS (SELECT 1 FROM ProductImages WHERE ImageId = 23)
        INSERT INTO ProductImages (ImageId, ProductId, VariantId, ImageUrl, AltText, IsThumbnail, SortOrder, CreatedAt)
        VALUES (23, 14, NULL, '/images/quan-jean-basic-ong-rong.jpg', N'Quần Jean Basic', 1, 1, @fixedDate);

    IF NOT EXISTS (SELECT 1 FROM ProductImages WHERE ImageId = 24)
        INSERT INTO ProductImages (ImageId, ProductId, VariantId, ImageUrl, AltText, IsThumbnail, SortOrder, CreatedAt)
        VALUES (24, 15, NULL, '/images/t-shirt-basic.webp', N'Áo Thun Basic', 1, 1, @fixedDate);

    IF NOT EXISTS (SELECT 1 FROM ProductImages WHERE ImageId = 25)
        INSERT INTO ProductImages (ImageId, ProductId, VariantId, ImageUrl, AltText, IsThumbnail, SortOrder, CreatedAt)
        VALUES (25, 16, NULL, '/images/shein-dress.webp', N'Váy SHEIN', 1, 1, @fixedDate);

    IF NOT EXISTS (SELECT 1 FROM ProductImages WHERE ImageId = 26)
        INSERT INTO ProductImages (ImageId, ProductId, VariantId, ImageUrl, AltText, IsThumbnail, SortOrder, CreatedAt)
        VALUES (26, 17, NULL, '/images/calca-moletom-pants.jpg', N'Quần Nỉ calca moletom', 1, 1, @fixedDate);

    -- Ảnh phụ bổ sung (Gallery)
    IF NOT EXISTS (SELECT 1 FROM ProductImages WHERE ImageId = 27)
        INSERT INTO ProductImages (ImageId, ProductId, VariantId, ImageUrl, AltText, IsThumbnail, SortOrder, CreatedAt)
        VALUES (27, 6, NULL, '/images/zapatillas-sport-casual.jpg', N'Giày Slim (Góc khác)', 0, 2, @fixedDate);

    IF NOT EXISTS (SELECT 1 FROM ProductImages WHERE ImageId = 28)
        INSERT INTO ProductImages (ImageId, ProductId, VariantId, ImageUrl, AltText, IsThumbnail, SortOrder, CreatedAt)
        VALUES (28, 8, NULL, '/images/quan-jean-1.jpg', N'Sơ mi Plaid (Góc khác)', 0, 2, @fixedDate);

    IF NOT EXISTS (SELECT 1 FROM ProductImages WHERE ImageId = 29)
        INSERT INTO ProductImages (ImageId, ProductId, VariantId, ImageUrl, AltText, IsThumbnail, SortOrder, CreatedAt)
        VALUES (29, 11, NULL, '/images/mens-new-arrivals.jpg', N'Áo Len (Góc khác)', 0, 2, @fixedDate);

    IF NOT EXISTS (SELECT 1 FROM ProductImages WHERE ImageId = 30)
        INSERT INTO ProductImages (ImageId, ProductId, VariantId, ImageUrl, AltText, IsThumbnail, SortOrder, CreatedAt)
        VALUES (30, 10, NULL, '/images/spring-summer-25.jpg', N'Váy Hoa (Góc khác)', 0, 2, @fixedDate);

    SET IDENTITY_INSERT ProductImages OFF;
    PRINT 'Inserted new ProductImages.';

    COMMIT TRANSACTION;
    PRINT 'Transaction committed successfully.';
END TRY
BEGIN CATCH
    ROLLBACK TRANSACTION;
    PRINT 'Transaction rolled back due to error.';
    THROW;
END CATCH
