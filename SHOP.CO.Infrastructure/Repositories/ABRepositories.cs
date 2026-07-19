
namespace SHOP.CO.Application.Repositories
{
    // 1. INTERFACE GỐC CHỨA CÁC HÀM DÙNG CHUNG CHO MỌI BẢNG
    public interface IBaseRepository<T> where T : class
    {
        IQueryable<T> GetQueryable();
        Task<T?> GetByIdAsync(int id);
        Task<T> AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
    }
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        protected readonly ShopCoDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public BaseRepository(ShopCoDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public virtual IQueryable<T> GetQueryable()
        {
            return _dbSet.AsQueryable();
        }

        public virtual async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public virtual async Task<T> AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public virtual async Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
        }

        public virtual async Task DeleteAsync(T entity)
        {
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
    // 2. CÁC INTERFACE CỤ THỂ KẾ THỪA TỪ IBaseRepository
    public interface ICategoryRepository : IBaseRepository<Category>
    {
        IQueryable<Category> GetCategoriesWithParentAsQueryable();
        Task<bool> IsSlugExistsAsync(string slug, int? excludeId = null);
        Task<bool> HasChildrenAsync(int id);
    }

    public interface IOrderRepository : IBaseRepository<Order>
    {
        Task<Order?> GetOrderWithDetailsAsync(int orderId);
        Task<List<Order>> GetOrdersByUserIdAsync(int userId, string? status = null, string? search = null);
        Task<Order> GetOrderByIdAsync(int orderId);
    }

    public interface IProductRepository : IBaseRepository<Product>
    {
        IQueryable<Product> GetProductsWithDetailsAsQueryable();
        Task<Product?> GetProductWithVariantsByIdAsync(int productId);
        Task SoftDeleteProductAsync(Product product);
        Task<bool> IsSlugExistsAsync(string slug, int? excludeProductId = null);
        Task<bool> IsSkuExistsAsync(string sku);
        Task<(List<Product> Items, int TotalCount)> GetPagedProductAsync(string? searchTerm, int pageNumber, int pageSize);

        IQueryable<Product> GetProductsQuery();
        Task<Product?> GetProductByIdAsync(int id);
        Task<List<Product>> GetRelatedProductsAsync(int categoryId, int excludeProductId, int limit);
        Task<List<CustomerActivity>> GetReviewsByProductIdAsync(int productId);
        Task AddReviewAsync(CustomerActivity review);
        Task<List<Category>> GetActiveCategoriesAsync();
        Task<CustomerActivity?> GetWishlistItemAsync(int productId, int userId);
        Task AddWishlistItemAsync(CustomerActivity wishlistActivity);
        Task RemoveWishlistItemAsync(CustomerActivity wishlistActivity);
        Task<List<Product>> GetWishlistProductsAsync(int userId);
    }

    public interface IUserRepository : IBaseRepository<User>
    {
        Task<bool> EmailExistsAsync(string email);
        Task<User?> GetUserByEmailAsync(string email);
        Task<User?> GetUserByRefreshTokenAsync(string refreshToken);
        Task<User?> GetUserByIdAsync(int id);
        Task AddUserAsync(User user);
        Task UpdateUserAsync(User user);
        void UpdateUser(User user);
        void DeleteUser(User user);
        Task SaveChangesAsync();
    }
}