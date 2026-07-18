using System.Threading.Tasks;
using SHOP.CO.MVC.Models;

namespace SHOP.CO.MVC.Services
{
    public interface IOrderTrackingApiClient
    {
        Task<OrderTrackingViewModel?> GetTrackingAsync(int orderId);
    }
}
