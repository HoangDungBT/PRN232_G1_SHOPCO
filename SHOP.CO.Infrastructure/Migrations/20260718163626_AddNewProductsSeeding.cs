using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SHOP.CO.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddNewProductsSeeding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Users_Status",
                table: "Users");

            migrationBuilder.AddColumn<bool>(
                name: "IsNewsletterSubscribed",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "CategoryId", "CategoryName", "CreatedAt", "Description", "ImageUrl", "IsActive", "ParentCategoryId", "Slug", "UpdatedAt" },
                values: new object[] { 8, "Giày Thể Thao", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, 3, "giay-the-thao", null });

            migrationBuilder.UpdateData(
                table: "OrderItems",
                keyColumn: "OrderItemId",
                keyValue: 1,
                column: "ImageUrlSnapshot",
                value: "/images/dressstyleimg1.png");

            migrationBuilder.UpdateData(
                table: "OrderItems",
                keyColumn: "OrderItemId",
                keyValue: 2,
                column: "ImageUrlSnapshot",
                value: "/images/newarrivalimg2.png");

            migrationBuilder.UpdateData(
                table: "OrderItems",
                keyColumn: "OrderItemId",
                keyValue: 3,
                column: "ImageUrlSnapshot",
                value: "/images/dressstyleimg3.png");

            migrationBuilder.UpdateData(
                table: "OrderItems",
                keyColumn: "OrderItemId",
                keyValue: 4,
                column: "ImageUrlSnapshot",
                value: "/images/heroimg.png");

            migrationBuilder.UpdateData(
                table: "ProductImages",
                keyColumn: "ImageId",
                keyValue: 1,
                column: "ImageUrl",
                value: "/images/dressstyleimg1.png");

            migrationBuilder.UpdateData(
                table: "ProductImages",
                keyColumn: "ImageId",
                keyValue: 2,
                column: "ImageUrl",
                value: "/images/newarrivalimg1.png");

            migrationBuilder.UpdateData(
                table: "ProductImages",
                keyColumn: "ImageId",
                keyValue: 3,
                column: "ImageUrl",
                value: "/images/dressstyleimg3.png");

            migrationBuilder.UpdateData(
                table: "ProductImages",
                keyColumn: "ImageId",
                keyValue: 4,
                column: "ImageUrl",
                value: "/images/heroimg.png");

            migrationBuilder.UpdateData(
                table: "ProductImages",
                keyColumn: "ImageId",
                keyValue: 5,
                column: "ImageUrl",
                value: "/images/newarrivalimg2.png");

            migrationBuilder.UpdateData(
                table: "ProductImages",
                keyColumn: "ImageId",
                keyValue: 6,
                column: "ImageUrl",
                value: "/images/topsellingimg4.png");

            migrationBuilder.UpdateData(
                table: "ProductImages",
                keyColumn: "ImageId",
                keyValue: 7,
                column: "ImageUrl",
                value: "/images/topsellingimg1.png");

            migrationBuilder.InsertData(
                table: "ProductImages",
                columns: new[] { "ImageId", "AltText", "ColorAnalysisJson", "ContentType", "CreatedAt", "DominantColorHex", "FileName", "FileSize", "ImageUrl", "IsThumbnail", "ProductId", "SortOrder", "VariantId" },
                values: new object[] { 8, null, null, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, "https://via.placeholder.com/600x800.png?text=Ao+Thun+Trang", true, 1, 1, 1 });

            migrationBuilder.InsertData(
                table: "ProductImages",
                columns: new[] { "ImageId", "AltText", "ColorAnalysisJson", "ContentType", "CreatedAt", "DominantColorHex", "FileName", "FileSize", "ImageUrl", "ProductId", "SortOrder", "VariantId" },
                values: new object[] { 9, null, null, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, "https://via.placeholder.com/600x800.png?text=Ao+Thun+Den", 1, 2, 2 });

            migrationBuilder.InsertData(
                table: "ProductImages",
                columns: new[] { "ImageId", "AltText", "ColorAnalysisJson", "ContentType", "CreatedAt", "DominantColorHex", "FileName", "FileSize", "ImageUrl", "IsThumbnail", "ProductId", "SortOrder", "VariantId" },
                values: new object[,]
                {
                    { 10, null, null, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, "https://via.placeholder.com/600x800.png?text=Vay+Hoa", true, 2, 1, null },
                    { 11, null, null, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, "https://via.placeholder.com/600x800.png?text=Hoodie+Xam", true, 3, 1, null },
                    { 12, null, null, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, "https://via.placeholder.com/600x800.png?text=Jeans+Denim", true, 4, 1, 5 }
                });

            migrationBuilder.InsertData(
                table: "ProductImages",
                columns: new[] { "ImageId", "AltText", "ColorAnalysisJson", "ContentType", "CreatedAt", "DominantColorHex", "FileName", "FileSize", "ImageUrl", "ProductId", "SortOrder", "VariantId" },
                values: new object[] { 13, null, null, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, "https://via.placeholder.com/600x800.png?text=Jeans+Dam", 4, 2, 6 });

            migrationBuilder.InsertData(
                table: "ProductImages",
                columns: new[] { "ImageId", "AltText", "ColorAnalysisJson", "ContentType", "CreatedAt", "DominantColorHex", "FileName", "FileSize", "ImageUrl", "IsThumbnail", "ProductId", "SortOrder", "VariantId" },
                values: new object[] { 14, null, null, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, "https://via.placeholder.com/600x800.png?text=So+Mi+Trang", true, 5, 1, null });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "ProductId", "AverageRating", "BasePrice", "Brand", "CategoryId", "CreatedAt", "DeletedAt", "Description", "GenderTarget", "IsActive", "Material", "ProductName", "ReviewCount", "SalePrice", "Slug", "UpdatedAt" },
                values: new object[] { 8, 4.4m, 280000m, "PlaidWear", 7, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, true, "Cotton/Plaid", "Áo Sơ Mi Sọc Xanh Plaid", 8, 250000m, "ao-so-mi-soc-xanh-plaid", null });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "ProductId", "AverageRating", "BasePrice", "Brand", "CategoryId", "CreatedAt", "DeletedAt", "Description", "GenderTarget", "IsActive", "IsNewArrival", "Material", "ProductName", "ReviewCount", "SalePrice", "Slug", "UpdatedAt" },
                values: new object[] { 9, 4.6m, 190000m, "H&M", 4, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, true, true, "Cotton", "Áo Phông H&M Pride Collection", 12, null, "ao-phong-hm-pride", null });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "ProductId", "AverageRating", "BasePrice", "Brand", "CategoryId", "CreatedAt", "DeletedAt", "Description", "GenderTarget", "IsActive", "Material", "ProductName", "ReviewCount", "SalePrice", "Slug", "UpdatedAt" },
                values: new object[,]
                {
                    { 10, 4.8m, 320000m, "BeautifulWear", 6, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, true, "Chiffon", "Váy Hoa Looks Beautiful", 21, 299000m, "vay-hoa-looks-beautiful", null },
                    { 11, 4.5m, 480000m, "CrewClothing", 4, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, true, "Len/Acrylic", "Áo Len Cổ Cao Half Zip Pink", 7, 420000m, "ao-len-co-cao-half-zip", null },
                    { 12, 4.2m, 520000m, "Nanushka", 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, true, "Crepe", "Quần Ống Rộng Nanushka Cleo", 4, null, "quan-ong-rong-nanushka", null }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "ProductId", "AverageRating", "BasePrice", "Brand", "CategoryId", "CreatedAt", "DeletedAt", "Description", "GenderTarget", "IsActive", "IsBestSeller", "Material", "ProductName", "ReviewCount", "SalePrice", "Slug", "UpdatedAt" },
                values: new object[,]
                {
                    { 13, 4.5m, 360000m, "Palazzo", 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, true, true, "Tuyết mưa", "Quần Palazzo Cạp Cao Womens Wide", 19, 320000m, "quan-palazzo-cap-cao", null },
                    { 14, 4.8m, 390000m, "DenimBasic", 5, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, true, true, "Denim", "Quần Jean Basic Ống Rộng Cá Tính", 31, 350000m, "quan-jean-basic-ong-rong", null }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "ProductId", "AverageRating", "BasePrice", "Brand", "CategoryId", "CreatedAt", "DeletedAt", "Description", "GenderTarget", "IsActive", "Material", "ProductName", "ReviewCount", "SalePrice", "Slug", "UpdatedAt" },
                values: new object[,]
                {
                    { 15, 4.3m, 120000m, "Shop.Co", 4, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, true, "Cotton", "Áo Thun T-Shirt Basic", 5, 99000m, "ao-thun-t-shirt-basic", null },
                    { 16, 4.1m, 290000m, "SHEIN", 6, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, true, "Polyester", "Váy Đầm SHEIN", 3, null, "vay-dam-shein", null },
                    { 17, 4.6m, 250000m, "Moletom", 5, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, true, "Nỉ da cá", "Quần Nỉ calca moletom", 9, 199000m, "quan-ni-moletom", null }
                });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "IsNewsletterSubscribed",
                value: false);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 2,
                column: "IsNewsletterSubscribed",
                value: false);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 3,
                column: "IsNewsletterSubscribed",
                value: false);

            migrationBuilder.InsertData(
                table: "ProductImages",
                columns: new[] { "ImageId", "AltText", "ColorAnalysisJson", "ContentType", "CreatedAt", "DominantColorHex", "FileName", "FileSize", "ImageUrl", "IsThumbnail", "ProductId", "SortOrder", "VariantId" },
                values: new object[,]
                {
                    { 17, null, null, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, "/images/blue-plaid-oversized-jeans.jpg", true, 8, 1, null },
                    { 18, null, null, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, "/images/hm-pride-collection.jpg", true, 9, 1, null },
                    { 19, null, null, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, "/images/looks-beautiful-dress.jpg", true, 10, 1, null },
                    { 20, null, null, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, "/images/mens-pink-half-zip-jumper.jpg", true, 11, 1, null },
                    { 21, null, null, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, "/images/nanushka-wide-leg-pants.jpg", true, 12, 1, null },
                    { 22, null, null, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, "/images/womens-palazzo-pants.jpg", true, 13, 1, null },
                    { 23, null, null, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, "/images/quan-jean-basic-ong-rong.jpg", true, 14, 1, null },
                    { 24, null, null, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, "/images/t-shirt-basic.webp", true, 15, 1, null },
                    { 25, null, null, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, "/images/shein-dress.webp", true, 16, 1, null },
                    { 26, null, null, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, "/images/calca-moletom-pants.jpg", true, 17, 1, null }
                });

            migrationBuilder.InsertData(
                table: "ProductImages",
                columns: new[] { "ImageId", "AltText", "ColorAnalysisJson", "ContentType", "CreatedAt", "DominantColorHex", "FileName", "FileSize", "ImageUrl", "ProductId", "SortOrder", "VariantId" },
                values: new object[,]
                {
                    { 28, null, null, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, "/images/quan-jean-1.jpg", 8, 2, null },
                    { 29, null, null, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, "/images/mens-new-arrivals.jpg", 11, 2, null },
                    { 30, null, null, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, "/images/spring-summer-25.jpg", 10, 2, null }
                });

            migrationBuilder.InsertData(
                table: "ProductVariants",
                columns: new[] { "VariantId", "Barcode", "Color", "ColorHex", "CreatedAt", "IsActive", "LowStockThreshold", "OriginalPrice", "ProductId", "Size", "Sku", "StockQuantity", "UpdatedAt", "WeightGram" },
                values: new object[,]
                {
                    { 12, null, "Kẻ sọc xanh", "#4682B4", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 5, null, 8, "M", "ASM-PL-M", 50, null, null },
                    { 13, null, "Kẻ sọc xanh", "#4682B4", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 5, null, 8, "L", "ASM-PL-L", 50, null, null },
                    { 14, null, "Pride", "#FF1493", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 5, null, 9, "M", "AT-HM-M", 60, null, null },
                    { 15, null, "Pride", "#FF1493", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 5, null, 9, "L", "AT-HM-L", 60, null, null },
                    { 16, null, "Đỏ", "#8B0000", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 5, null, 10, "S", "V-LB-S", 30, null, null },
                    { 17, null, "Đỏ", "#8B0000", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 5, null, 10, "M", "V-LB-M", 30, null, null },
                    { 18, null, "Hồng", "#FFB6C1", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 5, null, 11, "M", "AL-HZ-M", 40, null, null },
                    { 19, null, "Hồng", "#FFB6C1", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 5, null, 11, "L", "AL-HZ-L", 40, null, null },
                    { 20, null, "Be", "#F5F5DC", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 5, null, 12, "M", "Q-NS-M", 30, null, null },
                    { 21, null, "Be", "#F5F5DC", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 5, null, 12, "L", "Q-NS-L", 30, null, null },
                    { 22, null, "Đen", "#000000", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 5, null, 13, "M", "Q-PZ-M", 50, null, null },
                    { 23, null, "Đen", "#000000", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 5, null, 13, "L", "Q-PZ-L", 50, null, null },
                    { 24, null, "Xanh nhạt", "#ADD8E6", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 5, null, 14, "30", "QJ-BS-30", 40, null, null },
                    { 25, null, "Xanh nhạt", "#ADD8E6", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 5, null, 14, "31", "QJ-BS-31", 40, null, null },
                    { 26, null, "Trắng", "#FFFFFF", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 5, null, 15, "M", "AT-BS-M", 100, null, null },
                    { 27, null, "Trắng", "#FFFFFF", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 5, null, 15, "L", "AT-BS-L", 100, null, null },
                    { 28, null, "Đen", "#000000", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 5, null, 16, "S", "V-SN-S", 30, null, null },
                    { 29, null, "Đen", "#000000", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 5, null, 16, "M", "V-SN-M", 30, null, null },
                    { 30, null, "Xám", "#808080", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 5, null, 17, "M", "Q-MT-M", 40, null, null },
                    { 31, null, "Xám", "#808080", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 5, null, 17, "L", "Q-MT-L", 40, null, null }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "ProductId", "AverageRating", "BasePrice", "Brand", "CategoryId", "CreatedAt", "DeletedAt", "Description", "GenderTarget", "IsActive", "IsFeatured", "Material", "ProductName", "ReviewCount", "SalePrice", "Slug", "UpdatedAt" },
                values: new object[] { 6, 4.7m, 450000m, "SlimSport", 8, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, true, true, "Lưới/Da", "Giày Thể Thao Nữ Slim Đế 2cm", 14, 390000m, "giay-the-thao-nu-slim", null });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "ProductId", "AverageRating", "BasePrice", "Brand", "CategoryId", "CreatedAt", "DeletedAt", "Description", "GenderTarget", "IsActive", "IsBestSeller", "Material", "ProductName", "ReviewCount", "SalePrice", "Slug", "UpdatedAt" },
                values: new object[] { 7, 4.9m, 850000m, "YinYang", 8, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, true, true, "Lưới thoáng khí", "Giày Bóng Rổ Cao Cấp Yin Yang", 38, 790000m, "giay-bong-ro-yin-yang", null });

            migrationBuilder.InsertData(
                table: "ProductImages",
                columns: new[] { "ImageId", "AltText", "ColorAnalysisJson", "ContentType", "CreatedAt", "DominantColorHex", "FileName", "FileSize", "ImageUrl", "IsThumbnail", "ProductId", "SortOrder", "VariantId" },
                values: new object[,]
                {
                    { 15, null, null, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, "/images/giay-the-thao-nu-slim.jpg", true, 6, 1, null },
                    { 16, null, null, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, "/images/giay-bong-ro-yin-yang.jpg", true, 7, 1, null }
                });

            migrationBuilder.InsertData(
                table: "ProductImages",
                columns: new[] { "ImageId", "AltText", "ColorAnalysisJson", "ContentType", "CreatedAt", "DominantColorHex", "FileName", "FileSize", "ImageUrl", "ProductId", "SortOrder", "VariantId" },
                values: new object[] { 27, null, null, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, "/images/zapatillas-sport-casual.jpg", 6, 2, null });

            migrationBuilder.InsertData(
                table: "ProductVariants",
                columns: new[] { "VariantId", "Barcode", "Color", "ColorHex", "CreatedAt", "IsActive", "LowStockThreshold", "OriginalPrice", "ProductId", "Size", "Sku", "StockQuantity", "UpdatedAt", "WeightGram" },
                values: new object[,]
                {
                    { 8, null, "Sọc", "#FF4500", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 5, null, 6, "38", "GTT-SLIM-38", 50, null, null },
                    { 9, null, "Sọc", "#FF4500", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 5, null, 6, "39", "GTT-SLIM-39", 50, null, null },
                    { 10, null, "Yin Yang", "#000000", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 5, null, 7, "40", "GBR-YY-40", 40, null, null },
                    { 11, null, "Yin Yang", "#FFFFFF", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 5, null, 7, "41", "GBR-YY-41", 40, null, null }
                });

            migrationBuilder.AddCheckConstraint(
                name: "CK_Users_Status",
                table: "Users",
                sql: "[Status] IN (N'Active', N'Locked', N'Deleted')");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Users_Status",
                table: "Users");

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "ImageId",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "ImageId",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "ImageId",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "ImageId",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "ImageId",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "ImageId",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "ImageId",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "ImageId",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "ImageId",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "ImageId",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "ImageId",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "ImageId",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "ImageId",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "ImageId",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "ImageId",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "ImageId",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "ImageId",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "ImageId",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "ImageId",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "ImageId",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "ImageId",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "ImageId",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "ImageId",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "VariantId",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "VariantId",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "VariantId",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "VariantId",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "VariantId",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "VariantId",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "VariantId",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "VariantId",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "VariantId",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "VariantId",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "VariantId",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "VariantId",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "VariantId",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "VariantId",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "VariantId",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "VariantId",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "VariantId",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "VariantId",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "VariantId",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "VariantId",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "VariantId",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "VariantId",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "VariantId",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "VariantId",
                keyValue: 31);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 8);

            migrationBuilder.DropColumn(
                name: "IsNewsletterSubscribed",
                table: "Users");

            migrationBuilder.UpdateData(
                table: "OrderItems",
                keyColumn: "OrderItemId",
                keyValue: 1,
                column: "ImageUrlSnapshot",
                value: "https://via.placeholder.com/600x800.png?text=Ao+Thun+Trang");

            migrationBuilder.UpdateData(
                table: "OrderItems",
                keyColumn: "OrderItemId",
                keyValue: 2,
                column: "ImageUrlSnapshot",
                value: "https://via.placeholder.com/600x800.png?text=Jeans+Denim");

            migrationBuilder.UpdateData(
                table: "OrderItems",
                keyColumn: "OrderItemId",
                keyValue: 3,
                column: "ImageUrlSnapshot",
                value: "https://via.placeholder.com/600x800.png?text=Vay+Hoa");

            migrationBuilder.UpdateData(
                table: "OrderItems",
                keyColumn: "OrderItemId",
                keyValue: 4,
                column: "ImageUrlSnapshot",
                value: "https://via.placeholder.com/600x800.png?text=Hoodie+Xam");

            migrationBuilder.UpdateData(
                table: "ProductImages",
                keyColumn: "ImageId",
                keyValue: 1,
                column: "ImageUrl",
                value: "https://via.placeholder.com/600x800.png?text=Ao+Thun+Trang");

            migrationBuilder.UpdateData(
                table: "ProductImages",
                keyColumn: "ImageId",
                keyValue: 2,
                column: "ImageUrl",
                value: "https://via.placeholder.com/600x800.png?text=Ao+Thun+Den");

            migrationBuilder.UpdateData(
                table: "ProductImages",
                keyColumn: "ImageId",
                keyValue: 3,
                column: "ImageUrl",
                value: "https://via.placeholder.com/600x800.png?text=Vay+Hoa");

            migrationBuilder.UpdateData(
                table: "ProductImages",
                keyColumn: "ImageId",
                keyValue: 4,
                column: "ImageUrl",
                value: "https://via.placeholder.com/600x800.png?text=Hoodie+Xam");

            migrationBuilder.UpdateData(
                table: "ProductImages",
                keyColumn: "ImageId",
                keyValue: 5,
                column: "ImageUrl",
                value: "https://via.placeholder.com/600x800.png?text=Jeans+Denim");

            migrationBuilder.UpdateData(
                table: "ProductImages",
                keyColumn: "ImageId",
                keyValue: 6,
                column: "ImageUrl",
                value: "https://via.placeholder.com/600x800.png?text=Jeans+Dam");

            migrationBuilder.UpdateData(
                table: "ProductImages",
                keyColumn: "ImageId",
                keyValue: 7,
                column: "ImageUrl",
                value: "https://via.placeholder.com/600x800.png?text=So+Mi+Trang");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Users_Status",
                table: "Users",
                sql: "[Status] IN (N'Unverified', N'Active', N'Locked', N'Deleted')");
        }
    }
}
