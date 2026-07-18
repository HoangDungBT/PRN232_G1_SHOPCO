using System.Threading.Tasks;

namespace SHOP.CO.Application.Services
{
    public interface IInvoiceService
    {
        Task<byte[]> GenerateInvoiceAsync(int orderId, int userId);
    }
}
