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
  
    public class ProductRepository : BaseRepository<Product>, IProductRepository
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
            var ratings = await _context.CustomerActivities
                .Where(a => a.ProductId == review.ProductId && a.ActivityType == "Review" && a.IsActive)
                .Select(a => a.Rating)
                .ToListAsync();

            if (review.IsActive && review.ActivityType == "Review")
            {
                ratings.Add(review.Rating);
            }

            var product = await _context.Products.FindAsync(review.ProductId);
            if (product != null)
            {
                product.ReviewCount = ratings.Count;
                product.AverageRating = ratings.Any() ? Math.Round((decimal)ratings.Average(r => r ?? 0), 2) : 0;
            }

            await _context.SaveChangesAsync();
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
    }
}
