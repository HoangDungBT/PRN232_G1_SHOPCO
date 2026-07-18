namespace SHOP.CO.Infrastructure.Repositories
{  

    public class CategoryRepository : BaseRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(ShopCoDbContext context) : base(context) { }

        public IQueryable<Category> GetCategoriesWithParentAsQueryable()
        {
            return _dbSet.Include(c => c.ParentCategory).AsQueryable();
        }

        public async Task<bool> IsSlugExistsAsync(string slug, int? excludeId = null)
        {
            var query = _dbSet.Where(c => c.Slug == slug);
            if (excludeId.HasValue)
            {
                query = query.Where(c => c.CategoryId != excludeId.Value);
            }
            return await query.AnyAsync();
        }

        public async Task<bool> HasChildrenAsync(int id)
        {
            return await _dbSet.AnyAsync(c => c.ParentCategoryId == id);
        }
    }
}
