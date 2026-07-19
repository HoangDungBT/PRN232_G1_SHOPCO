using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SHOP.CO.Application.Services
{
    public interface IAdminLogService
    {
        IQueryable<InteractionLogDto> GetLogsODataQuery();
    }

    public class AdminLogService : IAdminLogService
    {
        // Vì Log chỉ đọc để báo cáo, ta dùng thẳng DbContext cho lẹ, không cần tạo Repository rườm rà
        private readonly ShopCoDbContext _context;
        private readonly IMapper _mapper;

        public AdminLogService(ShopCoDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public IQueryable<InteractionLogDto> GetLogsODataQuery()
        {
            return _context.InteractionLogs
                           .ProjectTo<InteractionLogDto>(_mapper.ConfigurationProvider);
        }
    }
}
