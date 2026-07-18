using System.Threading.Tasks;


using SHOP.CO.Infrastructure.Data;
using SHOP.CO.Domain.Entities;
using SHOP.CO.Application.DTOs;
using SHOP.CO.Application.Common;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
namespace SHOP.CO.Application.Services
{
    public interface IInvoiceService
    {
        Task<byte[]> GenerateInvoiceAsync(int orderId, int userId);
    }
}
