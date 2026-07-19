using SHOP.CO.MVC.Models;

namespace SHOP.CO.MVC.Services
{
    public interface IProductApiClient
    {
        Task<List<ProductVM>> GetProductsAsync();
        Task<ProductVM> GetProductByIdAsync(int id);
    }
}
