
namespace SHOP.CO.Application.Services
{
    public interface IProductService
    {
        // test
        Task<PagedResult<ProductDto>> GetProductsAsync(string? searchTerm, int pageNumber, int pageSize);
        // test

    }
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repo;

        public ProductService(IProductRepository repository)
        {
            _repo= repository;
        }



        public async Task<PagedResult<ProductDto>> GetProductsAsync(string? searchTerm, int pageNumber, int pageSize)
        {
            // Logic mặc định chống lỗi
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 10; // Chặn request lấy quá nhiều data


            var result = await _repo.GetPagedProductAsync(searchTerm, pageNumber, pageSize);

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
                TotalCount = dtos.Count,
                PageNumber = pageNumber,
                PageSize = pageSize
            
            };

        }
    }
}
