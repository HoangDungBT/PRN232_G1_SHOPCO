using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SHOP.CO.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Feature25_ShoppingCart : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 3,
                columns: new[] { "CategoryName", "ParentCategoryId", "Slug" },
                values: new object[] { "Unisex", null, "thoi-trang-unisex" });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "CategoryId", "CategoryName", "CreatedAt", "Description", "ImageUrl", "IsActive", "ParentCategoryId", "Slug", "UpdatedAt" },
                values: new object[,]
                {
                    { 4, "Áo Thun Nam", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, 1, "ao-thun-nam", null },
                    { 5, "Quần Jeans Nam", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, 1, "quan-jeans-nam", null },
                    { 6, "Váy Nữ", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, 2, "vay-nu", null },
                    { 7, "Áo Sơ Mi Nữ", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, 2, "ao-so-mi-nu", null }
                });

            migrationBuilder.UpdateData(
                table: "CommerceRecords",
                keyColumn: "RecordId",
                keyValue: 1,
                columns: new[] { "PaymentMethod", "PaymentProvider", "TransactionCode" },
                values: new object[] { "Online", "VNPay", "VNP123456" });

            migrationBuilder.UpdateData(
                table: "CommerceRecords",
                keyColumn: "RecordId",
                keyValue: 2,
                columns: new[] { "Code", "DiscountType", "DiscountValue" },
                values: new object[] { "FREESHIP19K", "Fixed", 19000m });

            migrationBuilder.UpdateData(
                table: "CommerceRecords",
                keyColumn: "RecordId",
                keyValue: 3,
                columns: new[] { "Amount", "PaymentMethod", "Status" },
                values: new object[] { 300000m, "COD", "Pending" });

            migrationBuilder.InsertData(
                table: "CommerceRecords",
                columns: new[] { "RecordId", "Amount", "Code", "CreatedAt", "DiscountType", "DiscountValue", "EndAt", "MaxDiscountAmount", "MinOrderAmount", "Name", "OrderId", "PayloadJson", "PaymentMethod", "PaymentProvider", "ProductId", "RecordType", "StartAt", "Status", "TransactionCode", "UpdatedAt", "UsageLimit", "UsedCount", "UserId", "VariantId" },
                values: new object[] { 4, null, "WELCOME50", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Fixed", 50000m, new DateTime(2024, 2, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 200000m, "Giảm 50K cho thành viên mới", null, null, null, null, null, "Voucher", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Active", null, null, 1000, 150, null, null });

            migrationBuilder.InsertData(
                table: "CommerceRecords",
                columns: new[] { "RecordId", "Amount", "Code", "CreatedAt", "DiscountType", "DiscountValue", "EndAt", "MaxDiscountAmount", "MinOrderAmount", "Name", "OrderId", "PayloadJson", "PaymentMethod", "PaymentProvider", "ProductId", "RecordType", "StartAt", "Status", "TransactionCode", "UpdatedAt", "UsageLimit", "UserId", "VariantId" },
                values: new object[] { 5, null, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, new DateTime(2024, 1, 2, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "Sale 11.11", null, "{\"discountPercent\": 10}", null, null, null, "FlashSale", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Active", null, null, null, null, null });



            migrationBuilder.InsertData(
                table: "CustomerActivities",
                columns: new[] { "ActivityId", "ActivityType", "Comment", "CreatedAt", "InputJson", "IpAddress", "IsActive", "Keyword", "OrderItemId", "ProductId", "Rating", "ResultJson", "SessionId", "UpdatedAt", "UserId", "VariantId" },
                values: new object[,]
                {
                    { 4, "RecentlyViewed", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, null, null, 2, null, null, null, null, 3, null },
                    { 5, "AiColorSearch", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "{\"imageUrl\": \"/uploads/user-search-1.jpg\"}", null, true, null, null, null, null, "{\"dominantColor\": \"#FF0000\", \"matchScore\": 95}", null, null, 3, null }
                });

            migrationBuilder.UpdateData(
                table: "InteractionLogs",
                keyColumn: "LogId",
                keyValue: 1,
                columns: new[] { "Message", "SessionId" },
                values: new object[] { "Cho tôi hỏi quần jeans size 30 còn hàng không?", "sess_123" });

            migrationBuilder.UpdateData(
                table: "InteractionLogs",
                keyColumn: "LogId",
                keyValue: 2,
                columns: new[] { "EntitiesJson", "IntentName", "Message", "SessionId" },
                values: new object[] { "{\"product\": \"quần jeans\", \"size\": \"30\"}", "check_stock", "Dạ, quần jeans nam slimfit size 30 hiện còn 40 sản phẩm ạ.", "sess_123" });

            migrationBuilder.UpdateData(
                table: "InteractionLogs",
                keyColumn: "LogId",
                keyValue: 3,
                columns: new[] { "Message", "OrderId", "Status" },
                values: new object[] { "Đơn hàng ORD-0003 của bạn đã được giao cho đơn vị vận chuyển.", 3, "Sent" });

            migrationBuilder.InsertData(
                table: "InteractionLogs",
                columns: new[] { "LogId", "ActionName", "CreatedAt", "EntitiesJson", "IntentName", "LogType", "Message", "NewValueJson", "OldValueJson", "OrderId", "PayloadJson", "ProductId", "QuantityChanged", "ReadAt", "ReferenceId", "ReferenceType", "SenderType", "SessionId", "Status", "Title", "UserId", "VariantId" },
                values: new object[] { 5, "UpdateProductPrice", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "Audit", null, "{\"price\": 399000}", "{\"price\": 400000}", null, null, null, null, null, null, null, "Admin", null, null, "Cập nhật giá sản phẩm", 1, null });



            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "OrderId",
                keyValue: 1,
                columns: new[] { "AddressId", "OrderCode", "ShippingStatus", "SubtotalAmount" },
                values: new object[] { 1, "ORD-0001", "Completed", 240000m });

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "OrderId",
                keyValue: 2,
                columns: new[] { "AddressId", "DiscountAmount", "OrderCode", "ReceiverName", "SubtotalAmount", "TotalAmount" },
                values: new object[] { 2, 19000m, "ORD-0002", "Nguyễn Văn A (Công ty)", 399000m, 380000m });

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "OrderId",
                keyValue: 3,
                columns: new[] { "AddressId", "OrderCode", "SubtotalAmount", "TotalAmount" },
                values: new object[] { 3, "ORD-0003", 300000m, 300000m });

            migrationBuilder.InsertData(
                table: "Orders",
                columns: new[] { "OrderId", "AddressId", "CancelReason", "CanceledAt", "CompletedAt", "CreatedAt", "CustomerNote", "OrderCode", "OrderStatus", "PaymentStatus", "ReceiverName", "ReceiverPhone", "ShippingAddressText", "ShippingStatus", "StaffNote", "SubtotalAmount", "TotalAmount", "UpdatedAt", "UserId" },
                values: new object[] { 4, 3, "Đổi ý", null, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "ORD-0004", "Canceled", "Refunded", "Trần Thị B", "0987654321", "789 Hai Bà Trưng, Quận 3, TP.HCM", "NotShipped", null, 400000m, 400000m, null, 3 });

            migrationBuilder.UpdateData(
                table: "ProductImages",
                keyColumn: "ImageId",
                keyValue: 1,
                columns: new[] { "ImageUrl", "SortOrder", "VariantId" },
                values: new object[] { "https://via.placeholder.com/600x800.png?text=Ao+Thun+Trang", 1, 1 });

            migrationBuilder.UpdateData(
                table: "ProductImages",
                keyColumn: "ImageId",
                keyValue: 2,
                columns: new[] { "ImageUrl", "IsThumbnail", "ProductId", "SortOrder", "VariantId" },
                values: new object[] { "https://via.placeholder.com/600x800.png?text=Ao+Thun+Den", false, 1, 2, 2 });

            migrationBuilder.UpdateData(
                table: "ProductImages",
                keyColumn: "ImageId",
                keyValue: 3,
                columns: new[] { "ImageUrl", "ProductId", "SortOrder" },
                values: new object[] { "https://via.placeholder.com/600x800.png?text=Vay+Hoa", 2, 1 });

            migrationBuilder.InsertData(
                table: "ProductImages",
                columns: new[] { "ImageId", "AltText", "ColorAnalysisJson", "ContentType", "CreatedAt", "DominantColorHex", "FileName", "FileSize", "ImageUrl", "IsThumbnail", "ProductId", "SortOrder", "VariantId" },
                values: new object[] { 4, null, null, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, "https://via.placeholder.com/600x800.png?text=Hoodie+Xam", true, 3, 1, null });

            migrationBuilder.UpdateData(
                table: "ProductVariants",
                keyColumn: "VariantId",
                keyValue: 1,
                column: "ColorHex",
                value: "#FFFFFF");

            migrationBuilder.UpdateData(
                table: "ProductVariants",
                keyColumn: "VariantId",
                keyValue: 2,
                column: "ColorHex",
                value: "#000000");

            migrationBuilder.UpdateData(
                table: "ProductVariants",
                keyColumn: "VariantId",
                keyValue: 3,
                columns: new[] { "ColorHex", "LowStockThreshold" },
                values: new object[] { "#FF0000", 2 });

            migrationBuilder.UpdateData(
                table: "ProductVariants",
                keyColumn: "VariantId",
                keyValue: 4,
                columns: new[] { "ColorHex", "LowStockThreshold" },
                values: new object[] { "#808080", 10 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 1,
                columns: new[] { "AverageRating", "Brand", "CategoryId", "IsBestSeller", "Material", "ReviewCount" },
                values: new object[] { 4.5m, "Shop.Co", 4, true, "Cotton", 10 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 2,
                columns: new[] { "AverageRating", "Brand", "CategoryId", "IsNewArrival", "Material", "ReviewCount" },
                values: new object[] { 4.8m, "Shop.Co", 6, true, "Voan", 25 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 3,
                columns: new[] { "AverageRating", "Brand", "CategoryId", "IsFeatured", "Material", "ReviewCount" },
                values: new object[] { 4.2m, "Shop.Co", 3, true, "Nỉ", 5 });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 2,
                column: "Gender",
                value: "Nam");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 3,
                column: "Gender",
                value: "Nữ");

            migrationBuilder.InsertData(
                table: "OrderItems",
                columns: new[] { "OrderItemId", "ColorSnapshot", "CreatedAt", "ImageUrlSnapshot", "LineTotal", "OrderId", "ProductId", "ProductNameSnapshot", "Quantity", "ReviewStatus", "SalePrice", "SizeSnapshot", "SkuSnapshot", "UnitPrice", "UpdatedAt", "VariantId" },
                values: new object[] { 4, "Xám", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "https://via.placeholder.com/600x800.png?text=Hoodie+Xam", 400000m, 4, 3, "Áo Khoác Hoodie Unisex", 1, "NotReviewed", 400000m, "XL", "AKH-XA-XL", 400000m, null, 4 });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "ProductId", "AverageRating", "BasePrice", "Brand", "CategoryId", "CreatedAt", "DeletedAt", "Description", "GenderTarget", "IsActive", "IsBestSeller", "Material", "ProductName", "ReviewCount", "SalePrice", "Slug", "UpdatedAt" },
                values: new object[] { 4, 4.9m, 450000m, "DenimX", 5, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, true, true, "Denim", "Quần Jeans Nam Slimfit", 50, 399000m, "quan-jeans-nam-slimfit", null });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "ProductId", "AverageRating", "BasePrice", "Brand", "CategoryId", "CreatedAt", "DeletedAt", "Description", "GenderTarget", "IsActive", "Material", "ProductName", "ReviewCount", "SalePrice", "Slug", "UpdatedAt" },
                values: new object[] { 5, 4.0m, 250000m, "OfficeWear", 7, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, true, "Lụa", "Áo Sơ Mi Lụa Công Sở", 2, null, "ao-so-mi-lua-cong-so", null });

            migrationBuilder.InsertData(
                table: "ProductImages",
                columns: new[] { "ImageId", "AltText", "ColorAnalysisJson", "ContentType", "CreatedAt", "DominantColorHex", "FileName", "FileSize", "ImageUrl", "IsThumbnail", "ProductId", "SortOrder", "VariantId" },
                values: new object[] { 7, null, null, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, "https://via.placeholder.com/600x800.png?text=So+Mi+Trang", true, 5, 1, null });

            migrationBuilder.InsertData(
                table: "ProductVariants",
                columns: new[] { "VariantId", "Barcode", "Color", "ColorHex", "CreatedAt", "IsActive", "LowStockThreshold", "OriginalPrice", "ProductId", "Size", "Sku", "StockQuantity", "UpdatedAt", "WeightGram" },
                values: new object[,]
                {
                    { 5, null, "Xanh Denim", "#1560BD", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 5, null, 4, "30", "QJN-XANH-30", 40, null, null },
                    { 6, null, "Xanh Đậm", "#00008B", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 5, null, 4, "31", "QJN-XANH-31", 3, null, null },
                    { 7, null, "Trắng", "#FFFFFF", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 5, null, 5, "M", "ASM-TR-M", 15, null, null }
                });

            migrationBuilder.InsertData(
                table: "InteractionLogs",
                columns: new[] { "LogId", "ActionName", "CreatedAt", "EntitiesJson", "IntentName", "LogType", "Message", "NewValueJson", "OldValueJson", "OrderId", "PayloadJson", "ProductId", "QuantityChanged", "ReadAt", "ReferenceId", "ReferenceType", "SenderType", "SessionId", "Status", "Title", "UserId", "VariantId" },
                values: new object[] { 4, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "StockAlert", "Biến thể QJN-XANH-31 chỉ còn 3 sản phẩm trong kho.", null, null, null, null, 4, null, null, null, null, "System", null, null, "Cảnh báo tồn kho thấp", 1, 6 });

            migrationBuilder.InsertData(
                table: "ProductImages",
                columns: new[] { "ImageId", "AltText", "ColorAnalysisJson", "ContentType", "CreatedAt", "DominantColorHex", "FileName", "FileSize", "ImageUrl", "IsThumbnail", "ProductId", "SortOrder", "VariantId" },
                values: new object[] { 5, null, null, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, "https://via.placeholder.com/600x800.png?text=Jeans+Denim", true, 4, 1, 5 });

            migrationBuilder.InsertData(
                table: "ProductImages",
                columns: new[] { "ImageId", "AltText", "ColorAnalysisJson", "ContentType", "CreatedAt", "DominantColorHex", "FileName", "FileSize", "ImageUrl", "ProductId", "SortOrder", "VariantId" },
                values: new object[] { 6, null, null, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, "https://via.placeholder.com/600x800.png?text=Jeans+Dam", 4, 2, 6 });

            migrationBuilder.UpdateData(
                table: "CustomerActivities",
                keyColumn: "ActivityId",
                keyValue: 1,
                columns: new[] { "Comment", "OrderItemId" },
                values: new object[] { "Áo chất lượng rất tốt, form chuẩn.", 1 });

            migrationBuilder.UpdateData(
                table: "CustomerActivities",
                keyColumn: "ActivityId",
                keyValue: 2,
                column: "ProductId",
                value: 5);

            migrationBuilder.UpdateData(
                table: "CustomerActivities",
                keyColumn: "ActivityId",
                keyValue: 3,
                column: "Keyword",
                value: "quần jeans nam");

            migrationBuilder.UpdateData(
                table: "OrderItems",
                keyColumn: "OrderItemId",
                keyValue: 1,
                columns: new[] { "ColorSnapshot", "ImageUrlSnapshot", "ReviewStatus", "SizeSnapshot" },
                values: new object[] { "Trắng", "https://via.placeholder.com/600x800.png?text=Ao+Thun+Trang", "Reviewed", "M" });

            migrationBuilder.UpdateData(
                table: "OrderItems",
                keyColumn: "OrderItemId",
                keyValue: 2,
                columns: new[] { "ColorSnapshot", "ImageUrlSnapshot", "LineTotal", "ProductId", "ProductNameSnapshot", "SalePrice", "SizeSnapshot", "SkuSnapshot", "UnitPrice", "VariantId" },
                values: new object[] { "Xanh Denim", "https://via.placeholder.com/600x800.png?text=Jeans+Denim", 399000m, 4, "Quần Jeans Nam Slimfit", 399000m, "30", "QJN-XANH-30", 450000m, 5 });

            migrationBuilder.UpdateData(
                table: "OrderItems",
                keyColumn: "OrderItemId",
                keyValue: 3,
                columns: new[] { "ColorSnapshot", "ImageUrlSnapshot", "LineTotal", "ProductId", "ProductNameSnapshot", "SalePrice", "SizeSnapshot", "SkuSnapshot", "UnitPrice", "VariantId" },
                values: new object[] { "Đỏ", "https://via.placeholder.com/600x800.png?text=Vay+Hoa", 300000m, 2, "Váy Hoa Mùa Hè", 300000m, "S", "VHM-DO-S", 350000m, 3 });

            migrationBuilder.UpdateData(
                table: "CartItems",
                keyColumn: "CartItemId",
                keyValue: 1,
                columns: new[] { "SelectedColor", "SelectedSize" },
                values: new object[] { "Trắng", "M" });

            migrationBuilder.UpdateData(
                table: "CartItems",
                keyColumn: "CartItemId",
                keyValue: 2,
                columns: new[] { "SelectedColor", "SelectedSize", "UnitPrice", "VariantId" },
                values: new object[] { "Xanh Denim", "30", 399000m, 5 });

            migrationBuilder.UpdateData(
                table: "CartItems",
                keyColumn: "CartItemId",
                keyValue: 3,
                columns: new[] { "SelectedColor", "SelectedSize", "UnitPrice", "VariantId" },
                values: new object[] { "Trắng", "M", 250000m, 7 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "CommerceRecords",
                keyColumn: "RecordId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "CommerceRecords",
                keyColumn: "RecordId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "CustomerActivities",
                keyColumn: "ActivityId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "CustomerActivities",
                keyColumn: "ActivityId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "InteractionLogs",
                keyColumn: "LogId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "InteractionLogs",
                keyColumn: "LogId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "OrderItems",
                keyColumn: "OrderItemId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "ImageId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "ImageId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "ImageId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "ImageId",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "VariantId",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Orders",
                keyColumn: "OrderId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "VariantId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "VariantId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 5);

            migrationBuilder.UpdateData(
                table: "CartItems",
                keyColumn: "CartItemId",
                keyValue: 1,
                columns: new[] { "SelectedColor", "SelectedSize" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "CartItems",
                keyColumn: "CartItemId",
                keyValue: 2,
                columns: new[] { "SelectedColor", "SelectedSize", "UnitPrice", "VariantId" },
                values: new object[] { null, null, 300000m, 3 });

            migrationBuilder.UpdateData(
                table: "CartItems",
                keyColumn: "CartItemId",
                keyValue: 3,
                columns: new[] { "SelectedColor", "SelectedSize", "UnitPrice", "VariantId" },
                values: new object[] { null, null, 400000m, 4 });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 3,
                columns: new[] { "CategoryName", "ParentCategoryId", "Slug" },
                values: new object[] { "Áo Thun Nam", 1, "ao-thun-nam" });

            migrationBuilder.UpdateData(
                table: "CommerceRecords",
                keyColumn: "RecordId",
                keyValue: 1,
                columns: new[] { "PaymentMethod", "PaymentProvider", "TransactionCode" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "CommerceRecords",
                keyColumn: "RecordId",
                keyValue: 2,
                columns: new[] { "Code", "DiscountType", "DiscountValue" },
                values: new object[] { "GIAM10K", null, 10000m });

            migrationBuilder.UpdateData(
                table: "CommerceRecords",
                keyColumn: "RecordId",
                keyValue: 3,
                columns: new[] { "Amount", "PaymentMethod", "Status" },
                values: new object[] { 400000m, null, "Success" });

            migrationBuilder.UpdateData(
                table: "CustomerActivities",
                keyColumn: "ActivityId",
                keyValue: 1,
                columns: new[] { "Comment", "OrderItemId" },
                values: new object[] { "Áo mặc mát mẻ, đẹp", null });

            migrationBuilder.UpdateData(
                table: "CustomerActivities",
                keyColumn: "ActivityId",
                keyValue: 2,
                column: "ProductId",
                value: 2);

            migrationBuilder.UpdateData(
                table: "CustomerActivities",
                keyColumn: "ActivityId",
                keyValue: 3,
                column: "Keyword",
                value: "Áo khoác mùa đông");

            migrationBuilder.UpdateData(
                table: "InteractionLogs",
                keyColumn: "LogId",
                keyValue: 1,
                columns: new[] { "Message", "SessionId" },
                values: new object[] { "Cho tôi hỏi size áo thun", null });

            migrationBuilder.UpdateData(
                table: "InteractionLogs",
                keyColumn: "LogId",
                keyValue: 2,
                columns: new[] { "EntitiesJson", "IntentName", "Message", "SessionId" },
                values: new object[] { null, null, "Dạ, size M phù hợp với người từ 50-60kg ạ.", null });

            migrationBuilder.UpdateData(
                table: "InteractionLogs",
                keyColumn: "LogId",
                keyValue: 3,
                columns: new[] { "Message", "OrderId", "Status" },
                values: new object[] { "Đơn hàng ORD003 của bạn đang được giao", null, null });

            migrationBuilder.UpdateData(
                table: "OrderItems",
                keyColumn: "OrderItemId",
                keyValue: 1,
                columns: new[] { "ColorSnapshot", "ImageUrlSnapshot", "ReviewStatus", "SizeSnapshot" },
                values: new object[] { null, null, "NotReviewed", null });

            migrationBuilder.UpdateData(
                table: "OrderItems",
                keyColumn: "OrderItemId",
                keyValue: 2,
                columns: new[] { "ColorSnapshot", "ImageUrlSnapshot", "LineTotal", "ProductId", "ProductNameSnapshot", "SalePrice", "SizeSnapshot", "SkuSnapshot", "UnitPrice", "VariantId" },
                values: new object[] { null, null, 300000m, 2, "Váy Hoa Mùa Hè", 300000m, null, "VHM-DO-S", 350000m, 3 });

            migrationBuilder.UpdateData(
                table: "OrderItems",
                keyColumn: "OrderItemId",
                keyValue: 3,
                columns: new[] { "ColorSnapshot", "ImageUrlSnapshot", "LineTotal", "ProductId", "ProductNameSnapshot", "SalePrice", "SizeSnapshot", "SkuSnapshot", "UnitPrice", "VariantId" },
                values: new object[] { null, null, 400000m, 3, "Áo Khoác Hoodie Unisex", 400000m, null, "AKH-XA-XL", 400000m, 4 });

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "OrderId",
                keyValue: 1,
                columns: new[] { "AddressId", "OrderCode", "ShippingStatus", "SubtotalAmount" },
                values: new object[] { null, "ORD001", "Delivered", 0m });

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "OrderId",
                keyValue: 2,
                columns: new[] { "AddressId", "DiscountAmount", "OrderCode", "ReceiverName", "SubtotalAmount", "TotalAmount" },
                values: new object[] { null, 0m, "ORD002", "Nguyễn Văn A", 0m, 300000m });

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "OrderId",
                keyValue: 3,
                columns: new[] { "AddressId", "OrderCode", "SubtotalAmount", "TotalAmount" },
                values: new object[] { null, "ORD003", 0m, 400000m });

            migrationBuilder.UpdateData(
                table: "ProductImages",
                keyColumn: "ImageId",
                keyValue: 1,
                columns: new[] { "ImageUrl", "SortOrder", "VariantId" },
                values: new object[] { "/images/ao-thun-trang.jpg", 0, null });

            migrationBuilder.UpdateData(
                table: "ProductImages",
                keyColumn: "ImageId",
                keyValue: 2,
                columns: new[] { "ImageUrl", "IsThumbnail", "ProductId", "SortOrder", "VariantId" },
                values: new object[] { "/images/vay-hoa.jpg", true, 2, 0, null });

            migrationBuilder.UpdateData(
                table: "ProductImages",
                keyColumn: "ImageId",
                keyValue: 3,
                columns: new[] { "ImageUrl", "ProductId", "SortOrder" },
                values: new object[] { "/images/hoodie-xam.jpg", 3, 0 });

            migrationBuilder.UpdateData(
                table: "ProductVariants",
                keyColumn: "VariantId",
                keyValue: 1,
                column: "ColorHex",
                value: null);

            migrationBuilder.UpdateData(
                table: "ProductVariants",
                keyColumn: "VariantId",
                keyValue: 2,
                column: "ColorHex",
                value: null);

            migrationBuilder.UpdateData(
                table: "ProductVariants",
                keyColumn: "VariantId",
                keyValue: 3,
                columns: new[] { "ColorHex", "LowStockThreshold" },
                values: new object[] { null, 5 });

            migrationBuilder.UpdateData(
                table: "ProductVariants",
                keyColumn: "VariantId",
                keyValue: 4,
                columns: new[] { "ColorHex", "LowStockThreshold" },
                values: new object[] { null, 5 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 1,
                columns: new[] { "AverageRating", "Brand", "CategoryId", "IsBestSeller", "Material", "ReviewCount" },
                values: new object[] { 0m, null, 3, false, null, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 2,
                columns: new[] { "AverageRating", "Brand", "CategoryId", "IsNewArrival", "Material", "ReviewCount" },
                values: new object[] { 0m, null, 2, false, null, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 3,
                columns: new[] { "AverageRating", "Brand", "CategoryId", "IsFeatured", "Material", "ReviewCount" },
                values: new object[] { 0m, null, 1, false, null, 0 });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 2,
                column: "Gender",
                value: null);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 3,
                column: "Gender",
                value: null);
        }
    }
}
