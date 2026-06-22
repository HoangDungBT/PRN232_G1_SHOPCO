using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SHOP.CO.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ShopCoDbContext _context;
        public ProductRepository(ShopCoDbContext context)
        {
            _context = context;
        }

        public async Task<(List<Product> Items, int TotalCount)> GetPagedProductAsync(string? searchTerm, int pageNumber, int pageSize)
        {
            // khởi tạo query
            var query = _context.Products.Include(p => p.Category).AsQueryable();

            // xử lí search 
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.Trim().ToLower();
                query = query.Where(p => p.ProductName.ToLower().Contains(term)
                || p.Slug.ToLower().Contains(term));
            }

            // đếm tổng item để phân trang 
            int totalCount = await query.CountAsync();

            // phân trang và lấy dữ liệu(sắp xếp theo mới nhất)
            var items = await query
                .OrderByDescending(p => p.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // trả dữ liệu
            return(items,  totalCount);

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
            // 1. Thêm đánh giá vào Tracker bộ nhớ của EF
            await _context.CustomerActivities.AddAsync(review);

            // 2. Truy vấn các rating hiện tại từ DB (chưa lưu đánh giá mới)
            var ratings = await _context.CustomerActivities
                .Where(a => a.ProductId == review.ProductId && a.ActivityType == "Review" && a.IsActive)
                .Select(a => a.Rating)
                .ToListAsync();

            // 3. Đưa đánh giá mới đang ở bộ nhớ vào danh sách tính toán luôn
            if (review.IsActive && review.ActivityType == "Review")
            {
                ratings.Add(review.Rating);
            }

            // 4. Tìm sản phẩm và cập nhật các chỉ số tổng hợp
            var product = await _context.Products.FindAsync(review.ProductId);
            if (product != null)
            {
                product.ReviewCount = ratings.Count;
                product.AverageRating = ratings.Any() ? Math.Round((decimal)ratings.Average(r => r ?? 0), 2) : 0;
            }

            // 5. Lưu toàn bộ thay đổi trong một Transaction duy nhất
            await _context.SaveChangesAsync();
        }

        public async Task<List<Category>> GetActiveCategoriesAsync()
        {
            return await _context.Categories
                .Where(c => c.IsActive)
                .OrderBy(c => c.SortOrder)
                .ToListAsync();
        }
    }
}

