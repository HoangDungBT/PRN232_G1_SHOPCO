using Microsoft.EntityFrameworkCore;
using SHOP.CO.Application.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SHOP.CO.Application.Services
{
    public interface ICategoryAdminService
    {
        IQueryable<CategoryDto> GetODataQuery();
        Task<ResultModel<CategoryDto>> GetByIdAsync(int id);
        Task<ResultModel<int>> CreateAsync(SaveCategoryRequestDto dto);
        Task<ResultModel<bool>> UpdateAsync(int id, SaveCategoryRequestDto dto);
        Task<ResultModel<bool>> SoftDeleteAsync(int id);
        Task<ResultModel<bool>> ToggleCategoryStatusAsync(int id);
    }

    public class CategoryAdminService : ICategoryAdminService
    {
        private readonly ICategoryRepository _repo;

        public CategoryAdminService(ICategoryRepository repo)
        {
            _repo = repo;
        }

        public IQueryable<CategoryDto> GetODataQuery()
        {
            return _repo.GetCategoriesAsQueryable()
                .Select(c => new CategoryDto
                {
                    CategoryId = c.CategoryId,
                    ParentCategoryId = c.ParentCategoryId,
                    ParentCategoryName = c.ParentCategory != null ? c.ParentCategory.CategoryName : null,
                    CategoryName = c.CategoryName,
                    Slug = c.Slug,
                    Description = c.Description,
                    SortOrder = c.SortOrder,
                    IsActive = c.IsActive,
                    CreatedAt = c.CreatedAt

                });
        }
        public async Task<ResultModel<CategoryDto>> GetByIdAsync(int id)
        {
            try
            {
                var c = await _repo.GetByIdAsync(id);
                if (c == null) { return ResultModel<CategoryDto>.Error("Không tìm thấy Category này!", 404); }

                var data = new CategoryDto
                {
                    CategoryId = c.CategoryId,
                    ParentCategoryId = c.ParentCategoryId,
                    CategoryName= c.CategoryName,
                    Description = c.Description,    
                    SortOrder = c.SortOrder,
                    IsActive = c.IsActive,
                };

                return ResultModel<CategoryDto>.Success(data);

            }
            catch (Exception ex )
            {

                return ResultModel<CategoryDto>.Exception(ex);
            }
        }


        public async Task<ResultModel<int>> CreateAsync(SaveCategoryRequestDto dto)
        {
            try
            {
                // Tránh lỗi gán cha bằng chính nó (Mặc dù Create thì chưa có ID nhưng rào cho chắc)
                if (dto.ParentCategoryId.HasValue && dto.ParentCategoryId <= 0) dto.ParentCategoryId = null;

                string generatedSlug = GenerateSlug(dto.CategoryName);
                if (await _repo.IsSlugExistsAsync(generatedSlug))
                {
                    generatedSlug += $"-{Guid.NewGuid().ToString().Substring(0, 5)}";
                }

                var newCat = new Category
                {
                    ParentCategoryId = dto.ParentCategoryId,
                    CategoryName = dto.CategoryName,
                    Slug = generatedSlug,
                    Description = dto.Description,
                    ImageUrl = dto.ImageUrl,
                    SortOrder = dto.SortOrder,
                    IsActive = dto.IsActive,
                    CreatedAt = DateTime.UtcNow
                };

                await _repo.AddAsync(newCat);
                return ResultModel<int>.Success(newCat.CategoryId, "Thêm danh mục thành công!", 201);
            }
            catch (Exception ex) { return ResultModel<int>.Exception(ex); }
        }

        public async Task<ResultModel<bool>> UpdateAsync(int id, SaveCategoryRequestDto dto)
        {
            try
            {
                var cat = await _repo.GetByIdAsync(id);
                if (cat == null) return ResultModel<bool>.Error("Không tìm thấy danh mục", 404);

                // Validation: Chống vòng lặp đệ quy vô tận (Lấy chính nó làm cha)
                if (dto.ParentCategoryId == id)
                    return ResultModel<bool>.Error("Danh mục không thể nhận chính nó làm danh mục cha!", 400);

                string newSlug = GenerateSlug(dto.CategoryName);
                if (await _repo.IsSlugExistsAsync(newSlug, id))
                {
                    newSlug += $"-{Guid.NewGuid().ToString().Substring(0, 5)}";
                }

                cat.ParentCategoryId = dto.ParentCategoryId;
                cat.CategoryName = dto.CategoryName;
                cat.Slug = newSlug;
                cat.Description = dto.Description;
                cat.ImageUrl = dto.ImageUrl;
                cat.SortOrder = dto.SortOrder;
                cat.IsActive = dto.IsActive;
                cat.UpdatedAt = DateTime.UtcNow;

                await _repo.UpdateAsync(cat);
                return ResultModel<bool>.Success(true, "Cập nhật thành công!");
            }
            catch (Exception ex) { return ResultModel<bool>.Exception(ex); }
        }
        public async Task<ResultModel<bool>> ToggleCategoryStatusAsync(int id)
        {
            try
            {
                var category = await _repo.GetByIdAsync(id);
                if (category == null) return ResultModel<bool>.Error("Không tìm thấy danh mục", 404);

                // Đảo ngược trạng thái: Đang hoạt động -> Khóa, Đã khóa -> Hoạt động
                category.IsActive = !category.IsActive;
                category.UpdatedAt = DateTime.UtcNow;

                await _repo.UpdateAsync(category);

                string msg = category.IsActive ? "Đã mở khóa danh mục!" : "Đã khóa danh mục!";
                return ResultModel<bool>.Success(true, msg);
            }
            catch (Exception ex)
            {
                return ResultModel<bool>.Exception(ex);
            }
        }
        public async Task<ResultModel<bool>> SoftDeleteAsync(int id)
        {
            try
            {
                var cat = await _repo.GetByIdAsync(id);
                if (cat == null) return ResultModel<bool>.Error("Không tìm thấy danh mục", 404);

                // Validation: Nếu đang có danh mục con, không cho xóa để tránh mồ côi
                if (await _repo.HasChildrenAsync(id))
                    return ResultModel<bool>.Error("Không thể khóa! Danh mục này đang chứa các danh mục con.", 400);
                if(cat.IsActive == false) {
                    cat.IsActive = true;
                    cat.UpdatedAt = DateTime.UtcNow;
                    await _repo.UpdateAsync(cat);

                    return ResultModel<bool>.Success(true, "Đã mở khóa danh mục thành công!");
                }
                cat.IsActive = false; // Xóa mềm = Ẩn đi
                cat.UpdatedAt = DateTime.UtcNow;
                await _repo.UpdateAsync(cat);

                return ResultModel<bool>.Success(true, "Đã khóa danh mục thành công!");
            }
            catch (Exception ex) { return ResultModel<bool>.Exception(ex); }
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
