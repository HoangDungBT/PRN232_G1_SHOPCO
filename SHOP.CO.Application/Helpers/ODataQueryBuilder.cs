using System;
using System.Collections.Generic;
using System.Linq;

namespace SHOP.CO.Application.Helpers
{
    public static class ODataQueryBuilder
    {
        public static string Build(
            int? categoryId,
            string? brand,
            decimal? minPrice,
            decimal? maxPrice,
            string? size,
            string? color,
            string? sortOrder,
            int page,
            int pageSize = 9)
        {
            var filters = new List<string>();

            // Chỉ lấy các sản phẩm đang active
            filters.Add("IsActive eq true");

            if (categoryId.HasValue)
            {
                filters.Add($"CategoryId eq {categoryId.Value}");
            }

            if (!string.IsNullOrWhiteSpace(brand))
            {
                filters.Add($"Brand eq '{EscapeString(brand)}'");
            }

            if (minPrice.HasValue)
            {
                // Sử dụng SalePrice nếu có sale, ngược lại dùng BasePrice
                filters.Add($"((SalePrice ne null and SalePrice ge {minPrice.Value}) or (SalePrice eq null and BasePrice ge {minPrice.Value}))");
            }

            if (maxPrice.HasValue)
            {
                filters.Add($"((SalePrice ne null and SalePrice le {maxPrice.Value}) or (SalePrice eq null and BasePrice le {maxPrice.Value}))");
            }

            if (!string.IsNullOrWhiteSpace(size))
            {
                filters.Add($"Variants/any(v: v/Size eq '{EscapeString(size)}')");
            }

            if (!string.IsNullOrWhiteSpace(color))
            {
                filters.Add($"Variants/any(v: v/Color eq '{EscapeString(color)}')");
            }

            var queryParams = new List<string>();

            if (filters.Any())
            {
                queryParams.Add($"$filter={string.Join(" and ", filters)}");
            }

            // Sắp xếp (Sort)
            if (!string.IsNullOrWhiteSpace(sortOrder))
            {
                string orderby = sortOrder.ToLower() switch
                {
                    "price_asc" => "BasePrice asc",
                    "price_desc" => "BasePrice desc",
                    "newest" => "ProductId desc",
                    "popular" => "ViewCount desc",
                    _ => ""
                };

                if (!string.IsNullOrEmpty(orderby))
                {
                    queryParams.Add($"$orderby={orderby}");
                }
            }
            else
            {
                // Mặc định sắp xếp theo sản phẩm mới nhất
                queryParams.Add("$orderby=ProductId desc");
            }

            // Phân trang
            if (page < 1) page = 1;
            int skip = (page - 1) * pageSize;
            queryParams.Add($"$skip={skip}");
            queryParams.Add($"$top={pageSize}");
            queryParams.Add("$count=true");

            return "?" + string.Join("&", queryParams);
        }

        public static string EscapeString(string value)
        {
            if (string.IsNullOrEmpty(value)) return "";
            return value.Replace("'", "''");
        }
    }
}
