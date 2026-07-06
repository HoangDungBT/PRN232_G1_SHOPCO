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
    }
}
//    public interface IProductRepository
//    {
//        IQueryable<Product> GetProductsAsQueryable();
//        Task<Product?> GetProductWithVariantsByIdAsync(int productId);
//        Task<Product> CreateProductWithVariantsAsync(Product product);
//        Task UpdateProductAsync(Product product);
//        Task SoftDeleteProductAsync(Product product);
//        Task<bool> IsSlugExistsAsync(string slug, int? excludeProductId = null);
//        Task<bool> IsSkuExistsAsync(string sku);
//        //Task<List<Product>> GetProductsBiIdsAsync(string sku);



//        // test 
//        Task<(List<Product> Items, int TotalCount)> GetPagedProductAsync(string? searchTerm, int pageNumber, int pageSize);
//        // test

//    }
//    public class ProductRepository : IProductRepository
//    {
//        private readonly ShopCoDbContext _context;
//        public ProductRepository(ShopCoDbContext context)
//        {
//            _context = context;
//        }

//       public async Task<Product?> GetProductWithVariantsByIdAsync(int productId){
//        return await _context.Products
//                .Include(p => p.ProductVariants)
//                .Include(p => p.ProductImages)
//                .FirstOrDefaultAsync(p =>  p.ProductId == productId);
//                }
//       public async Task<Product> CreateProductWithVariantsAsync(Product product)
//        {
//            _context.Products.Add(product);
//            await _context.SaveChangesAsync();
//            return product;
//        }
//       public async Task UpdateProductAsync(Product product)
//        {
//            _context.Products.Update(product);
//            await _context.SaveChangesAsync();
//        }
//       public async Task SoftDeleteProductAsync(Product product)
//        {
//            product.IsActive = false;
//            product.DeletedAt = DateTime.UtcNow;
//            _context.Products.Update(product);
//            await _context.SaveChangesAsync();

//        }
//       public async Task<bool> IsSlugExistsAsync(string slug, int? excludeProductId = null)
//        {
//            var query =  _context.Products.Where(p => p.Slug == slug);
//            if(excludeProductId.HasValue)
//            {
//                query = query.Where(p => p.ProductId != excludeProductId.Value);
//            }
//            return await query.AnyAsync();
//        }
//       public async Task<bool> IsSkuExistsAsync(string sku)
//        {
//            return await _context.ProductVariants.AnyAsync(v => v.Sku == sku);
            
//        }

//        public IQueryable<Product> GetProductsAsQueryable()
//        {
//            return _context.Products
//                .Include(p => p.Category)
//                .Include(p => p.ProductImages)
//                .AsQueryable();
//        }

//        public async Task<(List<Product> Items, int TotalCount)> GetPagedProductAsync(string? searchTerm, int pageNumber, int pageSize)
//        {
//            // khởi tạo query
//            var query = _context.Products.Include(p => p.Category).AsQueryable();

//            // xử lí search 
//            if (!string.IsNullOrWhiteSpace(searchTerm))
//            {
//                var term = searchTerm.Trim().ToLower();
//                query = query.Where(p => p.ProductName.ToLower().Contains(term)
//                || p.Slug.ToLower().Contains(term));
//            }

//            // đếm tổng item để phân trang 
//            int totalCount = await query.CountAsync();

//            // phân trang và lấy dữ liệu(sắp xếp theo mới nhất)
//            var items = await query
//                .OrderByDescending(p => p.CreatedAt)
//                .Skip((pageNumber - 1) * pageSize)
//                .Take(pageSize)
//                .ToListAsync();

//            // trả dữ liệu
//            return (items, totalCount);

//        }
//    }
//}

