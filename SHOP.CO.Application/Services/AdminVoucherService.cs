
namespace SHOP.CO.Application.Services
{
    public interface IAdminVoucherService
    {
        IQueryable<VoucherDto> GetVouchersODataQuery();
        Task<ResultModel<VoucherDto>> GetVoucherByIdAsync(int id);
        Task<ResultModel<int>> CreateVoucherAsync(SaveVoucherDto dto);
        Task<ResultModel<bool>> UpdateVoucherAsync(int id, SaveVoucherDto dto);
        Task<ResultModel<bool>> ToggleVoucherStatusAsync(int id);
    }

    public class AdminVoucherService : IAdminVoucherService
    {
        private readonly SHOP.CO.Application.Repositories.ICommerceRecordRepository _repo;
        private readonly IMapper _mapper;
        private readonly ShopCoDbContext _context;
        private readonly Microsoft.AspNetCore.Http.IHttpContextAccessor _http;

        public AdminVoucherService(SHOP.CO.Application.Repositories.ICommerceRecordRepository repo, IMapper mapper, ShopCoDbContext context, Microsoft.AspNetCore.Http.IHttpContextAccessor http)
        {
            _repo = repo;
            _mapper = mapper;
            _context = context;
            _http = http;
        }

        public IQueryable<VoucherDto> GetVouchersODataQuery()
        {
            return _repo.GetVouchersAsQueryable()
                        .ProjectTo<VoucherDto>(_mapper.ConfigurationProvider);
        }

        public async Task<ResultModel<VoucherDto>> GetVoucherByIdAsync(int id)
        {
            try
            {
                var voucher = await _repo.GetByIdAsync(id);
                if (voucher == null || voucher.RecordType != "Voucher")
                    return ResultModel<VoucherDto>.Error("Không tìm thấy Voucher", 404);

                var data = _mapper.Map<VoucherDto>(voucher);
                return ResultModel<VoucherDto>.Success(data);
            }
            catch (Exception ex) { return ResultModel<VoucherDto>.Exception(ex); }
        }

        public async Task<ResultModel<int>> CreateVoucherAsync(SaveVoucherDto dto)
        {
            try
            {
                // Kiểm tra trùng mã Code
                if (await _repo.IsVoucherCodeExistsAsync(dto.Code))
                    return ResultModel<int>.Error($"Mã '{dto.Code}' đã tồn tại!", 400);

                var newVoucher = new CommerceRecord
                {
                    RecordType = "Voucher", // BẮT BUỘC ĐỂ PHÂN BIỆT
                    Code = dto.Code.ToUpper(),
                    Name = dto.Name,
                    DiscountType = dto.DiscountType,
                    DiscountValue = dto.DiscountValue,
                    MaxDiscountAmount = dto.MaxDiscountAmount,
                    MinOrderAmount = dto.MinOrderAmount,
                    UsageLimit = dto.UsageLimit,
                    UsedCount = 0, // Vừa tạo thì số lượt dùng = 0
                    StartAt = dto.StartAt,
                    EndAt = dto.EndAt,
                    Status = dto.IsActive ? "Active" : "Disabled",
                    CreatedAt = DateTime.UtcNow
                };

                await _repo.AddAsync(newVoucher);
                
                // Ghi log thêm Voucher
                var idClaim = _http.HttpContext?.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                int? userId = int.TryParse(idClaim, out int parsedUserId) ? parsedUserId : null;
                
                var log = new InteractionLog
                {
                    UserId = userId,
                    LogType = "Audit",
                    ActionName = "CreateVoucher",
                    Title = "Thêm mới Voucher",
                    Message = $"Tạo mới mã giảm giá: {newVoucher.Code}",
                    ReferenceType = "Voucher",
                    ReferenceId = newVoucher.RecordId,
                    CreatedAt = DateTime.UtcNow,
                    Status = "Success"
                };
                _context.InteractionLogs.Add(log);
                await _context.SaveChangesAsync();

                return ResultModel<int>.Success(newVoucher.RecordId, "Tạo Voucher thành công!", 201);
            }
            catch (Exception ex) { return ResultModel<int>.Exception(ex); }
        }

        public async Task<ResultModel<bool>> UpdateVoucherAsync(int id, SaveVoucherDto dto)
        {
            try
            {
                var voucher = await _repo.GetByIdAsync(id);
                if (voucher == null || voucher.RecordType != "Voucher")
                    return ResultModel<bool>.Error("Không tìm thấy Voucher", 404);

                if (await _repo.IsVoucherCodeExistsAsync(dto.Code, id))
                    return ResultModel<bool>.Error($"Mã '{dto.Code}' đã được sử dụng ở Voucher khác!", 400);

                voucher.Code = dto.Code.ToUpper();
                voucher.Name = dto.Name;
                voucher.DiscountType = dto.DiscountType;
                voucher.DiscountValue = dto.DiscountValue;
                voucher.MaxDiscountAmount = dto.MaxDiscountAmount;
                voucher.MinOrderAmount = dto.MinOrderAmount;
                voucher.UsageLimit = dto.UsageLimit;
                voucher.StartAt = dto.StartAt;
                voucher.EndAt = dto.EndAt;
                voucher.Status = dto.IsActive ? "Active" : "Disabled";
                voucher.UpdatedAt = DateTime.UtcNow;

                await _repo.UpdateAsync(voucher);

                // Ghi log cập nhật Voucher
                var idClaim = _http.HttpContext?.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                int? userId = int.TryParse(idClaim, out int parsedUserId) ? parsedUserId : null;
                
                var log = new InteractionLog
                {
                    UserId = userId,
                    LogType = "Audit",
                    ActionName = "UpdateVoucher",
                    Title = "Cập nhật Voucher",
                    Message = $"Cập nhật thông tin mã giảm giá: {voucher.Code}",
                    ReferenceType = "Voucher",
                    ReferenceId = voucher.RecordId,
                    CreatedAt = DateTime.UtcNow,
                    Status = "Success"
                };
                _context.InteractionLogs.Add(log);
                await _context.SaveChangesAsync();

                return ResultModel<bool>.Success(true, "Cập nhật Voucher thành công!");
            }
            catch (Exception ex) { return ResultModel<bool>.Exception(ex); }
        }

        public async Task<ResultModel<bool>> ToggleVoucherStatusAsync(int id)
        {
            try
            {
                var voucher = await _repo.GetByIdAsync(id);
                if (voucher == null || voucher.RecordType != "Voucher")
                    return ResultModel<bool>.Error("Không tìm thấy Voucher", 404);

                // Khóa hoặc Mở khóa Voucher
                voucher.Status = voucher.Status == "Active" ? "Disabled" : "Active";
                voucher.UpdatedAt = DateTime.UtcNow;

                await _repo.UpdateAsync(voucher);

                // Ghi log cập nhật trạng thái Voucher
                var idClaim = _http.HttpContext?.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                int? userId = int.TryParse(idClaim, out int parsedUserId) ? parsedUserId : null;
                
                var actionStatus = voucher.Status == "Active" ? "Mở khóa" : "Khóa";
                var log = new InteractionLog
                {
                    UserId = userId,
                    LogType = "Audit",
                    ActionName = "ToggleVoucherStatus",
                    Title = $"{actionStatus} Voucher",
                    Message = $"Đã {actionStatus.ToLower()} mã giảm giá: {voucher.Code}",
                    ReferenceType = "Voucher",
                    ReferenceId = voucher.RecordId,
                    CreatedAt = DateTime.UtcNow,
                    Status = "Success"
                };
                _context.InteractionLogs.Add(log);
                await _context.SaveChangesAsync();

                return ResultModel<bool>.Success(true, voucher.Status == "Active" ? "Đã mở khóa Voucher" : "Đã khóa Voucher");
            }
            catch (Exception ex) { return ResultModel<bool>.Exception(ex); }
        }
    }
}