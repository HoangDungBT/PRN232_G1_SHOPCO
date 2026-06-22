using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SHOP.CO.Infrastructure.Repositories
{
    public interface ICategoryRepository
    {
        IQueryable<Category> GetCategoriesAsQueryable(); // Dùng cho OData và Select tối ưu
        Task<Category?> GetByIdAsync(int id);
        Task<Category> AddAsync(Category category);
        Task UpdateAsync(Category category);
        Task<bool> IsSlugExistsAsync(string slug, int? excludeId = null);
        Task<bool> HasChildrenAsync(int id); // Kiểm tra xem danh mục có danh mục con không

    }
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ShopCoDbContext _context;

        public CategoryRepository(ShopCoDbContext context)
        {
            _context = context;
        }
        public IQueryable<Category> GetCategoriesAsQueryable()
        {
            return _context.Categories.Include(c => c.ParentCategory).AsQueryable();
        }
        public async Task<Category?> GetByIdAsync(int id)
        {
            return await _context.Categories
                .FindAsync(id);
        }

        public async Task<Category> AddAsync(Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task UpdateAsync(Category category)
        {
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
        }
        public async Task<bool> IsSlugExistsAsync(string slug, int? excludeId = null)
        {
            var query = _context.Categories.Where(c => c.Slug == slug);
            if (excludeId.HasValue)
            {
                query = query.Where(c => c.CategoryId != excludeId.Value);
            }
            return await query.AnyAsync();
        }

        public async Task<bool> HasChildrenAsync(int id)
        {
            return await _context.Categories.AnyAsync(c => c.ParentCategoryId == id);
        }



    }
}
