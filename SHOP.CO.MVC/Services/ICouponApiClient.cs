using System.Threading.Tasks;
using SHOP.CO.MVC.Models;

namespace SHOP.CO.MVC.Services
{
    public interface ICouponApiClient
    {
        Task<CouponResponseViewModel?> ApplyCouponAsync(int userId, string couponCode);
    }
}
