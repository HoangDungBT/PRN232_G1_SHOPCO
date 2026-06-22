using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SHOP.CO.Application.Services
{
    public interface IProductService
    {
        Task<PagedResult<ProductDto>> GetProductsAsync(string? searchTerm, int pageNumber, int pageSize);
        IQueryable<ProductDto> GetProductsQuery();
        Task<ProductDto?> GetProductByIdAsync(int id);
        Task<List<ProductDto>> GetRelatedProductsAsync(int productId, int limit);
        Task<List<ReviewDto>> GetReviewsByProductIdAsync(int productId);
        Task AddReviewAsync(int productId, int userId, CreateReviewRequest request);
        Task<List<CategoryDto>> GetActiveCategoriesAsync();
        Task<bool> ToggleWishlistAsync(int productId, int userId);
        Task<List<ProductDto>> GetWishlistAsync(int userId);
    }
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repository;

        public ProductService(IProductRepository repository)
        {
            _repository = repository;
        }



        public async Task<PagedResult<ProductDto>> GetProductsAsync(string? searchTerm, int pageNumber, int pageSize)
        {
            // Logic mặc định chống lỗi
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 10; // Chặn request lấy quá nhiều data


            var result = await _repository.GetPagedProductAsync(searchTerm, pageNumber, pageSize);

            // chuyển entity => dto
            var dtos = result.Items.Select(p => new ProductDto
            {
                ProductId = p.ProductId,
                CategoryId = p.CategoryId,
                ProductName = p.ProductName,
                Slug = p.Slug,
                BasePrice = p.BasePrice,
                SalePrice = p.SalePrice,
                CategoryName = p.Category?.CategoryName,
                AverageRating = p.AverageRating,
                ReviewCount = p.ReviewCount,
                IsFeatured = p.IsFeatured,
                IsBestSeller = p.IsBestSeller,
                IsNewArrival = p.IsNewArrival,
                IsActive = p.IsActive,
                ThumbnailUrl = p.ProductImages.FirstOrDefault(img => img.IsThumbnail) != null 
                    ? p.ProductImages.FirstOrDefault(img => img.IsThumbnail)!.ImageUrl 
                    : p.ProductImages.OrderBy(img => img.SortOrder).Select(img => img.ImageUrl).FirstOrDefault()
            }).ToList();

            //trả kq
            return new PagedResult<ProductDto>
            { 
                Items = dtos,
                TotalCount = result.TotalCount, // SỬA LỖI PAGING: Lấy tổng số lượng từ result thay vì dtos.Count
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public IQueryable<ProductDto> GetProductsQuery()
        {
            var query = _repository.GetProductsQuery();
            
            return query.Select(p => new ProductDto
            {
                ProductId = p.ProductId,
                CategoryId = p.CategoryId,
                ProductName = p.ProductName,
                Slug = p.Slug,
                Brand = p.Brand,
                Description = p.Description,
                Material = p.Material,
                GenderTarget = p.GenderTarget,
                BasePrice = p.BasePrice,
                SalePrice = p.SalePrice,
                AverageRating = p.AverageRating,
                ReviewCount = p.ReviewCount,
                ViewCount = p.ViewCount,
                IsFeatured = p.IsFeatured,
                IsBestSeller = p.IsBestSeller,
                IsNewArrival = p.IsNewArrival,
                IsActive = p.IsActive,
                CategoryName = p.Category != null ? p.Category.CategoryName : null,
                ThumbnailUrl = p.ProductImages.FirstOrDefault(img => img.IsThumbnail) != null 
                    ? p.ProductImages.FirstOrDefault(img => img.IsThumbnail)!.ImageUrl 
                    : p.ProductImages.OrderBy(img => img.SortOrder).Select(img => img.ImageUrl).FirstOrDefault(),
                Variants = p.ProductVariants.Select(v => new ProductVariantDto
                {
                    VariantId = v.VariantId,
                    ProductId = v.ProductId,
                    Sku = v.Sku,
                    Size = v.Size,
                    Color = v.Color,
                    ColorHex = v.ColorHex,
                    ExtraPrice = v.ExtraPrice,
                    OriginalPrice = v.OriginalPrice,
                    StockQuantity = v.StockQuantity,
                    LowStockThreshold = v.LowStockThreshold,
                    Barcode = v.Barcode,
                    WeightGram = v.WeightGram
                }).ToList(),
                Images = p.ProductImages.Select(img => new ProductImageDto
                {
                    ImageId = img.ImageId,
                    ProductId = img.ProductId,
                    VariantId = img.VariantId,
                    ImageUrl = img.ImageUrl,
                    AltText = img.AltText,
                    IsThumbnail = img.IsThumbnail,
                    SortOrder = img.SortOrder
                }).ToList()
            });
        }

        public async Task<ProductDto?> GetProductByIdAsync(int id)
        {
            var product = await _repository.GetProductByIdAsync(id);
            if (product == null) return null;
            return MapToDto(product);
        }

        public async Task<List<ProductDto>> GetRelatedProductsAsync(int productId, int limit)
        {
            var product = await _repository.GetProductByIdAsync(productId);
            if (product == null) return new List<ProductDto>();

            var related = await _repository.GetRelatedProductsAsync(product.CategoryId, productId, limit);
            return related.Select(MapToDto).ToList();
        }

        private ProductDto MapToDto(SHOP.CO.Domain.Entities.Product p)
        {
            return new ProductDto
            {
                ProductId = p.ProductId,
                CategoryId = p.CategoryId,
                ProductName = p.ProductName,
                Slug = p.Slug,
                Brand = p.Brand,
                Description = p.Description,
                Material = p.Material,
                GenderTarget = p.GenderTarget,
                BasePrice = p.BasePrice,
                SalePrice = p.SalePrice,
                AverageRating = p.AverageRating,
                ReviewCount = p.ReviewCount,
                ViewCount = p.ViewCount,
                IsFeatured = p.IsFeatured,
                IsBestSeller = p.IsBestSeller,
                IsNewArrival = p.IsNewArrival,
                IsActive = p.IsActive,
                CategoryName = p.Category != null ? p.Category.CategoryName : null,
                ThumbnailUrl = p.ProductImages.FirstOrDefault(img => img.IsThumbnail) != null 
                    ? p.ProductImages.FirstOrDefault(img => img.IsThumbnail)!.ImageUrl 
                    : p.ProductImages.OrderBy(img => img.SortOrder).Select(img => img.ImageUrl).FirstOrDefault(),
                Variants = p.ProductVariants != null ? p.ProductVariants.Select(v => new ProductVariantDto
                {
                    VariantId = v.VariantId,
                    ProductId = v.ProductId,
                    Sku = v.Sku,
                    Size = v.Size,
                    Color = v.Color,
                    ColorHex = v.ColorHex,
                    ExtraPrice = v.ExtraPrice,
                    OriginalPrice = v.OriginalPrice,
                    StockQuantity = v.StockQuantity,
                    LowStockThreshold = v.LowStockThreshold,
                    Barcode = v.Barcode,
                    WeightGram = v.WeightGram
                }).ToList() : new List<ProductVariantDto>(),
                Images = p.ProductImages != null ? p.ProductImages.Select(img => new ProductImageDto
                {
                    ImageId = img.ImageId,
                    ProductId = img.ProductId,
                    VariantId = img.VariantId,
                    ImageUrl = img.ImageUrl,
                    AltText = img.AltText,
                    IsThumbnail = img.IsThumbnail,
                    SortOrder = img.SortOrder
                }).ToList() : new List<ProductImageDto>()
            };
        }

        public async Task<List<ReviewDto>> GetReviewsByProductIdAsync(int productId)
        {
            var reviews = await _repository.GetReviewsByProductIdAsync(productId);
            return reviews.Select(r => new ReviewDto
            {
                ActivityId = r.ActivityId,
                UserId = r.UserId,
                ReviewerName = r.User != null ? r.User.FullName : "Khách hàng",
                Rating = r.Rating ?? 0,
                Comment = r.Comment ?? "",
                CreatedAt = r.CreatedAt
            }).ToList();
        }

        public async Task AddReviewAsync(int productId, int userId, CreateReviewRequest request)
        {
            var product = await _repository.GetProductByIdAsync(productId);
            if (product == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy sản phẩm với ID {productId}.");
            }

            var review = new SHOP.CO.Domain.Entities.CustomerActivity
            {
                ProductId = productId,
                UserId = userId,
                ActivityType = "Review",
                Rating = request.Rating,
                Comment = request.Comment,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _repository.AddReviewAsync(review);
        }

        public async Task<List<CategoryDto>> GetActiveCategoriesAsync()
        {
            var categories = await _repository.GetActiveCategoriesAsync();
            return categories.Select(c => new CategoryDto
            {
                CategoryId = c.CategoryId,
                ParentCategoryId = c.ParentCategoryId,
                CategoryName = c.CategoryName,
                Slug = c.Slug,
                SortOrder = c.SortOrder,
                IsActive = c.IsActive
            }).ToList();
        }

        public async Task<bool> ToggleWishlistAsync(int productId, int userId)
        {
            var product = await _repository.GetProductByIdAsync(productId);
            if (product == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy sản phẩm với ID {productId}.");
            }

            var item = await _repository.GetWishlistItemAsync(productId, userId);
            if (item != null)
            {
                await _repository.RemoveWishlistItemAsync(item);
                return false;
            }
            else
            {
                var wishlistActivity = new CustomerActivity
                {
                    ProductId = productId,
                    UserId = userId,
                    ActivityType = "Wishlist",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };
                await _repository.AddWishlistItemAsync(wishlistActivity);
                return true;
            }
        }

        public async Task<List<ProductDto>> GetWishlistAsync(int userId)
        {
            var products = await _repository.GetWishlistProductsAsync(userId);
            return products.Select(MapToDto).ToList();
        }
    }
}
