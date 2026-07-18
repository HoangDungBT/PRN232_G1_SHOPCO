using System.Threading.Tasks;
using SHOP.CO.Application.DTOs;


using SHOP.CO.Infrastructure.Data;
using SHOP.CO.Domain.Entities;
using SHOP.CO.Application.Common;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
namespace SHOP.CO.Application.Services
{
    public interface IPaymentService
    {
        /// <summary>
        /// Process a payment for an existing order.
        /// COD: returns Success=true, PaymentUrl=null, PaymentStatus=Unpaid.
        /// VNPay: returns Success=true, PaymentUrl=(signed URL), PaymentStatus=Unpaid.
        /// </summary>
        Task<PaymentResponseDto> ProcessPaymentAsync(PaymentRequestDto request);

        /// <summary>
        /// Handle VNPay callback after user returns from payment gateway.
        /// Updates PaymentStatus to Paid (code=00) or Failed (any other code).
        /// </summary>
        Task<PaymentResponseDto> HandlePaymentCallbackAsync(
            string orderCode,
            string responseCode,
            string secureHash,
            string rawQuery);

        /// <summary>
        /// Simulate a payment callback (for testing/automation).
        /// </summary>
        Task<PaymentResponseDto> SimulateCallbackAsync(string orderCode, string responseCode);
    }
}
