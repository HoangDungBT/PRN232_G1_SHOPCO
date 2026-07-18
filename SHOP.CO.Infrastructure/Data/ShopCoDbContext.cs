using Microsoft.EntityFrameworkCore;
using SHOP.CO.Domain.Entities;

namespace SHOP.CO.Infrastructure.Persistence
{
    public class ShopCoDbContext : DbContext
    {
        public ShopCoDbContext(DbContextOptions<ShopCoDbContext> options) : base(options)
        {
        }

        // Khai báo 12 bảng (DbSet)
        public DbSet<User> Users { get; set; }
        public DbSet<UserAddress> UserAddresses { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductVariant> ProductVariants { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<CustomerActivity> CustomerActivities { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<CommerceRecord> CommerceRecords { get; set; }
        public DbSet<InteractionLog> InteractionLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ==========================================
            // FLUENT API CẤU HÌNH NGHIÊM NGẶT CHO 12 BẢNG
            // ==========================================

            // 1. Users
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users", t => {
                    t.HasCheckConstraint("CK_Users_Role", "[Role] IN (N'Customer', N'Staff', N'Admin')");
                    t.HasCheckConstraint("CK_Users_Status", "[Status] IN (N'Active', N'Locked', N'Deleted')");
                });
                entity.HasKey(e => e.UserId);
                entity.HasIndex(e => e.Email).IsUnique().HasDatabaseName("UQ_Users_Email");

                entity.Property(e => e.FullName).HasMaxLength(150).IsRequired();
                entity.Property(e => e.Email).HasMaxLength(255).IsRequired();
                entity.Property(e => e.PasswordHash).HasColumnType("nvarchar(max)").IsRequired();
                entity.Property(e => e.Phone).HasMaxLength(20);
                entity.Property(e => e.AvatarUrl).HasMaxLength(500);
                entity.Property(e => e.Role).HasMaxLength(30).IsRequired().HasDefaultValue("Customer");
                entity.Property(e => e.Status).HasMaxLength(30).IsRequired().HasDefaultValue("Active");
                entity.Property(e => e.Gender).HasMaxLength(20);
                entity.Property(e => e.DateOfBirth).HasColumnType("date");
                entity.Property(e => e.PreferredSize).HasMaxLength(20);
                entity.Property(e => e.PreferredStyle).HasMaxLength(100);
                entity.Property(e => e.ResetPasswordToken).HasMaxLength(255);
                entity.Property(e => e.ResetPasswordExpiresAt).HasColumnType("datetime2");
                entity.Property(e => e.RefreshToken).HasMaxLength(500);
                entity.Property(e => e.RefreshTokenExpiresAt).HasColumnType("datetime2");
                entity.Property(e => e.LastLoginAt).HasColumnType("datetime2");
                entity.Property(e => e.CreatedAt).HasColumnType("datetime2").HasDefaultValueSql("SYSDATETIME()").IsRequired();
                entity.Property(e => e.UpdatedAt).HasColumnType("datetime2");
            });

            // 2. UserAddresses
            modelBuilder.Entity<UserAddress>(entity =>
            {
                entity.ToTable("UserAddresses");
                entity.HasKey(e => e.AddressId);
                entity.HasIndex(e => e.UserId).HasDatabaseName("IX_UserAddresses_UserId");

                entity.Property(e => e.ReceiverName).HasMaxLength(150).IsRequired();
                entity.Property(e => e.ReceiverPhone).HasMaxLength(20).IsRequired();
                entity.Property(e => e.Province).HasMaxLength(100).IsRequired();
                entity.Property(e => e.District).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Ward).HasMaxLength(100).IsRequired();
                entity.Property(e => e.StreetAddress).HasMaxLength(255).IsRequired();
                entity.Property(e => e.PostalCode).HasMaxLength(20);
                entity.Property(e => e.IsDefault).IsRequired().HasDefaultValue(false);
                entity.Property(e => e.CreatedAt).HasColumnType("datetime2").HasDefaultValueSql("SYSDATETIME()").IsRequired();
                entity.Property(e => e.UpdatedAt).HasColumnType("datetime2");
                entity.Property(e => e.DeletedAt).HasColumnType("datetime2");

                entity.HasOne(a => a.User).WithMany(u => u.UserAddresses).HasForeignKey(a => a.UserId).OnDelete(DeleteBehavior.Cascade);
            });

            // 3. Categories
            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("Categories");
                entity.HasKey(e => e.CategoryId);
                entity.HasIndex(e => e.Slug).IsUnique().HasDatabaseName("UQ_Categories_Slug");

                entity.Property(e => e.CategoryName).HasMaxLength(150).IsRequired();
                entity.Property(e => e.Slug).HasMaxLength(200).IsRequired();
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.ImageUrl).HasMaxLength(500);
                entity.Property(e => e.SortOrder).IsRequired().HasDefaultValue(0);
                entity.Property(e => e.IsActive).IsRequired().HasDefaultValue(true);
                entity.Property(e => e.CreatedAt).HasColumnType("datetime2").HasDefaultValueSql("SYSDATETIME()").IsRequired();
                entity.Property(e => e.UpdatedAt).HasColumnType("datetime2");

                entity.HasOne(c => c.ParentCategory).WithMany(c => c.SubCategories).HasForeignKey(c => c.ParentCategoryId).OnDelete(DeleteBehavior.Restrict);
            });

            // 4. Products
            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("Products", t => {
                    t.HasCheckConstraint("CK_Products_BasePrice", "[BasePrice] >= 0");
                    t.HasCheckConstraint("CK_Products_SalePrice", "[SalePrice] IS NULL OR [SalePrice] >= 0");
                    t.HasCheckConstraint("CK_Products_Rating", "[AverageRating] >= 0 AND [AverageRating] <= 5");
                });
                entity.HasKey(e => e.ProductId);
                entity.HasIndex(e => e.Slug).IsUnique().HasDatabaseName("UQ_Products_Slug");
                entity.HasIndex(e => e.CategoryId).HasDatabaseName("IX_Products_CategoryId");
                entity.HasIndex(e => new { e.IsActive, e.IsFeatured, e.IsBestSeller, e.IsNewArrival }).HasDatabaseName("IX_Products_SearchFlags");

                entity.Property(e => e.ProductName).HasMaxLength(200).IsRequired();
                entity.Property(e => e.Slug).HasMaxLength(250).IsRequired();
                entity.Property(e => e.Brand).HasMaxLength(100);
                entity.Property(e => e.Description).HasColumnType("nvarchar(max)");
                entity.Property(e => e.Material).HasMaxLength(100);
                entity.Property(e => e.GenderTarget).HasMaxLength(30);
                entity.Property(e => e.BasePrice).HasColumnType("decimal(18,2)").IsRequired();
                entity.Property(e => e.SalePrice).HasColumnType("decimal(18,2)");
                entity.Property(e => e.AverageRating).HasColumnType("decimal(3,2)").IsRequired().HasDefaultValue(0m); 
                entity.Property(e => e.ReviewCount).IsRequired().HasDefaultValue(0);
                entity.Property(e => e.ViewCount).IsRequired().HasDefaultValue(0);
                entity.Property(e => e.IsFeatured).IsRequired().HasDefaultValue(false);
                entity.Property(e => e.IsBestSeller).IsRequired().HasDefaultValue(false);
                entity.Property(e => e.IsNewArrival).IsRequired().HasDefaultValue(false);
                entity.Property(e => e.IsActive).IsRequired().HasDefaultValue(true);
                entity.Property(e => e.CreatedAt).HasColumnType("datetime2").HasDefaultValueSql("SYSDATETIME()").IsRequired();
                entity.Property(e => e.UpdatedAt).HasColumnType("datetime2");
                entity.Property(e => e.DeletedAt).HasColumnType("datetime2");

                entity.HasOne(p => p.Category).WithMany(c => c.Products).HasForeignKey(p => p.CategoryId).OnDelete(DeleteBehavior.Restrict);

            });

            // 5. ProductVariants
            modelBuilder.Entity<ProductVariant>(entity =>
            {
                entity.ToTable("ProductVariants", t => {
                    t.HasCheckConstraint("CK_ProductVariants_Stock", "[StockQuantity] >= 0");
                    t.HasCheckConstraint("CK_ProductVariants_LowStock", "[LowStockThreshold] >= 0");
                });
                entity.HasKey(e => e.VariantId);
                entity.HasIndex(e => e.Sku).IsUnique().HasDatabaseName("UQ_ProductVariants_Sku");
                entity.HasIndex(e => e.ProductId).HasDatabaseName("IX_ProductVariants_ProductId");

                entity.Property(e => e.Sku).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Size).HasMaxLength(20);
                entity.Property(e => e.Color).HasMaxLength(50);
                entity.Property(e => e.ColorHex).HasMaxLength(20);
                entity.Property(e => e.ExtraPrice).HasColumnType("decimal(18,2)").IsRequired().HasDefaultValue(0m); 
                entity.Property(e => e.OriginalPrice).HasColumnType("decimal(18,2)");
                entity.Property(e => e.StockQuantity).IsRequired().HasDefaultValue(0);
                entity.Property(e => e.LowStockThreshold).IsRequired().HasDefaultValue(5);
                entity.Property(e => e.Barcode).HasMaxLength(100);
                entity.Property(e => e.IsActive).IsRequired().HasDefaultValue(true);
                entity.Property(e => e.CreatedAt).HasColumnType("datetime2").HasDefaultValueSql("SYSDATETIME()").IsRequired();
                entity.Property(e => e.UpdatedAt).HasColumnType("datetime2");

                entity.HasOne(v => v.Product).WithMany(p => p.ProductVariants).HasForeignKey(v => v.ProductId).OnDelete(DeleteBehavior.Cascade);
            });

            // 6. ProductImages
            modelBuilder.Entity<ProductImage>(entity =>
            {
                entity.ToTable("ProductImages", t => {
                    t.HasCheckConstraint("CK_ProductImages_ColorAnalysisJson", "[ColorAnalysisJson] IS NULL OR ISJSON([ColorAnalysisJson]) = 1");
                });
                entity.HasKey(e => e.ImageId);
                entity.HasIndex(e => e.ProductId).HasDatabaseName("IX_ProductImages_ProductId");

                entity.Property(e => e.ImageUrl).HasMaxLength(500).IsRequired();
                entity.Property(e => e.AltText).HasMaxLength(255);
                entity.Property(e => e.IsThumbnail).IsRequired().HasDefaultValue(false);
                entity.Property(e => e.SortOrder).IsRequired().HasDefaultValue(0);
                entity.Property(e => e.FileName).HasMaxLength(255);
                entity.Property(e => e.ContentType).HasMaxLength(100);
                entity.Property(e => e.DominantColorHex).HasMaxLength(20);
                entity.Property(e => e.ColorAnalysisJson).HasColumnType("nvarchar(max)");
                entity.Property(e => e.CreatedAt).HasColumnType("datetime2").HasDefaultValueSql("SYSDATETIME()").IsRequired();

                entity.HasOne(i => i.Product).WithMany(p => p.ProductImages).HasForeignKey(i => i.ProductId).OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(i => i.ProductVariant).WithMany().HasForeignKey(i => i.VariantId).OnDelete(DeleteBehavior.Restrict);
            });

            // 7. CartItems
            modelBuilder.Entity<CartItem>(entity =>
            {
                entity.ToTable("CartItems", t => {
                    t.HasCheckConstraint("CK_CartItems_Quantity", "[Quantity] > 0");
                    t.HasCheckConstraint("CK_CartItems_UserOrSession", "[UserId] IS NOT NULL OR [SessionId] IS NOT NULL");
                });
                entity.HasKey(e => e.CartItemId);
                entity.HasIndex(e => e.UserId).HasDatabaseName("IX_CartItems_UserId");
                entity.HasIndex(e => e.SessionId).HasDatabaseName("IX_CartItems_SessionId");

                entity.Property(e => e.SessionId).HasMaxLength(100);
                entity.Property(e => e.Quantity).IsRequired();
                entity.Property(e => e.UnitPrice).HasColumnType("decimal(18,2)").IsRequired();
                entity.Property(e => e.SelectedSize).HasMaxLength(20);
                entity.Property(e => e.SelectedColor).HasMaxLength(50);
                entity.Property(e => e.IsSelected).IsRequired().HasDefaultValue(true);
                entity.Property(e => e.CreatedAt).HasColumnType("datetime2").HasDefaultValueSql("SYSDATETIME()").IsRequired();
                entity.Property(e => e.UpdatedAt).HasColumnType("datetime2");

                entity.HasOne(c => c.ProductVariant).WithMany(v => v.CartItems).HasForeignKey(c => c.VariantId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(c => c.User).WithMany(u => u.CartItems).HasForeignKey(c => c.UserId).OnDelete(DeleteBehavior.Cascade); // Bổ sung FK_CartItems_Users
            });

            // 8. Orders
            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("Orders", t => {
                    t.HasCheckConstraint("CK_Orders_OrderStatus", "[OrderStatus] IN (N'Pending', N'Confirmed', N'Shipping', N'Completed', N'Canceled')");
                    t.HasCheckConstraint("CK_Orders_PaymentStatus", "[PaymentStatus] IN (N'Unpaid', N'Paid', N'Failed', N'Refunded')");
                    t.HasCheckConstraint("CK_Orders_TotalAmount", "[TotalAmount] >= 0");
                });
                entity.HasKey(e => e.OrderId);
                entity.HasIndex(e => e.OrderCode).IsUnique().HasDatabaseName("UQ_Orders_OrderCode");
                entity.HasIndex(e => e.UserId).HasDatabaseName("IX_Orders_UserId");
                entity.HasIndex(e => e.OrderStatus).HasDatabaseName("IX_Orders_OrderStatus");

                entity.Property(e => e.OrderCode).HasMaxLength(50).IsRequired();
                entity.Property(e => e.ReceiverName).HasMaxLength(150).IsRequired();
                entity.Property(e => e.ReceiverPhone).HasMaxLength(20).IsRequired();
                entity.Property(e => e.ShippingAddressText).HasMaxLength(500).IsRequired();
                entity.Property(e => e.OrderStatus).HasMaxLength(50).IsRequired().HasDefaultValue("Pending");
                entity.Property(e => e.PaymentStatus).HasMaxLength(50).IsRequired().HasDefaultValue("Unpaid");
                entity.Property(e => e.ShippingStatus).HasMaxLength(50).IsRequired().HasDefaultValue("NotShipped");
                entity.Property(e => e.SubtotalAmount).HasColumnType("decimal(18,2)").IsRequired().HasDefaultValue(0m); 
                entity.Property(e => e.DiscountAmount).HasColumnType("decimal(18,2)").IsRequired().HasDefaultValue(0m); 
                entity.Property(e => e.ShippingFee).HasColumnType("decimal(18,2)").IsRequired().HasDefaultValue(0m); 
                entity.Property(e => e.TotalAmount).HasColumnType("decimal(18,2)").IsRequired().HasDefaultValue(0m); 
                entity.Property(e => e.CustomerNote).HasMaxLength(500);
                entity.Property(e => e.StaffNote).HasMaxLength(500);
                entity.Property(e => e.CancelReason).HasMaxLength(500);
                entity.Property(e => e.CreatedAt).HasColumnType("datetime2").HasDefaultValueSql("SYSDATETIME()").IsRequired();
                entity.Property(e => e.UpdatedAt).HasColumnType("datetime2");
                entity.Property(e => e.CompletedAt).HasColumnType("datetime2");
                entity.Property(e => e.CanceledAt).HasColumnType("datetime2");

                entity.HasOne(o => o.User).WithMany(u => u.Orders).HasForeignKey(o => o.UserId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(o => o.UserAddress).WithMany().HasForeignKey(o => o.AddressId).OnDelete(DeleteBehavior.SetNull);
            });

            // 9. OrderItems
            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.ToTable("OrderItems", t => {
                    t.HasCheckConstraint("CK_OrderItems_Quantity", "[Quantity] > 0");
                    t.HasCheckConstraint("CK_OrderItems_LineTotal", "[LineTotal] >= 0");
                    t.HasCheckConstraint("CK_OrderItems_ReviewStatus", "[ReviewStatus] IN (N'NotReviewed', N'Reviewed')");
                });
                entity.HasKey(e => e.OrderItemId);
                entity.HasIndex(e => e.OrderId).HasDatabaseName("IX_OrderItems_OrderId");

                entity.Property(e => e.ProductNameSnapshot).HasMaxLength(200).IsRequired();
                entity.Property(e => e.SkuSnapshot).HasMaxLength(100).IsRequired();
                entity.Property(e => e.SizeSnapshot).HasMaxLength(20);
                entity.Property(e => e.ColorSnapshot).HasMaxLength(50);
                entity.Property(e => e.ImageUrlSnapshot).HasMaxLength(500);
                entity.Property(e => e.UnitPrice).HasColumnType("decimal(18,2)").IsRequired();
                entity.Property(e => e.SalePrice).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Quantity).IsRequired();
                entity.Property(e => e.DiscountAmount).HasColumnType("decimal(18,2)").IsRequired().HasDefaultValue(0m); 
                entity.Property(e => e.LineTotal).HasColumnType("decimal(18,2)").IsRequired();
                entity.Property(e => e.ReviewStatus).HasMaxLength(30).IsRequired().HasDefaultValue("NotReviewed");
                entity.Property(e => e.CreatedAt).HasColumnType("datetime2").HasDefaultValueSql("SYSDATETIME()").IsRequired();
                entity.Property(e => e.UpdatedAt).HasColumnType("datetime2");

                entity.HasOne(oi => oi.Order).WithMany(o => o.OrderItems).HasForeignKey(oi => oi.OrderId).OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(oi => oi.Product).WithMany().HasForeignKey(oi => oi.ProductId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(oi => oi.ProductVariant).WithMany().HasForeignKey(oi => oi.VariantId).OnDelete(DeleteBehavior.Restrict);
            });

            // 10. CustomerActivities
            modelBuilder.Entity<CustomerActivity>(entity =>
            {
                entity.ToTable("CustomerActivities", t => {
                    t.HasCheckConstraint("CK_CustomerActivities_Type", "[ActivityType] IN (N'Wishlist', N'Review', N'RecentlyViewed', N'Search', N'AiSize', N'AiOutfit', N'AiColorSearch')");
                    t.HasCheckConstraint("CK_CustomerActivities_Rating", "[Rating] IS NULL OR ([Rating] >= 1 AND [Rating] <= 5)");
                    t.HasCheckConstraint("CK_CustomerActivities_InputJson", "[InputJson] IS NULL OR ISJSON([InputJson]) = 1");
                    t.HasCheckConstraint("CK_CustomerActivities_ResultJson", "[ResultJson] IS NULL OR ISJSON([ResultJson]) = 1");
                });
                entity.HasKey(e => e.ActivityId);
                entity.HasIndex(e => new { e.UserId, e.ActivityType }).HasDatabaseName("IX_CustomerActivities_User_Type");

                entity.Property(e => e.ActivityType).HasMaxLength(50).IsRequired();
                entity.Property(e => e.Keyword).HasMaxLength(255);
                entity.Property(e => e.SessionId).HasMaxLength(100);
                entity.Property(e => e.IpAddress).HasMaxLength(50);
                entity.Property(e => e.InputJson).HasColumnType("nvarchar(max)");
                entity.Property(e => e.ResultJson).HasColumnType("nvarchar(max)");
                entity.Property(e => e.Comment).HasColumnType("nvarchar(max)");
                entity.Property(e => e.IsActive).IsRequired().HasDefaultValue(true);
                entity.Property(e => e.CreatedAt).HasColumnType("datetime2").HasDefaultValueSql("SYSDATETIME()").IsRequired();
                entity.Property(e => e.UpdatedAt).HasColumnType("datetime2");

                entity.HasOne(a => a.User).WithMany(u => u.CustomerActivities).HasForeignKey(a => a.UserId).OnDelete(DeleteBehavior.SetNull);
                entity.HasOne(a => a.Product).WithMany().HasForeignKey(a => a.ProductId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(a => a.ProductVariant).WithMany().HasForeignKey(a => a.VariantId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(a => a.OrderItem).WithMany().HasForeignKey(a => a.OrderItemId).OnDelete(DeleteBehavior.Restrict);
            });

            // 11. CommerceRecords
            modelBuilder.Entity<CommerceRecord>(entity =>
            {
                entity.ToTable("CommerceRecords", t => {
                    t.HasCheckConstraint("CK_CommerceRecords_Type", "[RecordType] IN (N'Payment', N'Voucher', N'VoucherUsage', N'FlashSale', N'HotDeal')");
                    t.HasCheckConstraint("CK_CommerceRecords_PaymentMethod", "[PaymentMethod] IS NULL OR [PaymentMethod] IN (N'COD', N'Online')");
                    t.HasCheckConstraint("CK_CommerceRecords_DiscountType", "[DiscountType] IS NULL OR [DiscountType] IN (N'Percent', N'Fixed')");
                    t.HasCheckConstraint("CK_CommerceRecords_PayloadJson", "[PayloadJson] IS NULL OR ISJSON([PayloadJson]) = 1");
                });
                entity.HasKey(e => e.RecordId);
                entity.HasIndex(e => new { e.RecordType, e.Status }).HasDatabaseName("IX_CommerceRecords_Type_Status");

                entity.Property(e => e.RecordType).HasMaxLength(50).IsRequired();
                entity.Property(e => e.Code).HasMaxLength(100);
                entity.Property(e => e.Name).HasMaxLength(200);
                entity.Property(e => e.PaymentMethod).HasMaxLength(50);
                entity.Property(e => e.PaymentProvider).HasMaxLength(100);
                entity.Property(e => e.TransactionCode).HasMaxLength(100);
                entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
                entity.Property(e => e.DiscountType).HasMaxLength(30);
                entity.Property(e => e.DiscountValue).HasColumnType("decimal(18,2)");
                entity.Property(e => e.MaxDiscountAmount).HasColumnType("decimal(18,2)");
                entity.Property(e => e.MinOrderAmount).HasColumnType("decimal(18,2)");
                entity.Property(e => e.UsedCount).IsRequired().HasDefaultValue(0);
                entity.Property(e => e.Status).HasMaxLength(50).IsRequired().HasDefaultValue("Active");
                entity.Property(e => e.PayloadJson).HasColumnType("nvarchar(max)");
                entity.Property(e => e.StartAt).HasColumnType("datetime2");
                entity.Property(e => e.EndAt).HasColumnType("datetime2");
                entity.Property(e => e.CreatedAt).HasColumnType("datetime2").HasDefaultValueSql("SYSDATETIME()").IsRequired();
                entity.Property(e => e.UpdatedAt).HasColumnType("datetime2");

                entity.HasOne(c => c.Order).WithMany(o => o.CommerceRecords).HasForeignKey(c => c.OrderId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(c => c.User).WithMany(u => u.CommerceRecords).HasForeignKey(c => c.UserId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(c => c.Product).WithMany().HasForeignKey(c => c.ProductId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(c => c.ProductVariant).WithMany().HasForeignKey(c => c.VariantId).OnDelete(DeleteBehavior.Restrict);
            });

            // 12. InteractionLogs
            modelBuilder.Entity<InteractionLog>(entity =>
            {
                entity.ToTable("InteractionLogs", t => {
                    t.HasCheckConstraint("CK_InteractionLogs_Type", "[LogType] IN (N'Chatbot', N'Notification', N'Email', N'Audit', N'StockMovement', N'StockAlert', N'ReportExport')");
                    t.HasCheckConstraint("CK_InteractionLogs_SenderType", "[SenderType] IS NULL OR [SenderType] IN (N'User', N'Bot', N'System', N'Admin')");
                    t.HasCheckConstraint("CK_InteractionLogs_EntitiesJson", "[EntitiesJson] IS NULL OR ISJSON([EntitiesJson]) = 1");
                    t.HasCheckConstraint("CK_InteractionLogs_OldValueJson", "[OldValueJson] IS NULL OR ISJSON([OldValueJson]) = 1");
                    t.HasCheckConstraint("CK_InteractionLogs_NewValueJson", "[NewValueJson] IS NULL OR ISJSON([NewValueJson]) = 1");
                    t.HasCheckConstraint("CK_InteractionLogs_PayloadJson", "[PayloadJson] IS NULL OR ISJSON([PayloadJson]) = 1");
                });
                entity.HasKey(e => e.LogId);
                entity.HasIndex(e => new { e.UserId, e.LogType }).HasDatabaseName("IX_InteractionLogs_User_Type");

                entity.Property(e => e.SessionId).HasMaxLength(100);
                entity.Property(e => e.LogType).HasMaxLength(50).IsRequired();
                entity.Property(e => e.Title).HasMaxLength(200);
                entity.Property(e => e.SenderType).HasMaxLength(30);
                entity.Property(e => e.IntentName).HasMaxLength(100);
                entity.Property(e => e.ActionName).HasMaxLength(150);
                entity.Property(e => e.ReferenceType).HasMaxLength(50);
                entity.Property(e => e.Status).HasMaxLength(50);
                entity.Property(e => e.IsRead).IsRequired().HasDefaultValue(false);
                entity.Property(e => e.Message).HasColumnType("nvarchar(max)");
                entity.Property(e => e.EntitiesJson).HasColumnType("nvarchar(max)");
                entity.Property(e => e.OldValueJson).HasColumnType("nvarchar(max)");
                entity.Property(e => e.NewValueJson).HasColumnType("nvarchar(max)");
                entity.Property(e => e.PayloadJson).HasColumnType("nvarchar(max)");
                entity.Property(e => e.ReadAt).HasColumnType("datetime2");
                entity.Property(e => e.CreatedAt).HasColumnType("datetime2").HasDefaultValueSql("SYSDATETIME()").IsRequired();

                entity.HasOne(l => l.User).WithMany(u => u.InteractionLogs).HasForeignKey(l => l.UserId).OnDelete(DeleteBehavior.SetNull);
                entity.HasOne(l => l.Order).WithMany().HasForeignKey(l => l.OrderId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(l => l.Product).WithMany().HasForeignKey(l => l.ProductId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(l => l.ProductVariant).WithMany().HasForeignKey(l => l.VariantId).OnDelete(DeleteBehavior.Restrict);
            });

            // Mở Seed data nếu cần
            modelBuilder.Seed();
        }
    }
}