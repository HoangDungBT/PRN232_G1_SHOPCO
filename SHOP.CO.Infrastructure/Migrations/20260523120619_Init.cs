using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SHOP.CO.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ParentCategoryId = table.Column<int>(type: "int", nullable: true),
                    CategoryName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    SortOrder = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSDATETIME()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.CategoryId);
                    table.ForeignKey(
                        name: "FK_Categories_Categories_ParentCategoryId",
                        column: x => x.ParentCategoryId,
                        principalTable: "Categories",
                        principalColumn: "CategoryId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    AvatarUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Role = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false, defaultValue: "Customer"),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false, defaultValue: "Active"),
                    Gender = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    DateOfBirth = table.Column<DateOnly>(type: "date", nullable: true),
                    PreferredSize = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PreferredStyle = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ResetPasswordToken = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ResetPasswordExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RefreshToken = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    RefreshTokenExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastLoginAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSDATETIME()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                    table.CheckConstraint("CK_Users_Role", "[Role] IN (N'Customer', N'Staff', N'Admin')");
                    table.CheckConstraint("CK_Users_Status", "[Status] IN (N'Active', N'Locked', N'Deleted')");
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    ProductId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    ProductName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Brand = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Material = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    GenderTarget = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    BasePrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SalePrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    AverageRating = table.Column<decimal>(type: "decimal(3,2)", nullable: false, defaultValue: 0m),
                    ReviewCount = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    ViewCount = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    IsFeatured = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsBestSeller = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsNewArrival = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSDATETIME()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.ProductId);
                    table.CheckConstraint("CK_Products_BasePrice", "[BasePrice] >= 0");
                    table.CheckConstraint("CK_Products_Rating", "[AverageRating] >= 0 AND [AverageRating] <= 5");
                    table.CheckConstraint("CK_Products_SalePrice", "[SalePrice] IS NULL OR [SalePrice] >= 0");
                    table.ForeignKey(
                        name: "FK_Products_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "CategoryId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserAddresses",
                columns: table => new
                {
                    AddressId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ReceiverName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    ReceiverPhone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Province = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    District = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Ward = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    StreetAddress = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    PostalCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSDATETIME()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAddresses", x => x.AddressId);
                    table.ForeignKey(
                        name: "FK_UserAddresses_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductVariants",
                columns: table => new
                {
                    VariantId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Sku = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Size = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Color = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ColorHex = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ExtraPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    OriginalPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    StockQuantity = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    LowStockThreshold = table.Column<int>(type: "int", nullable: false, defaultValue: 5),
                    Barcode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    WeightGram = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSDATETIME()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductVariants", x => x.VariantId);
                    table.CheckConstraint("CK_ProductVariants_LowStock", "[LowStockThreshold] >= 0");
                    table.CheckConstraint("CK_ProductVariants_Stock", "[StockQuantity] >= 0");
                    table.ForeignKey(
                        name: "FK_ProductVariants_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    OrderId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    OrderCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AddressId = table.Column<int>(type: "int", nullable: true),
                    ReceiverName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    ReceiverPhone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ShippingAddressText = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    OrderStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "Pending"),
                    PaymentStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "Unpaid"),
                    ShippingStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "NotShipped"),
                    SubtotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    DiscountAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    ShippingFee = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    CustomerNote = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    StaffNote = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CancelReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSDATETIME()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CanceledAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.OrderId);
                    table.CheckConstraint("CK_Orders_OrderStatus", "[OrderStatus] IN (N'Pending', N'Confirmed', N'Shipping', N'Completed', N'Canceled')");
                    table.CheckConstraint("CK_Orders_PaymentStatus", "[PaymentStatus] IN (N'Unpaid', N'Paid', N'Failed', N'Refunded')");
                    table.CheckConstraint("CK_Orders_TotalAmount", "[TotalAmount] >= 0");
                    table.ForeignKey(
                        name: "FK_Orders_UserAddresses_AddressId",
                        column: x => x.AddressId,
                        principalTable: "UserAddresses",
                        principalColumn: "AddressId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Orders_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CartItems",
                columns: table => new
                {
                    CartItemId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    SessionId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    VariantId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SelectedSize = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    SelectedColor = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsSelected = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSDATETIME()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartItems", x => x.CartItemId);
                    table.CheckConstraint("CK_CartItems_Quantity", "[Quantity] > 0");
                    table.CheckConstraint("CK_CartItems_UserOrSession", "[UserId] IS NOT NULL OR [SessionId] IS NOT NULL");
                    table.ForeignKey(
                        name: "FK_CartItems_ProductVariants_VariantId",
                        column: x => x.VariantId,
                        principalTable: "ProductVariants",
                        principalColumn: "VariantId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CartItems_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductImages",
                columns: table => new
                {
                    ImageId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    VariantId = table.Column<int>(type: "int", nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    AltText = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    IsThumbnail = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    FileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    FileSize = table.Column<long>(type: "bigint", nullable: true),
                    ContentType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DominantColorHex = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ColorAnalysisJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductImages", x => x.ImageId);
                    table.CheckConstraint("CK_ProductImages_ColorAnalysisJson", "[ColorAnalysisJson] IS NULL OR ISJSON([ColorAnalysisJson]) = 1");
                    table.ForeignKey(
                        name: "FK_ProductImages_ProductVariants_VariantId",
                        column: x => x.VariantId,
                        principalTable: "ProductVariants",
                        principalColumn: "VariantId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductImages_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CommerceRecords",
                columns: table => new
                {
                    RecordId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    OrderId = table.Column<int>(type: "int", nullable: true),
                    ProductId = table.Column<int>(type: "int", nullable: true),
                    VariantId = table.Column<int>(type: "int", nullable: true),
                    RecordType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PaymentMethod = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PaymentProvider = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TransactionCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    DiscountType = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    DiscountValue = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    MaxDiscountAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    MinOrderAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    UsageLimit = table.Column<int>(type: "int", nullable: true),
                    UsedCount = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    StartAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "Active"),
                    PayloadJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSDATETIME()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommerceRecords", x => x.RecordId);
                    table.CheckConstraint("CK_CommerceRecords_DiscountType", "[DiscountType] IS NULL OR [DiscountType] IN (N'Percent', N'Fixed')");
                    table.CheckConstraint("CK_CommerceRecords_PayloadJson", "[PayloadJson] IS NULL OR ISJSON([PayloadJson]) = 1");
                    table.CheckConstraint("CK_CommerceRecords_PaymentMethod", "[PaymentMethod] IS NULL OR [PaymentMethod] IN (N'COD', N'Online')");
                    table.CheckConstraint("CK_CommerceRecords_Type", "[RecordType] IN (N'Payment', N'Voucher', N'VoucherUsage', N'FlashSale', N'HotDeal')");
                    table.ForeignKey(
                        name: "FK_CommerceRecords_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "OrderId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CommerceRecords_ProductVariants_VariantId",
                        column: x => x.VariantId,
                        principalTable: "ProductVariants",
                        principalColumn: "VariantId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CommerceRecords_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CommerceRecords_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InteractionLogs",
                columns: table => new
                {
                    LogId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    OrderId = table.Column<int>(type: "int", nullable: true),
                    ProductId = table.Column<int>(type: "int", nullable: true),
                    VariantId = table.Column<int>(type: "int", nullable: true),
                    SessionId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    LogType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SenderType = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    IntentName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    EntitiesJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ActionName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    OldValueJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NewValueJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    QuantityChanged = table.Column<int>(type: "int", nullable: true),
                    ReferenceType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ReferenceId = table.Column<int>(type: "int", nullable: true),
                    IsRead = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    ReadAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PayloadJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InteractionLogs", x => x.LogId);
                    table.CheckConstraint("CK_InteractionLogs_EntitiesJson", "[EntitiesJson] IS NULL OR ISJSON([EntitiesJson]) = 1");
                    table.CheckConstraint("CK_InteractionLogs_NewValueJson", "[NewValueJson] IS NULL OR ISJSON([NewValueJson]) = 1");
                    table.CheckConstraint("CK_InteractionLogs_OldValueJson", "[OldValueJson] IS NULL OR ISJSON([OldValueJson]) = 1");
                    table.CheckConstraint("CK_InteractionLogs_PayloadJson", "[PayloadJson] IS NULL OR ISJSON([PayloadJson]) = 1");
                    table.CheckConstraint("CK_InteractionLogs_SenderType", "[SenderType] IS NULL OR [SenderType] IN (N'User', N'Bot', N'System', N'Admin')");
                    table.CheckConstraint("CK_InteractionLogs_Type", "[LogType] IN (N'Chatbot', N'Notification', N'Email', N'Audit', N'StockMovement', N'StockAlert', N'ReportExport')");
                    table.ForeignKey(
                        name: "FK_InteractionLogs_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "OrderId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InteractionLogs_ProductVariants_VariantId",
                        column: x => x.VariantId,
                        principalTable: "ProductVariants",
                        principalColumn: "VariantId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InteractionLogs_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InteractionLogs_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "OrderItems",
                columns: table => new
                {
                    OrderItemId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    VariantId = table.Column<int>(type: "int", nullable: false),
                    ProductNameSnapshot = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SkuSnapshot = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SizeSnapshot = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ColorSnapshot = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ImageUrlSnapshot = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SalePrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    DiscountAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    LineTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ReviewStatus = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false, defaultValue: "NotReviewed"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSDATETIME()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItems", x => x.OrderItemId);
                    table.CheckConstraint("CK_OrderItems_LineTotal", "[LineTotal] >= 0");
                    table.CheckConstraint("CK_OrderItems_Quantity", "[Quantity] > 0");
                    table.CheckConstraint("CK_OrderItems_ReviewStatus", "[ReviewStatus] IN (N'NotReviewed', N'Reviewed')");
                    table.ForeignKey(
                        name: "FK_OrderItems_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "OrderId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderItems_ProductVariants_VariantId",
                        column: x => x.VariantId,
                        principalTable: "ProductVariants",
                        principalColumn: "VariantId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderItems_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CustomerActivities",
                columns: table => new
                {
                    ActivityId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    ProductId = table.Column<int>(type: "int", nullable: true),
                    VariantId = table.Column<int>(type: "int", nullable: true),
                    OrderItemId = table.Column<int>(type: "int", nullable: true),
                    ActivityType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: true),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Keyword = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    InputJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResultJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SessionId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IpAddress = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSDATETIME()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerActivities", x => x.ActivityId);
                    table.CheckConstraint("CK_CustomerActivities_InputJson", "[InputJson] IS NULL OR ISJSON([InputJson]) = 1");
                    table.CheckConstraint("CK_CustomerActivities_Rating", "[Rating] IS NULL OR ([Rating] >= 1 AND [Rating] <= 5)");
                    table.CheckConstraint("CK_CustomerActivities_ResultJson", "[ResultJson] IS NULL OR ISJSON([ResultJson]) = 1");
                    table.CheckConstraint("CK_CustomerActivities_Type", "[ActivityType] IN (N'Wishlist', N'Review', N'RecentlyViewed', N'Search', N'AiSize', N'AiOutfit', N'AiColorSearch')");
                    table.ForeignKey(
                        name: "FK_CustomerActivities_OrderItems_OrderItemId",
                        column: x => x.OrderItemId,
                        principalTable: "OrderItems",
                        principalColumn: "OrderItemId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CustomerActivities_ProductVariants_VariantId",
                        column: x => x.VariantId,
                        principalTable: "ProductVariants",
                        principalColumn: "VariantId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CustomerActivities_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CustomerActivities_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "CategoryId", "CategoryName", "CreatedAt", "Description", "ImageUrl", "IsActive", "ParentCategoryId", "Slug", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, "Thời trang Nam", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, null, "thoi-trang-nam", null },
                    { 2, "Thời trang Nữ", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, null, "thoi-trang-nu", null }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "AvatarUrl", "CreatedAt", "DateOfBirth", "Email", "FullName", "Gender", "LastLoginAt", "PasswordHash", "Phone", "PreferredSize", "PreferredStyle", "RefreshToken", "RefreshTokenExpiresAt", "ResetPasswordExpiresAt", "ResetPasswordToken", "Role", "Status", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "admin@shop.co", "Admin System", null, null, "hashed123", null, null, null, null, null, null, null, "Admin", "Active", null },
                    { 2, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "nguyenvana@gmail.com", "Nguyễn Văn A", null, null, "hashed123", null, null, null, null, null, null, null, "Customer", "Active", null },
                    { 3, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "tranthib@gmail.com", "Trần Thị B", null, null, "hashed123", null, null, null, null, null, null, null, "Customer", "Active", null }
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "CategoryId", "CategoryName", "CreatedAt", "Description", "ImageUrl", "IsActive", "ParentCategoryId", "Slug", "UpdatedAt" },
                values: new object[] { 3, "Áo Thun Nam", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, 1, "ao-thun-nam", null });

            migrationBuilder.InsertData(
                table: "CustomerActivities",
                columns: new[] { "ActivityId", "ActivityType", "Comment", "CreatedAt", "InputJson", "IpAddress", "IsActive", "Keyword", "OrderItemId", "ProductId", "Rating", "ResultJson", "SessionId", "UpdatedAt", "UserId", "VariantId" },
                values: new object[] { 3, "Search", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, "Áo khoác mùa đông", null, null, null, null, null, null, 2, null });

            migrationBuilder.InsertData(
                table: "InteractionLogs",
                columns: new[] { "LogId", "ActionName", "CreatedAt", "EntitiesJson", "IntentName", "IsRead", "LogType", "Message", "NewValueJson", "OldValueJson", "OrderId", "PayloadJson", "ProductId", "QuantityChanged", "ReadAt", "ReferenceId", "ReferenceType", "SenderType", "SessionId", "Status", "Title", "UserId", "VariantId" },
                values: new object[,]
                {
                    { 1, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, "Chatbot", "Cho tôi hỏi size áo thun", null, null, null, null, null, null, null, null, null, "User", null, null, null, 2, null },
                    { 2, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, "Chatbot", "Dạ, size M phù hợp với người từ 50-60kg ạ.", null, null, null, null, null, null, null, null, null, "Bot", null, null, null, 2, null }
                });

            migrationBuilder.InsertData(
                table: "InteractionLogs",
                columns: new[] { "LogId", "ActionName", "CreatedAt", "EntitiesJson", "IntentName", "LogType", "Message", "NewValueJson", "OldValueJson", "OrderId", "PayloadJson", "ProductId", "QuantityChanged", "ReadAt", "ReferenceId", "ReferenceType", "SenderType", "SessionId", "Status", "Title", "UserId", "VariantId" },
                values: new object[] { 3, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "Notification", "Đơn hàng ORD003 của bạn đang được giao", null, null, null, null, null, null, null, null, null, null, null, null, "Đơn hàng đang giao", 3, null });

            migrationBuilder.InsertData(
                table: "Orders",
                columns: new[] { "OrderId", "AddressId", "CancelReason", "CanceledAt", "CompletedAt", "CreatedAt", "CustomerNote", "OrderCode", "OrderStatus", "PaymentStatus", "ReceiverName", "ReceiverPhone", "ShippingAddressText", "ShippingStatus", "StaffNote", "TotalAmount", "UpdatedAt", "UserId" },
                values: new object[,]
                {
                    { 1, null, null, null, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "ORD001", "Completed", "Paid", "Nguyễn Văn A", "0901234567", "123 Lê Lợi, Quận 1, TP.HCM", "Delivered", null, 240000m, null, 2 },
                    { 2, null, null, null, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "ORD002", "Pending", "Unpaid", "Nguyễn Văn A", "0901234567", "456 Nguyễn Huệ, Quận 1, TP.HCM", "NotShipped", null, 300000m, null, 2 },
                    { 3, null, null, null, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "ORD003", "Shipping", "Paid", "Trần Thị B", "0987654321", "789 Hai Bà Trưng, Quận 3, TP.HCM", "Shipping", null, 400000m, null, 3 }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "ProductId", "BasePrice", "Brand", "CategoryId", "CreatedAt", "DeletedAt", "Description", "GenderTarget", "IsActive", "Material", "ProductName", "SalePrice", "Slug", "UpdatedAt" },
                values: new object[,]
                {
                    { 2, 350000m, null, 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, true, null, "Váy Hoa Mùa Hè", 300000m, "vay-hoa-mua-he", null },
                    { 3, 400000m, null, 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, true, null, "Áo Khoác Hoodie Unisex", 400000m, "ao-khoac-hoodie-unisex", null }
                });

            migrationBuilder.InsertData(
                table: "UserAddresses",
                columns: new[] { "AddressId", "CreatedAt", "DeletedAt", "District", "IsDefault", "PostalCode", "Province", "ReceiverName", "ReceiverPhone", "StreetAddress", "UpdatedAt", "UserId", "Ward" },
                values: new object[] { 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Quận 1", true, null, "TP.HCM", "Nguyễn Văn A", "0901234567", "123 Lê Lợi", null, 2, "Phường Bến Nghé" });

            migrationBuilder.InsertData(
                table: "UserAddresses",
                columns: new[] { "AddressId", "CreatedAt", "DeletedAt", "District", "PostalCode", "Province", "ReceiverName", "ReceiverPhone", "StreetAddress", "UpdatedAt", "UserId", "Ward" },
                values: new object[] { 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Quận 1", null, "TP.HCM", "Nguyễn Văn A (Công ty)", "0901234567", "456 Nguyễn Huệ", null, 2, "Phường Bến Nghé" });

            migrationBuilder.InsertData(
                table: "UserAddresses",
                columns: new[] { "AddressId", "CreatedAt", "DeletedAt", "District", "IsDefault", "PostalCode", "Province", "ReceiverName", "ReceiverPhone", "StreetAddress", "UpdatedAt", "UserId", "Ward" },
                values: new object[] { 3, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Quận 3", true, null, "TP.HCM", "Trần Thị B", "0987654321", "789 Hai Bà Trưng", null, 3, "Phường 6" });

            migrationBuilder.InsertData(
                table: "CommerceRecords",
                columns: new[] { "RecordId", "Amount", "Code", "CreatedAt", "DiscountType", "DiscountValue", "EndAt", "MaxDiscountAmount", "MinOrderAmount", "Name", "OrderId", "PayloadJson", "PaymentMethod", "PaymentProvider", "ProductId", "RecordType", "StartAt", "Status", "TransactionCode", "UpdatedAt", "UsageLimit", "UserId", "VariantId" },
                values: new object[,]
                {
                    { 1, 240000m, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, null, null, null, 1, null, null, null, null, "Payment", null, "Success", null, null, null, 2, null },
                    { 2, null, "GIAM10K", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 10000m, null, null, null, null, 2, null, null, null, null, "VoucherUsage", null, "Applied", null, null, null, 2, null },
                    { 3, 400000m, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, null, null, null, 3, null, null, null, null, "Payment", null, "Success", null, null, null, 3, null }
                });

            migrationBuilder.InsertData(
                table: "CustomerActivities",
                columns: new[] { "ActivityId", "ActivityType", "Comment", "CreatedAt", "InputJson", "IpAddress", "IsActive", "Keyword", "OrderItemId", "ProductId", "Rating", "ResultJson", "SessionId", "UpdatedAt", "UserId", "VariantId" },
                values: new object[] { 2, "Wishlist", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, null, null, 2, null, null, null, null, 3, null });

            migrationBuilder.InsertData(
                table: "ProductImages",
                columns: new[] { "ImageId", "AltText", "ColorAnalysisJson", "ContentType", "CreatedAt", "DominantColorHex", "FileName", "FileSize", "ImageUrl", "IsThumbnail", "ProductId", "VariantId" },
                values: new object[,]
                {
                    { 2, null, null, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, "/images/vay-hoa.jpg", true, 2, null },
                    { 3, null, null, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, "/images/hoodie-xam.jpg", true, 3, null }
                });

            migrationBuilder.InsertData(
                table: "ProductVariants",
                columns: new[] { "VariantId", "Barcode", "Color", "ColorHex", "CreatedAt", "IsActive", "OriginalPrice", "ProductId", "Size", "Sku", "StockQuantity", "UpdatedAt", "WeightGram" },
                values: new object[,]
                {
                    { 3, null, "Đỏ", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, null, 2, "S", "VHM-DO-S", 20, null, null },
                    { 4, null, "Xám", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, null, 3, "XL", "AKH-XA-XL", 100, null, null }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "ProductId", "BasePrice", "Brand", "CategoryId", "CreatedAt", "DeletedAt", "Description", "GenderTarget", "IsActive", "Material", "ProductName", "SalePrice", "Slug", "UpdatedAt" },
                values: new object[] { 1, 150000m, null, 3, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, true, null, "Áo Thun Cổ Tròn Basic", 120000m, "ao-thun-co-tron-basic", null });

            migrationBuilder.InsertData(
                table: "CartItems",
                columns: new[] { "CartItemId", "CreatedAt", "IsSelected", "Quantity", "SelectedColor", "SelectedSize", "SessionId", "UnitPrice", "UpdatedAt", "UserId", "VariantId" },
                values: new object[,]
                {
                    { 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 1, null, null, null, 300000m, null, 2, 3 },
                    { 3, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 1, null, null, null, 400000m, null, 3, 4 }
                });

            migrationBuilder.InsertData(
                table: "CustomerActivities",
                columns: new[] { "ActivityId", "ActivityType", "Comment", "CreatedAt", "InputJson", "IpAddress", "IsActive", "Keyword", "OrderItemId", "ProductId", "Rating", "ResultJson", "SessionId", "UpdatedAt", "UserId", "VariantId" },
                values: new object[] { 1, "Review", "Áo mặc mát mẻ, đẹp", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, null, null, 1, 5, null, null, null, 2, null });

            migrationBuilder.InsertData(
                table: "OrderItems",
                columns: new[] { "OrderItemId", "ColorSnapshot", "CreatedAt", "ImageUrlSnapshot", "LineTotal", "OrderId", "ProductId", "ProductNameSnapshot", "Quantity", "ReviewStatus", "SalePrice", "SizeSnapshot", "SkuSnapshot", "UnitPrice", "UpdatedAt", "VariantId" },
                values: new object[,]
                {
                    { 2, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 300000m, 2, 2, "Váy Hoa Mùa Hè", 1, "NotReviewed", 300000m, null, "VHM-DO-S", 350000m, null, 3 },
                    { 3, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 400000m, 3, 3, "Áo Khoác Hoodie Unisex", 1, "NotReviewed", 400000m, null, "AKH-XA-XL", 400000m, null, 4 }
                });

            migrationBuilder.InsertData(
                table: "ProductImages",
                columns: new[] { "ImageId", "AltText", "ColorAnalysisJson", "ContentType", "CreatedAt", "DominantColorHex", "FileName", "FileSize", "ImageUrl", "IsThumbnail", "ProductId", "VariantId" },
                values: new object[] { 1, null, null, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, "/images/ao-thun-trang.jpg", true, 1, null });

            migrationBuilder.InsertData(
                table: "ProductVariants",
                columns: new[] { "VariantId", "Barcode", "Color", "ColorHex", "CreatedAt", "IsActive", "OriginalPrice", "ProductId", "Size", "Sku", "StockQuantity", "UpdatedAt", "WeightGram" },
                values: new object[,]
                {
                    { 1, null, "Trắng", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, null, 1, "M", "ATB-TR-M", 50, null, null },
                    { 2, null, "Đen", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, null, 1, "L", "ATB-DE-L", 30, null, null }
                });

            migrationBuilder.InsertData(
                table: "CartItems",
                columns: new[] { "CartItemId", "CreatedAt", "IsSelected", "Quantity", "SelectedColor", "SelectedSize", "SessionId", "UnitPrice", "UpdatedAt", "UserId", "VariantId" },
                values: new object[] { 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 2, null, null, null, 120000m, null, 2, 1 });

            migrationBuilder.InsertData(
                table: "OrderItems",
                columns: new[] { "OrderItemId", "ColorSnapshot", "CreatedAt", "ImageUrlSnapshot", "LineTotal", "OrderId", "ProductId", "ProductNameSnapshot", "Quantity", "ReviewStatus", "SalePrice", "SizeSnapshot", "SkuSnapshot", "UnitPrice", "UpdatedAt", "VariantId" },
                values: new object[] { 1, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 240000m, 1, 1, "Áo Thun Cổ Tròn Basic", 2, "NotReviewed", 120000m, null, "ATB-TR-M", 150000m, null, 1 });

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_SessionId",
                table: "CartItems",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_UserId",
                table: "CartItems",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_VariantId",
                table: "CartItems",
                column: "VariantId");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_ParentCategoryId",
                table: "Categories",
                column: "ParentCategoryId");

            migrationBuilder.CreateIndex(
                name: "UQ_Categories_Slug",
                table: "Categories",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CommerceRecords_OrderId",
                table: "CommerceRecords",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_CommerceRecords_ProductId",
                table: "CommerceRecords",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_CommerceRecords_Type_Status",
                table: "CommerceRecords",
                columns: new[] { "RecordType", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_CommerceRecords_UserId",
                table: "CommerceRecords",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CommerceRecords_VariantId",
                table: "CommerceRecords",
                column: "VariantId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerActivities_OrderItemId",
                table: "CustomerActivities",
                column: "OrderItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerActivities_ProductId",
                table: "CustomerActivities",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerActivities_User_Type",
                table: "CustomerActivities",
                columns: new[] { "UserId", "ActivityType" });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerActivities_VariantId",
                table: "CustomerActivities",
                column: "VariantId");

            migrationBuilder.CreateIndex(
                name: "IX_InteractionLogs_OrderId",
                table: "InteractionLogs",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_InteractionLogs_ProductId",
                table: "InteractionLogs",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_InteractionLogs_User_Type",
                table: "InteractionLogs",
                columns: new[] { "UserId", "LogType" });

            migrationBuilder.CreateIndex(
                name: "IX_InteractionLogs_VariantId",
                table: "InteractionLogs",
                column: "VariantId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_OrderId",
                table: "OrderItems",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_ProductId",
                table: "OrderItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_VariantId",
                table: "OrderItems",
                column: "VariantId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_AddressId",
                table: "Orders",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_OrderStatus",
                table: "Orders",
                column: "OrderStatus");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_UserId",
                table: "Orders",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "UQ_Orders_OrderCode",
                table: "Orders",
                column: "OrderCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductImages_ProductId",
                table: "ProductImages",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductImages_VariantId",
                table: "ProductImages",
                column: "VariantId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_CategoryId",
                table: "Products",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_SearchFlags",
                table: "Products",
                columns: new[] { "IsActive", "IsFeatured", "IsBestSeller", "IsNewArrival" });

            migrationBuilder.CreateIndex(
                name: "UQ_Products_Slug",
                table: "Products",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductVariants_ProductId",
                table: "ProductVariants",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "UQ_ProductVariants_Sku",
                table: "ProductVariants",
                column: "Sku",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserAddresses_UserId",
                table: "UserAddresses",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "UQ_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CartItems");

            migrationBuilder.DropTable(
                name: "CommerceRecords");

            migrationBuilder.DropTable(
                name: "CustomerActivities");

            migrationBuilder.DropTable(
                name: "InteractionLogs");

            migrationBuilder.DropTable(
                name: "ProductImages");

            migrationBuilder.DropTable(
                name: "OrderItems");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "ProductVariants");

            migrationBuilder.DropTable(
                name: "UserAddresses");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
