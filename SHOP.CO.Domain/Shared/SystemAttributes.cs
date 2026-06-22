using System.Collections.Generic;

namespace SHOP.CO.Domain.Shared
{
    // Nơi lưu trữ các giá trị hệ thống ít thay đổi
    public static class SystemAttributes
    {
        public static readonly List<string> Brands = new()
        {
            "Nike", "Adidas", "Uniqlo", "Zara", "H&M", "Gucci", "Local Brand", "Khác"
        };

        public static readonly List<string> Materials = new()
        {
            "100% Cotton", "Polyester", "Kaki", "Denim", "Len", "Lụa", "Nỉ", "Khác"
        };

        public static readonly List<string> Sizes = new()
        {
            "Freesize", "XS", "S", "M", "L", "XL", "XXL", "3XL"
        };

        public static readonly List<string> Colors = new()
        {
            "Đen", "Trắng", "Đỏ", "Xanh Dương", "Xanh Lá", "Vàng", "Xám", "Nâu", "Hồng", "Nhiều màu"
        };
    }
}