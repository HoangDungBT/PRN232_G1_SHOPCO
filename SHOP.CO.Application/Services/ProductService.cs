using SHOP.CO.Domain.Entities;
using SHOP.CO.Application.Repositories;
using SHOP.CO.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SHOP.CO.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly SHOP.CO.Application.Repositories.IProductRepository _repository;

        public ProductService(SHOP.CO.Application.Repositories.IProductRepository repository)
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
                ProductName = p.ProductName,
                Slug = p.Slug,
                CategoryName = p.Category?.CategoryName,
                BasePrice = p.BasePrice,
                SalePrice = p.SalePrice,
            }).ToList();

            //trả kq
            return new PagedResult<ProductDto>
            { 
                Items = dtos ,
                TotalCount = result.TotalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            
            };

        }

        public IQueryable<Product> GetProductsQuery()
        {
            return _repository.GetProductsQuery();
        }

        public async Task<IEnumerable<Category>> GetActiveCategoriesAsync()
        {
            return await _repository.GetActiveCategoriesAsync();
        }

        public async Task<Product> GetProductByIdAsync(int id)
        {
            return await _repository.GetProductByIdAsync(id);
        }

        public async Task<IEnumerable<Product>> GetRelatedProductsAsync(int productId, int categoryId, int limit)
        {
            int catId = categoryId;
            if (catId == 0)
            {
                var product = await _repository.GetProductByIdAsync(productId);
                if (product == null)
                {
                    return Enumerable.Empty<Product>();
                }
                catId = product.CategoryId;
            }
            return await _repository.GetRelatedProductsAsync(catId, productId, limit);
        }

        public async Task<IEnumerable<ReviewDto>> GetReviewsByProductIdAsync(int productId)
        {
            var activities = await _repository.GetReviewsByProductIdAsync(productId);
            return activities.Select(a => new ReviewDto
            {
                ActivityId = a.ActivityId,
                UserId = a.UserId,
                ReviewerName = a.User?.FullName ?? "Khách hàng",
                Rating = a.Rating ?? 0,
                Comment = a.Comment ?? "",
                CreatedAt = a.CreatedAt
            }).ToList();
        }

        public async Task AddReviewAsync(int userId, int productId, int rating, string comment)
        {
            var reviewActivity = new CustomerActivity
            {
                UserId = userId,
                ProductId = productId,
                ActivityType = "Review",
                Rating = rating,
                Comment = comment,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            await _repository.AddReviewAsync(reviewActivity);
        }

        public async Task<bool> ToggleWishlistAsync(int productId, int userId)
        {
            var product = await _repository.GetProductByIdAsync(productId);
            if (product == null)
            {
                throw new KeyNotFoundException($"Product with ID {productId} not found.");
            }

            var wishlistItem = await _repository.GetWishlistItemAsync(productId, userId);
            if (wishlistItem != null)
            {
                await _repository.RemoveWishlistItemAsync(wishlistItem);
                return false;
            }
            else
            {
                var wishlistActivity = new CustomerActivity
                {
                    UserId = userId,
                    ProductId = productId,
                    ActivityType = "Wishlist",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };
                await _repository.AddWishlistItemAsync(wishlistActivity);
                return true;
            }
        }

        public async Task<IEnumerable<Product>> GetWishlistAsync(int userId)
        {
            return await _repository.GetWishlistProductsAsync(userId);
        }
    }
}
