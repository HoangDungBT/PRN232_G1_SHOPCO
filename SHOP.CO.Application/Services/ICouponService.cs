using System.Threading.Tasks;
using SHOP.CO.Application.DTOs;

namespace SHOP.CO.Application.Services
{
    public interface ICouponService
    {
        Task<CouponResponseDto> ApplyCouponAsync(int userId, string couponCode);
    }
}
