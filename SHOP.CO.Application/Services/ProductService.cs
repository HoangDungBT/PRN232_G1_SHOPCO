using SHOP.CO.Domain.Entities;
﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SHOP.CO.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly SHOP.CO.Infrastructure.Repositories.IProductRepository _repository;

        public ProductService(SHOP.CO.Infrastructure.Repositories.IProductRepository repository)
        {
            _repository = repository;
        }

        public async Task<PagedResult<ProductDto>> GetProductsAsync(string? searchTerm, int pageNumber, int pageSize)
        {
            // Logic mặc định chống lỗi
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 10; // Chặn request lấy quá nhiều data


            var result = await _repository.GetPagedProductAsync(searchTerm, pageNumber, pageSize);

            // chuyển entity => dto
            var dtos = result.Items.Select(p => new ProductDto
            {
                ProductId = p.ProductId,
                ProductName = p.ProductName,
                Slug = p.Slug,
                CategoryName = p.Category?.CategoryName,
                BasePrice = p.BasePrice,
                SalePrice = p.SalePrice,
            }).ToList();

            //trả kq
            return new PagedResult<ProductDto>
            { 
                Items = dtos ,
                TotalCount = dtos.Count,
                PageNumber = pageNumber,
                PageSize = pageSize
            
            };

        }

    public IQueryable<Product> GetProductsQuery() { throw new NotImplementedException(); }
    public Task<IEnumerable<Category>> GetActiveCategoriesAsync() { throw new NotImplementedException(); }
    public Task<bool> ToggleWishlistAsync(int userId, int productId) => Task.FromResult(true);
    public Task<IEnumerable<Product>> GetWishlistAsync(int userId) { throw new NotImplementedException(); }
    public Task<Product> GetProductByIdAsync(int id) { throw new NotImplementedException(); }
    public Task<IEnumerable<Product>> GetRelatedProductsAsync(int productId, int categoryId, int limit) { throw new NotImplementedException(); }
    public Task<IEnumerable<object>> GetReviewsByProductIdAsync(int productId) { throw new NotImplementedException(); }
    public Task AddReviewAsync(object review) { throw new NotImplementedException(); }
    }
}
