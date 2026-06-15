using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SHOP.CO.Application.Services
{
    public interface IProductService
    {
        Task<PagedResult<ProductDto>> GetProductsAsync(string? searchTerm, int pageNumber, int pageSize);
        IQueryable<ProductDto> GetProductsQuery();
        Task<ProductDto?> GetProductByIdAsync(int id);
        Task<List<ProductDto>> GetRelatedProductsAsync(int productId, int limit);
    }
}
