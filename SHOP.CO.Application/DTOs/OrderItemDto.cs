using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SHOP.CO.Application.DTOs
{
    public class OrderItemDto
    {
        public string ProductNameSnapshot { get; set; } = string.Empty;
        public string? SizeSnapshot { get; set; }
        public string? ColorSnapshot { get; set; }
        public string? ImageUrlSnapshot { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal { get; set; }
    }
}
