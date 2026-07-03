using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using SHOP.CO.Application.Common;
using SHOP.CO.Domain.Shared;
using SHOP.CO.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SHOP.CO.Application.Services
{
    public interface IProductAdminService
    {
        IQueryable<ProductDto> GetProductODataQuery();

        Task<ResultModel<ProductDetailAdminDto>> GetProductByIdAsync(int id);
        Task<ResultModel<int>> CreateProductAsync(CreateProductRequestDto dto);
        Task<ResultModel<bool>> UpdateProductAsync(int id, UpdateProductRequestDto dto);
        Task<ResultModel<bool>> DeleteProductAsync(int id);
        Task<ResultModel<bool>> DeleteProductImageAsync(int imageId);
        Task<ResultModel<ProductFormAttributesDto>> GetFormAttributesAsync();
    }
    public class ProductAdminService : IProductAdminService
    {
        private readonly IProductRepository _repo;
        private readonly ICategoryRepository _cateRepo;
        private readonly ShopCoDbContext _context;
        private readonly IWebHostEnvironment _env;


        public ProductAdminService(IProductRepository repository, ICategoryRepository cateRepo, ShopCoDbContext context,IWebHostEnvironment env )
        {
            _repo = repository;
            _cateRepo = cateRepo;
            _context = context;
            _env = env;
        }
        // HÀM LẤY ATTRIBUTES(TỰ HỌC TỪ DATABASE) 🟢
        public async Task<ResultModel<ProductFormAttributesDto>> GetFormAttributesAsync()
        {
            try
            {
                // 1. Lấy danh sách Category 
                var categoryEntities = _cateRepo.GetCategoriesAsQueryable();
                var categories = await categoryEntities.Select(c => new CategorySimpleDto { Id = c.CategoryId, Name = c.CategoryName }).ToListAsync();

                // 2. Tự học các giá trị mới từ DB (Quét các giá trị không bị trùng)
                var dbBrands = await _context.Products.Where(p => !string.IsNullOrEmpty(p.Brand)).Select(p => p.Brand!).Distinct().ToListAsync();
                var dbMaterials = await _context.Products.Where(p => !string.IsNullOrEmpty(p.Material)).Select(p => p.Material!).Distinct().ToListAsync();
                var dbSizes = await _context.ProductVariants.Where(v => !string.IsNullOrEmpty(v.Size)).Select(v => v.Size!).Distinct().ToListAsync();
                var dbColors = await _context.ProductVariants.Where(v => !string.IsNullOrEmpty(v.Color)).Select(v => v.Color!).Distinct().ToListAsync();

                // 3. Trộn dữ liệu tĩnh (SystemAttributes) với dữ liệu mới từ DB
                var finalBrands = SHOP.CO.Domain.Shared.SystemAttributes.Brands.Union(dbBrands).Distinct().ToList();
                var finalMaterials = SHOP.CO.Domain.Shared.SystemAttributes.Materials.Union(dbMaterials).Distinct().ToList();
                var finalSizes = SHOP.CO.Domain.Shared.SystemAttributes.Sizes.Union(dbSizes).Distinct().ToList();
                var finalColors = SHOP.CO.Domain.Shared.SystemAttributes.Colors.Union(dbColors).Distinct().ToList();

                var data = new ProductFormAttributesDto
                {
                    Categories = categories,
                    Brands = finalBrands,
                    Materials = finalMaterials,
                    Sizes = finalSizes,
                    Colors = finalColors
                };

                return ResultModel<ProductFormAttributesDto>.Success(data);
            }
            catch (Exception ex)
            {
                return ResultModel<ProductFormAttributesDto>.Exception(ex);
            }
        }

        public IQueryable<ProductDto> GetProductODataQuery()
        {
            return _repo.GetProductsAsQueryable().Select(p => new ProductDto
            {
                ProductId = p.ProductId,
                ProductName = p.ProductName,
                Slug = p.Slug,
                SalePrice = p.SalePrice,
                BasePrice = p.BasePrice,
                CategoryName = p.Category != null ? p.Category.CategoryName : "Không có",
                IsActive = p.IsActive,
                ThumbnailUrl = p.ProductImages.Where(i => i.IsThumbnail).Select(i => i.ImageUrl).FirstOrDefault()
                               ?? p.ProductImages.Select(i => i.ImageUrl).FirstOrDefault(),
                HasLowStock = p.ProductVariants.Any(v => v.StockQuantity <= v.LowStockThreshold),
            });
        }

        public async Task<ResultModel<ProductDetailAdminDto>> GetProductByIdAsync(int id)
        {
            try
            {
                var product = await _repo.GetProductWithVariantsByIdAsync(id);
                if (product == null) { return ResultModel<ProductDetailAdminDto>.Error("Không tìm thấy sản phẩm", 404); }

                var data = new ProductDetailAdminDto
                {
                    ProductId = product.ProductId,
                    CategoryId = product.CategoryId,
                    ProductName = product.ProductName,
                    Brand = product.Brand,
                    Description = product.Description,
                    Material = product.Material,
                    GenderTarget = product.GenderTarget,
                    BasePrice = product.BasePrice,
                    SalePrice = product.SalePrice,
                    IsFeatured = product.IsFeatured,
                    IsBestSeller = product.IsBestSeller,
                    IsNewArrival = product.IsNewArrival,
                    IsActive = product.IsActive,
                    Variants = product.ProductVariants.Select(pv => new CreateVariantDto
                    {
                        Sku = pv.Sku,
                        Size = pv.Size,
                        Color = pv.Color,
                        ExtraPrice = pv.ExtraPrice,
                        StockQuantity = pv.StockQuantity,
                    }).ToList(),
                    Images = product.ProductImages.Select(i => new ProductImageDto
                    {
                        ImageId = i.ImageId,
                        ImageUrl = i.ImageUrl,
                        IsThumbnail = i.IsThumbnail
                    }).ToList()

                };
                return ResultModel<ProductDetailAdminDto>.Success(data);

            }
            catch (Exception ex)
            {

                return ResultModel<ProductDetailAdminDto>.Exception(ex);
            }
        }
        // Tạo mới Sản phẩm kèm theo Biến thể
        public async Task<ResultModel<int>> CreateProductAsync(CreateProductRequestDto dto)
        {
            try
            {
                // 1. Tạo Slug tự động từ Tên sản phẩm
                string generatedSlug = GenerateSlug(dto.ProductName);

                // Chống trùng Slug
                if (await _repo.IsSlugExistsAsync(generatedSlug))
                {
                    generatedSlug += $"-{Guid.NewGuid().ToString().Substring(0, 5)}";
                }

                // 2. Kiểm tra danh sách biến thể có bị trùng mã SKU không
                foreach (var variant in dto.Variants)
                {
                    if (await _repo.IsSkuExistsAsync(variant.Sku))
                    {
                        return ResultModel<int>.Error($"Mã SKU '{variant.Sku}' đã tồn tại trong hệ thống. Vui lòng chọn mã khác!", 400);
                    }
                }

                // 3. Khởi tạo Entity Product (Core)
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
                    AverageRating = 0,
                    ReviewCount = 0,
                    ViewCount = 0,
                    IsBestSeller = false,
                    IsNewArrival = true // Sản phẩm mới tạo mặc định là New Arrival
                };
                if (dto.ImageUrls != null && dto.ImageUrls.Count > 0)
                {
                    for (int i = 0; i < dto.ImageUrls.Count; i++)
                    {
                        newProduct.ProductImages.Add(new ProductImage
                        {
                            ImageUrl = dto.ImageUrls[i],
                            IsThumbnail = (i == 0), // Ảnh đầu tiên được ưu tiên làm Thumbnail
                            CreatedAt = DateTime.UtcNow
                        });
                    }
                }

                // 4. Khởi tạo danh sách Entity Variants (Biến thể)
                foreach (var vDto in dto.Variants)
                {
                    var newVariant = new ProductVariant
                    {
                        Sku = vDto.Sku,
                        Size = vDto.Size,
                        Color = vDto.Color,
                        ExtraPrice = vDto.ExtraPrice,
                        StockQuantity = vDto.StockQuantity,
                        LowStockThreshold = 5, // Cảnh báo khi tồn kho <= 5
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    };

                    // Nối Variant vào Product (Entity Framework sẽ tự xử lý khóa ngoại ProductId)
                    newProduct.ProductVariants.Add(newVariant);
                }

                // 5. Lưu vào Database thông qua Repository
                var savedProduct = await _repo.CreateProductWithVariantsAsync(newProduct);

                // Trả về ProductId vừa được tạo
                return ResultModel<int>.Success(savedProduct.ProductId, "Thêm mới sản phẩm và biến thể thành công!", 201);
            }
            catch (Exception ex)
            {
                return ResultModel<int>.Exception(ex);
            }
        }

        public async Task<ResultModel<bool>> UpdateProductAsync(int id, UpdateProductRequestDto dto)
        {
            try
            {
                var product = await _repo.GetProductWithVariantsByIdAsync(id);
                if (product == null) return ResultModel<bool>.Error("Không tìm thấy sản phẩm", 404);

                // Tạo Slug mới nếu đổi tên
                string newSlug = GenerateSlug(dto.ProductName);
                if (await _repo.IsSlugExistsAsync(newSlug, id))
                {
                    newSlug += $"-{Guid.NewGuid().ToString().Substring(0, 5)}";
                }

                // Cập nhật thông tin
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
                    // Bước A: Xóa bỏ các record ảnh cũ trong bảng ProductImages
                    _context.ProductImages.RemoveRange(product.ProductImages);

                    // Bước B: Thêm các record ảnh mới vào
                    foreach (var url in dto.ImageUrls)
                    {
                        product.ProductImages.Add(new ProductImage
                        {
                            ProductId = product.ProductId,
                            ImageUrl = url,
                            IsThumbnail = (product.ProductImages.Count == 0), // Ảnh đầu tiên làm Thumbnail
                            CreatedAt = DateTime.UtcNow
                        });
                    }
                }

                if (dto.Variants != null && dto.Variants.Any())
                {
                    var dtoSkus = dto.Variants.Select(v => v.Sku).ToList();

                    var varianToRemove = product.ProductVariants.Where(v => !dtoSkus.Contains(v.Sku)).ToList();
                    foreach (var v in varianToRemove)
                    {
                        v.IsActive = false;
                    }

                    foreach (var vDto in dto.Variants)
                    {
                        var existingVariant = product.ProductVariants.FirstOrDefault(v => v.Sku == vDto.Sku);
                        if(existingVariant != null)
                        {
                            existingVariant.Size = vDto.Size;
                            existingVariant.Color = vDto.Color;
                            existingVariant.ExtraPrice = vDto.ExtraPrice;
                            existingVariant.StockQuantity = vDto.StockQuantity;
                            existingVariant.IsActive = true; // Kích hoạt lại nếu lỡ bị khóa trước đó
                            existingVariant.UpdatedAt = DateTime.UtcNow;
                        }
                        else
                        {
                            if (await _repo.IsSkuExistsAsync(vDto.Sku))
                            {
                                return ResultModel<bool>.Error($"Mã SKU '{vDto.Sku}' đã tồn tại ở một sản phẩm khác. Vui lòng chọn mã khác!", 400);
                            }
                            // Thêm mới
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



                await _repo.UpdateProductAsync(product);
                return ResultModel<bool>.Success(true, "Cập nhật sản phẩm thành công!");
            }
            catch (Exception ex) { return ResultModel<bool>.Exception(ex); }
        }
    


        public async Task<ResultModel<bool>> DeleteProductAsync(int id)
        {
            try
            {
                var product = await _repo.GetProductWithVariantsByIdAsync(id);
                if (product == null) return ResultModel<bool>.Error("Không tìm thấy sản phẩm", 404);

                await _repo.SoftDeleteProductAsync(product);
                return ResultModel<bool>.Success(true, "Đã xóa sản phẩm!");
            }
            catch (Exception ex) { return ResultModel<bool>.Exception(ex); }
        }
        public async Task<ResultModel<bool>> DeleteProductImageAsync(int imageId)
        {
            try
            {
                // Lấy ảnh từ Database
                var image = await _context.ProductImages.FindAsync(imageId);
                if (image == null) return ResultModel<bool>.Error("Không tìm thấy ảnh", 404);

                // Xóa file vật lý trong thư mục wwwroot
                if (!string.IsNullOrEmpty(image.ImageUrl))
                {
                    // Lấy WebRootPath, nếu rỗng thì tự build đường dẫn
                    string webRootPath = _env.WebRootPath;
                    if (string.IsNullOrWhiteSpace(webRootPath))
                    {
                        webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                    }

                    // Loại bỏ dấu '/' ở đầu ImageUrl để nối chuỗi cho chuẩn (VD: /uploads/products/anh.jpg -> uploads/products/anh.jpg)
                    string imageRelativePath = image.ImageUrl.TrimStart('/');
                    string exactFilePath = Path.Combine(webRootPath, imageRelativePath);

                    // Xóa file nếu nó tồn tại trên ổ cứng
                    if (System.IO.File.Exists(exactFilePath))
                    {
                        System.IO.File.Delete(exactFilePath);
                    }
                }

                // Xóa dòng dữ liệu trong Database
                _context.ProductImages.Remove(image);
                await _context.SaveChangesAsync();

                return ResultModel<bool>.Success(true, "Xóa ảnh thành công!");
            }
            catch (Exception ex)
            {
                return ResultModel<bool>.Exception(ex);
            }
        }

        // Hàm tiện ích nội bộ để tự tạo chữ không dấu làm đường dẫn (VD: "Áo Thun" -> "ao-thun")
        private string GenerateSlug(string phrase)
        {
            string str = phrase.ToLower();
            str = Regex.Replace(str, @"á|à|ả|ạ|ã|ă|ắ|ằ|ẳ|ặ|ẵ|â|ấ|ầ|ẩ|ậ|ẫ", "a");
            str = Regex.Replace(str, @"é|è|ẻ|ẹ|ẽ|ê|ế|ề|ể|ệ|ễ", "e");
            str = Regex.Replace(str, @"í|ì|ỉ|ị|ĩ", "i");
            str = Regex.Replace(str, @"ó|ò|ỏ|ọ|õ|ô|ố|ồ|ổ|ộ|ỗ|ơ|ớ|ờ|ở|ợ|ỡ", "o");
            str = Regex.Replace(str, @"ú|ù|ủ|ụ|ũ|ư|ứ|ừ|ử|ự|ữ", "u");
            str = Regex.Replace(str, @"ý|ỳ|ỷ|ỵ|ỹ", "y");
            str = Regex.Replace(str, @"đ", "d");
            str = Regex.Replace(str, @"[^a-z0-9\s-]", ""); // Xóa ký tự đặc biệt
            str = Regex.Replace(str, @"\s+", " ").Trim(); // Xóa khoảng trắng thừa
            str = Regex.Replace(str, @"\s", "-"); // Thay khoảng trắng bằng dấu -
            return str;
        }
    }
}
