using System.Threading.Tasks;
using SHOP.CO.MVC.Models;

namespace SHOP.CO.MVC.Services
{
    public interface IPaymentApiClient
    {
        /// <summary>
        /// Call POST /api/payment/process
        /// Returns PaymentResponseViewModel with Success flag and optional PaymentUrl (VNPay).
        /// </summary>
        Task<PaymentResponseViewModel?> ProcessPaymentAsync(int orderId, string paymentMethod, string returnUrl);
    }
}
