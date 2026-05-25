using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SHOP.CO.Application.DTOs
{
    public class ProductDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public string Slug { get; set; } = null!;
        public decimal BasePrice { get; set; }
        public decimal? SalePrice { get; set; }
        public string? CategoryName { get; set; }
    }
}
