


namespace SHOP.CO.Application.Services
{
    public interface IAdminCategoryService
    {
        IQueryable<CategoryDto> GetODataQuery();
        Task<ResultModel<CategoryDto>> GetByIdAsync(int id);
        Task<ResultModel<int>> CreateAsync(SaveCategoryRequestDto dto);
        Task<ResultModel<bool>> UpdateAsync(int id, SaveCategoryRequestDto dto);
        Task<ResultModel<bool>> SoftDeleteAsync(int id);
        Task<ResultModel<bool>> ToggleCategoryStatusAsync(int id);
    }

    public class AdminCategoryService : IAdminCategoryService
    {
        private readonly ICategoryRepository _repo;
        private readonly IMapper _mapper; // 🟢 Inject AutoMapper

        public AdminCategoryService(ICategoryRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public IQueryable<CategoryDto> GetODataQuery()
        {
            // 🟢 TỰ ĐỘNG MAP TỪ IQueryable CỦA EF CORE SANG DTO
            return _repo.GetCategoriesWithParentAsQueryable()
                        .ProjectTo<CategoryDto>(_mapper.ConfigurationProvider);
        }

        public async Task<ResultModel<CategoryDto>> GetByIdAsync(int id)
        {
            try
            {
                var c = await _repo.GetByIdAsync(id);
                if (c == null) return ResultModel<CategoryDto>.Error("Không tìm thấy Category này!", 404);

                // 🟢 TỰ ĐỘNG MAP CHỈ VỚI 1 DÒNG
                var data = _mapper.Map<CategoryDto>(c);
                return ResultModel<CategoryDto>.Success(data);
            }
            catch (Exception ex) { return ResultModel<CategoryDto>.Exception(ex); }
        }

        public async Task<ResultModel<int>> CreateAsync(SaveCategoryRequestDto dto)
        {
            try
            {
                if (dto.ParentCategoryId.HasValue && dto.ParentCategoryId <= 0) dto.ParentCategoryId = null;

                string generatedSlug = SlugHelper.GenerateSlug(dto.CategoryName);
                if (await _repo.IsSlugExistsAsync(generatedSlug))
                    generatedSlug += $"-{Guid.NewGuid().ToString().Substring(0, 5)}";

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

                if (dto.ParentCategoryId == id)
                    return ResultModel<bool>.Error("Danh mục không thể nhận chính nó làm cha!", 400);

                string newSlug = SlugHelper.GenerateSlug(dto.CategoryName);
                if (await _repo.IsSlugExistsAsync(newSlug, id))
                    newSlug += $"-{Guid.NewGuid().ToString().Substring(0, 5)}";

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
                if (category == null) return ResultModel<bool>.Error("Không tìm thấy", 404);

                category.IsActive = !category.IsActive;
                category.UpdatedAt = DateTime.UtcNow;
                await _repo.UpdateAsync(category);

                return ResultModel<bool>.Success(true, category.IsActive ? "Mở khóa thành công" : "Đã khóa");
            }
            catch (Exception ex) { return ResultModel<bool>.Exception(ex); }
        }

        public async Task<ResultModel<bool>> SoftDeleteAsync(int id)
        {
            try
            {
                var cat = await _repo.GetByIdAsync(id);
                if (cat == null) return ResultModel<bool>.Error("Không tìm thấy", 404);

                if (await _repo.HasChildrenAsync(id))
                    return ResultModel<bool>.Error("Đang chứa danh mục con, không thể khóa!", 400);

                cat.IsActive = !cat.IsActive;
                cat.UpdatedAt = DateTime.UtcNow;
                await _repo.UpdateAsync(cat);

                return ResultModel<bool>.Success(true, "Đổi trạng thái thành công!");
            }
            catch (Exception ex) { return ResultModel<bool>.Exception(ex); }
        }
    }
}
