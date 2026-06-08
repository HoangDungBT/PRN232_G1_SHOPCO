using SHOP.CO.Application.DTOs;

namespace SHOP.CO.Application.Services
{
    public interface IProductUiService
    {
        Task<IEnumerable<ProductUiDto>> GetUiProductsAsync();
        Task<ProductUiDto?> GetUiProductByIdAsync(int id);
    }
}
