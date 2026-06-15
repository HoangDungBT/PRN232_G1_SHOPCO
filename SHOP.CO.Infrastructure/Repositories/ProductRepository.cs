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
    }
}

