using AutoMapper;

namespace SHOP.CO.Application.DTOs
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // voucher mapping
            CreateMap<CommerceRecord, VoucherDto>();

            // ==========================================
            // 1. CATEGORY MAPPINGS
            // ==========================================
            CreateMap<Category, CategoryDto>()
                .ForMember(dest => dest.ParentCategoryName, opt => opt.MapFrom(src => src.ParentCategory != null ? src.ParentCategory.CategoryName : null));

            // ==========================================
            // 2. PRODUCT MAPPINGS
            // ==========================================
            CreateMap<Product, ProductDto>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.CategoryName : "Không có"))
                .ForMember(dest => dest.ThumbnailUrl, opt => opt.MapFrom(src =>
                    src.ProductImages.FirstOrDefault(i => i.IsThumbnail) != null
                    ? src.ProductImages.FirstOrDefault(i => i.IsThumbnail)!.ImageUrl
                    : src.ProductImages.FirstOrDefault() != null ? src.ProductImages.FirstOrDefault()!.ImageUrl : null))
                .ForMember(dest => dest.HasLowStock, opt => opt.MapFrom(src => src.ProductVariants.Any(v => v.StockQuantity <= v.LowStockThreshold)))
                .ForMember(dest => dest.Variants, opt => opt.MapFrom(src => src.ProductVariants))
                .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.ProductImages));

            // Map cho Chi tiết Admin (Kèm List con)
            CreateMap<Product, ProductDetailAdminDto>()
                .ForMember(dest => dest.Variants, opt => opt.MapFrom(src => src.ProductVariants))
                .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.ProductImages));

            CreateMap<ProductVariant, CreateVariantDto>(); // Entity -> Dto
            CreateMap<ProductVariant, ProductVariantDto>(); // Entity -> Dto
            CreateMap<ProductImage, ProductImageDto>();

            // ==========================================
            // 3. USER MAPPINGS
            // ==========================================
            CreateMap<User, UserDto>()
                .ForMember(dest => dest.CreateAt, opt => opt.MapFrom(src => src.CreatedAt)); // Xử lý sai lệch tên (CreatedAt -> CreateAt)

            // ==========================================
            // 4. ORDER MAPPINGS
            // ==========================================
            CreateMap<Order, OrderDto>();

            CreateMap<Order, OrderDetailAdminDto>()
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.OrderItems));

            CreateMap<OrderItem, OrderItemAdminDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.ProductNameSnapshot))
                .ForMember(dest => dest.Sku, opt => opt.MapFrom(src => src.SkuSnapshot))
                .ForMember(dest => dest.Size, opt => opt.MapFrom(src => src.SizeSnapshot))
                .ForMember(dest => dest.Color, opt => opt.MapFrom(src => src.ColorSnapshot))
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.ImageUrlSnapshot))
                .ForMember(dest => dest.SalePrice, opt => opt.MapFrom(src => src.SalePrice ?? src.UnitPrice));

            // ==========================================
            // 5. LOGS MAPPINGS
            // ==========================================
            CreateMap<InteractionLog, InteractionLogDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.FullName)); 


        }
    }
}