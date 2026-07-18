using System.Threading.Tasks;
using SHOP.CO.Application.DTOs;


using SHOP.CO.Infrastructure.Data;
using SHOP.CO.Domain.Entities;
using SHOP.CO.Application.Common;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
namespace SHOP.CO.Application.Services
{
    public interface ICouponService
    {
        Task<CouponResponseDto> ApplyCouponAsync(int userId, string couponCode);
    }
}
