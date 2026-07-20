using System.Collections.Generic;
using System.Threading.Tasks;
using SHOP.CO.MVC.Models;

namespace SHOP.CO.MVC.Services
{
    public interface IOrderApiClient
    {
        Task<List<OrderViewModel>> GetOrdersByUserIdAsync(int userId);
        Task<OrderViewModel?> GetOrderByIdAsync(int orderId);
        Task<bool> CancelOrderAsync(int orderId, int userId, string cancelReason);
        Task<bool> ConfirmReceivedAsync(int orderId, int userId);
        Task<bool> SubmitReturnRequestAsync(int orderId, int userId, string reason, string description);
        Task<byte[]?> GetInvoiceAsync(int orderId, int userId);
    }
}
