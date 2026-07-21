using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SHOP.CO.Application.Repositories;
using SHOP.CO.Domain.Entities;
using SHOP.CO.Infrastructure.Data;

namespace SHOP.CO.Infrastructure.Repositories
{
  
    public class ProductRepository : BaseRepository<Product>, SHOP.CO.Application.Repositories.IProductRepository, SHOP.CO.Infrastructure.Repositories.IProductRepository
    {
        public ProductRepository(ShopCoDbContext context) : base(context) { }

        public async Task<Product?> GetProductWithVariantsByIdAsync(int productId)
        {
            return await _dbSet
                .Include(p => p.ProductVariants)
                .Include(p => p.ProductImages)
                .FirstOrDefaultAsync(p => p.ProductId == productId);
        }

        public async Task SoftDeleteProductAsync(Product product)
        {
            product.IsActive = false;
            product.DeletedAt = DateTime.UtcNow;
            _dbSet.Update(product);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsSlugExistsAsync(string slug, int? excludeProductId = null)
        {
            var query = _dbSet.Where(p => p.Slug == slug);
            if (excludeProductId.HasValue)
            {
                query = query.Where(p => p.ProductId != excludeProductId.Value);
            }
            return await query.AnyAsync();
        }

        public async Task<bool> IsSkuExistsAsync(string sku)
        {
            return await _context.ProductVariants.AnyAsync(v => v.Sku == sku);
        }

        public IQueryable<Product> GetProductsWithDetailsAsQueryable()
        {
            return _dbSet
                .Include(p => p.Category)
                .Include(p => p.ProductImages)
                .AsQueryable();
        }

        public async Task<(List<Product> Items, int TotalCount)> GetPagedProductAsync(string? searchTerm, int pageNumber, int pageSize)
        {
            var query = _dbSet.Include(p => p.Category).AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.Trim().ToLower();
                query = query.Where(p => p.ProductName.ToLower().Contains(term) || p.Slug.ToLower().Contains(term));
            }

            int totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(p => p.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public IQueryable<Product> GetProductsQuery()
        {
            return _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductVariants)
                .Include(p => p.ProductImages)
                .AsQueryable();
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductVariants)
                .Include(p => p.ProductImages)
                .FirstOrDefaultAsync(p => p.ProductId == id);
        }

        public async Task<List<Product>> GetRelatedProductsAsync(int categoryId, int excludeProductId, int limit)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductImages)
                .Where(p => p.IsActive && p.CategoryId == categoryId && p.ProductId != excludeProductId)
                .OrderByDescending(p => p.CreatedAt)
                .Take(limit)
                .ToListAsync();
        }

        public async Task<List<CustomerActivity>> GetReviewsByProductIdAsync(int productId)
        {
            return await _context.CustomerActivities
                .Include(a => a.User)
                .Where(a => a.ProductId == productId && a.ActivityType == "Review" && a.IsActive)
                .OrderByDescending(a => a.CreatedAt)
                .Take(50)
                .ToListAsync();
        }

        public async Task AddReviewAsync(CustomerActivity review)
        {
            await _context.CustomerActivities.AddAsync(review);
            await _context.SaveChangesAsync();

            var validRatings = await _context.CustomerActivities
                .Where(a => a.ProductId == review.ProductId && a.ActivityType == "Review" && a.IsActive && a.Rating.HasValue)
                .Select(a => (decimal)a.Rating!.Value)
                .ToListAsync();

            var product = await _context.Products.FindAsync(review.ProductId);
            if (product != null)
            {
                product.ReviewCount = validRatings.Count;
                product.AverageRating = validRatings.Any() ? Math.Round(validRatings.Average(), 2) : 0m;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Category>> GetActiveCategoriesAsync()
        {
            return await _context.Categories
                .Where(c => c.IsActive)
                .OrderBy(c => c.SortOrder)
                .ToListAsync();
        }

        public async Task<CustomerActivity?> GetWishlistItemAsync(int productId, int userId)
        {
            return await _context.CustomerActivities
                .FirstOrDefaultAsync(a => a.ProductId == productId && a.UserId == userId && a.ActivityType == "Wishlist" && a.IsActive);
        }

        public async Task AddWishlistItemAsync(CustomerActivity wishlistActivity)
        {
            await _context.CustomerActivities.AddAsync(wishlistActivity);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveWishlistItemAsync(CustomerActivity wishlistActivity)
        {
            _context.CustomerActivities.Remove(wishlistActivity);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Product>> GetWishlistProductsAsync(int userId)
        {
            return await _context.CustomerActivities
                .Where(a => a.UserId == userId && a.ActivityType == "Wishlist" && a.IsActive && a.Product != null)
                .Include(a => a.Product)
                    .ThenInclude(p => p!.ProductImages)
                .Include(a => a.Product)
                    .ThenInclude(p => p!.ProductVariants)
                .Include(a => a.Product)
                    .ThenInclude(p => p!.Category)
                .Select(a => a.Product!)
                .ToListAsync();
        }

        public async Task<bool> HasUserPurchasedProductAsync(int userId, int productId)
        {
            return await _context.OrderItems
                .Include(oi => oi.Order)
                .Include(oi => oi.ProductVariant)
                .AnyAsync(oi => oi.Order.UserId == userId 
                             && oi.ProductVariant.ProductId == productId 
                             && oi.Order.OrderStatus != "Canceled");
        }

        public async Task<bool> HasUserAlreadyReviewedProductAsync(int userId, int productId)
        {
            return await _context.CustomerActivities
                .AnyAsync(a => a.UserId == userId 
                             && a.ProductId == productId 
                             && a.ActivityType == "Review" 
                             && a.IsActive);
        }

        public async Task<int> GetCompletedPurchaseCountAsync(int userId, int productId)
        {
            return await _context.OrderItems
                .Include(oi => oi.Order)
                .Include(oi => oi.ProductVariant)
                .Where(oi => oi.Order.UserId == userId 
                          && oi.ProductVariant.ProductId == productId 
                          && oi.Order.OrderStatus == "Completed")
                .Select(oi => oi.OrderId)
                .Distinct()
                .CountAsync();
        }

        public async Task<int> GetUserReviewCountForProductAsync(int userId, int productId)
        {
            return await _context.CustomerActivities
                .CountAsync(a => a.UserId == userId 
                              && a.ProductId == productId 
                              && a.ActivityType == "Review" 
                              && a.IsActive);
        }

        public async Task SyncAllProductRatingsAsync()
        {
            var products = await _context.Products.ToListAsync();
            var allReviews = await _context.CustomerActivities
                .Where(a => a.ProductId.HasValue && a.ActivityType == "Review" && a.IsActive && a.Rating.HasValue)
                .ToListAsync();

            var reviewsByProduct = allReviews
                .GroupBy(r => r.ProductId!.Value)
                .ToDictionary(g => g.Key, g => g.ToList());

            bool isModified = false;
            foreach (var product in products)
            {
                if (reviewsByProduct.TryGetValue(product.ProductId, out var reviews) && reviews.Any())
                {
                    var count = reviews.Count;
                    var avg = Math.Round((decimal)reviews.Average(r => r.Rating!.Value), 2);

                    if (product.ReviewCount != count || product.AverageRating != avg)
                    {
                        product.ReviewCount = count;
                        product.AverageRating = avg;
                        isModified = true;
                    }
                }
                else
                {
                    if (product.ReviewCount != 0 || product.AverageRating != 0m)
                    {
                        product.ReviewCount = 0;
                        product.AverageRating = 0m;
                        isModified = true;
                    }
                }
            }

            if (isModified)
            {
                await _context.SaveChangesAsync();
            }
        }
    }
}
