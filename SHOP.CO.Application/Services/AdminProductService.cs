
namespace SHOP.CO.Application.Services
{
    public interface IAdminProductService
    {
        IQueryable<ProductDto> GetProductODataQuery();
        Task<ResultModel<ProductDetailAdminDto>> GetProductByIdAsync(int id);
        Task<ResultModel<int>> CreateProductAsync(CreateProductRequestDto dto);
        Task<ResultModel<bool>> UpdateAsync(int id, UpdateProductRequestDto dto);
        Task<ResultModel<bool>> DeleteProductAsync(int id);
        Task<ResultModel<bool>> DeleteProductImageAsync(int imageId);
        Task<ResultModel<ProductFormAttributesDto>> GetFormAttributesAsync();
        Task<ResultModel<string>> BulkUpdateStatusAsync(BulkUpdateStatusDto dto);
        Task<ResultModel<string>> BulkUpdateFeaturedAsync(BulkUpdateFeaturedDto dto);
    }

    public class AdminProductService : IAdminProductService
    {
        private readonly IProductRepository _repo;
        private readonly ICategoryRepository _cateRepo;
        private readonly ShopCoDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper; // 🟢 Inject AutoMapper

        public AdminProductService(IProductRepository repository, ICategoryRepository cateRepo, ShopCoDbContext context, IWebHostEnvironment env, IMapper mapper)
        {
            _repo = repository;
            _cateRepo = cateRepo;
            _context = context;
            _env = env;
            _mapper = mapper;
        }

        public async Task<ResultModel<ProductFormAttributesDto>> GetFormAttributesAsync()
        {
            try
            {
                var categories = await _cateRepo.GetCategoriesWithParentAsQueryable()
                    .Select(c => new CategorySimpleDto { Id = c.CategoryId, Name = c.CategoryName })
                    .ToListAsync();

                var dbBrands = await _context.Products.Where(p => !string.IsNullOrEmpty(p.Brand)).Select(p => p.Brand!).Distinct().ToListAsync();
                var dbMaterials = await _context.Products.Where(p => !string.IsNullOrEmpty(p.Material)).Select(p => p.Material!).Distinct().ToListAsync();
                var dbSizes = await _context.ProductVariants.Where(v => !string.IsNullOrEmpty(v.Size)).Select(v => v.Size!).Distinct().ToListAsync();
                var dbColors = await _context.ProductVariants.Where(v => !string.IsNullOrEmpty(v.Color)).Select(v => v.Color!).Distinct().ToListAsync();

                var data = new ProductFormAttributesDto
                {
                    Categories = categories,
                    Brands = SHOP.CO.Domain.Shared.SystemAttributes.Brands.Union(dbBrands).Distinct().ToList(),
                    Materials = SHOP.CO.Domain.Shared.SystemAttributes.Materials.Union(dbMaterials).Distinct().ToList(),
                    Sizes = SHOP.CO.Domain.Shared.SystemAttributes.Sizes.Union(dbSizes).Distinct().ToList(),
                    Colors = SHOP.CO.Domain.Shared.SystemAttributes.Colors.Union(dbColors).Distinct().ToList()
                };

                return ResultModel<ProductFormAttributesDto>.Success(data);
            }
            catch (Exception ex) { return ResultModel<ProductFormAttributesDto>.Exception(ex); }
        }

        public IQueryable<ProductDto> GetProductODataQuery()
        {
            // 🟢 TỰ ĐỘNG MAP: Code rút gọn từ 15 dòng xuống 2 dòng!
            return _repo.GetProductsWithDetailsAsQueryable()
                        .ProjectTo<ProductDto>(_mapper.ConfigurationProvider);
        }

        public async Task<ResultModel<ProductDetailAdminDto>> GetProductByIdAsync(int id)
        {
            try
            {
                var product = await _repo.GetProductWithVariantsByIdAsync(id);
                if (product == null) return ResultModel<ProductDetailAdminDto>.Error("Không tìm thấy sản phẩm", 404);

                // 🟢 TỰ ĐỘNG MAP TOÀN BỘ SẢN PHẨM + BIẾN THỂ + HÌNH ẢNH CHỈ BẰNG 1 DÒNG
                var data = _mapper.Map<ProductDetailAdminDto>(product);

                // Lọc bỏ các biến thể đã bị xóa mềm (Vì AutoMapper map hết)
                data.Variants = data.Variants.Where(v => v.IsActive).ToList();

                return ResultModel<ProductDetailAdminDto>.Success(data);
            }
            catch (Exception ex) { return ResultModel<ProductDetailAdminDto>.Exception(ex); }
        }

        public async Task<ResultModel<int>> CreateProductAsync(CreateProductRequestDto dto)
        {
            try
            {
                string generatedSlug = SlugHelper.GenerateSlug(dto.ProductName);
                if (await _repo.IsSlugExistsAsync(generatedSlug))
                    generatedSlug += $"-{Guid.NewGuid().ToString().Substring(0, 5)}";

                foreach (var variant in dto.Variants)
                {
                    if (await _repo.IsSkuExistsAsync(variant.Sku))
                        return ResultModel<int>.Error($"Mã SKU '{variant.Sku}' đã tồn tại!", 400);
                }

                var newProduct = new Product
                {
                    CategoryId = dto.CategoryId,
                    ProductName = dto.ProductName,
                    Slug = generatedSlug,
                    Brand = dto.Brand,
                    Description = dto.Description,
                    Material = dto.Material,
                    GenderTarget = dto.GenderTarget,
                    BasePrice = dto.BasePrice,
                    SalePrice = dto.SalePrice,
                    IsFeatured = dto.IsFeatured,
                    IsActive = dto.IsActive,
                    CreatedAt = DateTime.UtcNow,
                    IsNewArrival = true
                };

                if (dto.ImageUrls != null && dto.ImageUrls.Count > 0)
                {
                    for (int i = 0; i < dto.ImageUrls.Count; i++)
                    {
                        newProduct.ProductImages.Add(new ProductImage
                        {
                            ImageUrl = dto.ImageUrls[i],
                            IsThumbnail = (i == 0),
                            CreatedAt = DateTime.UtcNow
                        });
                    }
                }

                foreach (var vDto in dto.Variants)
                {
                    if (vDto.StockQuantity < 0) return ResultModel<int>.Error("Số lượng tồn kho không thể nhỏ hơn 0!", 400);

                    newProduct.ProductVariants.Add(new ProductVariant
                    {
                        Sku = vDto.Sku,
                        Size = vDto.Size,
                        Color = vDto.Color,
                        ExtraPrice = vDto.ExtraPrice,
                        StockQuantity = vDto.StockQuantity,
                        LowStockThreshold = 5,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    });
                }

                await _repo.AddAsync(newProduct); // Đã đổi tên theo BaseRepository
                return ResultModel<int>.Success(newProduct.ProductId, "Thêm mới sản phẩm thành công!", 201);
            }
            catch (Exception ex) { return ResultModel<int>.Exception(ex); }
        }

        public async Task<ResultModel<bool>> UpdateAsync(int id, UpdateProductRequestDto dto)
        {
            try
            {
                var product = await _repo.GetProductWithVariantsByIdAsync(id);
                if (product == null) return ResultModel<bool>.Error("Không tìm thấy sản phẩm", 404);

                string newSlug = SlugHelper.GenerateSlug(dto.ProductName);
                if (await _repo.IsSlugExistsAsync(newSlug, id))
                    newSlug += $"-{Guid.NewGuid().ToString().Substring(0, 5)}";

                product.CategoryId = dto.CategoryId;
                product.ProductName = dto.ProductName;
                product.Slug = newSlug;
                product.Brand = dto.Brand;
                product.Description = dto.Description;
                product.Material = dto.Material;
                product.GenderTarget = dto.GenderTarget;
                product.BasePrice = dto.BasePrice;
                product.SalePrice = dto.SalePrice;
                product.IsFeatured = dto.IsFeatured;
                product.IsBestSeller = dto.IsBestSeller;
                product.IsNewArrival = dto.IsNewArrival;
                product.IsActive = dto.IsActive;
                product.UpdatedAt = DateTime.UtcNow;

                if (dto.ImageUrls != null && dto.ImageUrls.Count > 0)
                {
                    _context.ProductImages.RemoveRange(product.ProductImages);
                    foreach (var url in dto.ImageUrls)
                    {
                        product.ProductImages.Add(new ProductImage
                        {
                            ProductId = product.ProductId,
                            ImageUrl = url,
                            IsThumbnail = (product.ProductImages.Count == 0),
                            CreatedAt = DateTime.UtcNow
                        });
                    }
                }

                if (dto.Variants != null && dto.Variants.Any())
                {
                    var dtoSkus = dto.Variants.Select(v => v.Sku).ToList();
                    var varianToRemove = product.ProductVariants.Where(v => !dtoSkus.Contains(v.Sku)).ToList();
                    foreach (var v in varianToRemove) v.IsActive = false;

                    foreach (var vDto in dto.Variants)
                    {
                        var existingVariant = product.ProductVariants.FirstOrDefault(v => v.Sku == vDto.Sku);
                        if (existingVariant != null)
                        {
                            if (vDto.StockQuantity < 0) return ResultModel<bool>.Error("Số lượng tồn kho không thể nhỏ hơn 0!", 400);

                            existingVariant.Size = vDto.Size;
                            existingVariant.Color = vDto.Color;
                            existingVariant.ExtraPrice = vDto.ExtraPrice;
                            existingVariant.StockQuantity = vDto.StockQuantity;
                            existingVariant.IsActive = true;
                            existingVariant.UpdatedAt = DateTime.UtcNow;
                        }
                        else
                        {
                            if (await _repo.IsSkuExistsAsync(vDto.Sku))
                                return ResultModel<bool>.Error($"Mã SKU '{vDto.Sku}' đã tồn tại!", 400);

                            if (vDto.StockQuantity < 0) return ResultModel<bool>.Error("Số lượng tồn kho không thể nhỏ hơn 0!", 400);

                            product.ProductVariants.Add(new ProductVariant
                            {
                                ProductId = product.ProductId,
                                Sku = vDto.Sku,
                                Size = vDto.Size,
                                Color = vDto.Color,
                                ExtraPrice = vDto.ExtraPrice,
                                StockQuantity = vDto.StockQuantity,
                                LowStockThreshold = 5,
                                IsActive = true,
                                CreatedAt = DateTime.UtcNow
                            });
                        }
                    }
                }
                else
                {
                    return ResultModel<bool>.Error("Sản phẩm phải có ít nhất 1 biến thể!", 400);
                }

                await _repo.UpdateAsync(product); // Gọi theo BaseRepository
                return ResultModel<bool>.Success(true, "Cập nhật sản phẩm thành công!");
            }
            catch (Exception ex) { return ResultModel<bool>.Exception(ex); }
        }

        public async Task<ResultModel<bool>> DeleteProductAsync(int id)
        {
            try
            {
                var product = await _repo.GetByIdAsync(id); // Gọi theo BaseRepository
                if (product == null) return ResultModel<bool>.Error("Không tìm thấy sản phẩm", 404);

                product.IsActive = false;
                product.DeletedAt = DateTime.UtcNow;
                await _repo.UpdateAsync(product);

                return ResultModel<bool>.Success(true, "Đã xóa sản phẩm!");
            }
            catch (Exception ex) { return ResultModel<bool>.Exception(ex); }
        }

        public async Task<ResultModel<bool>> DeleteProductImageAsync(int imageId)
        {
            try
            {
                var image = await _context.ProductImages.FindAsync(imageId);
                if (image == null) return ResultModel<bool>.Error("Không tìm thấy ảnh", 404);

                if (!string.IsNullOrEmpty(image.ImageUrl))
                {
                    string webRootPath = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                    string imageRelativePath = image.ImageUrl.TrimStart('/');
                    string exactFilePath = Path.Combine(webRootPath, imageRelativePath);

                    if (System.IO.File.Exists(exactFilePath)) System.IO.File.Delete(exactFilePath);
                }

                _context.ProductImages.Remove(image);
                await _context.SaveChangesAsync();

                return ResultModel<bool>.Success(true, "Xóa ảnh thành công!");
            }
            catch (Exception ex) { return ResultModel<bool>.Exception(ex); }
        }
        public async Task<ResultModel<string>> BulkUpdateStatusAsync(BulkUpdateStatusDto dto)
        {
            try
            {
                if (dto.ProductIds == null || !dto.ProductIds.Any()) 
                    return ResultModel<string>.Error("Không có sản phẩm nào được chọn.");

                var products = await _context.Products.Where(p => dto.ProductIds.Contains(p.ProductId)).ToListAsync();
                foreach (var p in products)
                {
                    p.IsActive = dto.IsActive;
                    p.UpdatedAt = DateTime.UtcNow;
                }
                await _context.SaveChangesAsync();
                return ResultModel<string>.Success($"Đã cập nhật trạng thái {products.Count} sản phẩm!");
            }
            catch (Exception ex) { return ResultModel<string>.Exception(ex); }
        }

        public async Task<ResultModel<string>> BulkUpdateFeaturedAsync(BulkUpdateFeaturedDto dto)
        {
            try
            {
                if (dto.ProductIds == null || !dto.ProductIds.Any()) 
                    return ResultModel<string>.Error("Không có sản phẩm nào được chọn.");

                var products = await _context.Products.Where(p => dto.ProductIds.Contains(p.ProductId)).ToListAsync();
                foreach (var p in products)
                {
                    p.IsFeatured = dto.IsFeatured;
                    p.UpdatedAt = DateTime.UtcNow;
                }
                await _context.SaveChangesAsync();
                return ResultModel<string>.Success($"Đã cập nhật nổi bật {products.Count} sản phẩm!");
            }
            catch (Exception ex) { return ResultModel<string>.Exception(ex); }
        }
    }
}
