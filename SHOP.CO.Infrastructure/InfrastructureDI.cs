global using Microsoft.EntityFrameworkCore;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using SHOP.CO.Domain.Entities;
global using SHOP.CO.Infrastructure.Data; 
global using SHOP.CO.Infrastructure.Persistence;
global using SHOP.CO.Infrastructure.Repositories;
global using SHOP.CO.Application.Repositories;
namespace SHOP.CO.Infrastructure
{
    public static class InfrastructureDI
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<ShopCoDbContext>(options =>
            {
                options.UseSqlServer(connectionString, sqlOptions =>
                {
                    // Đặt tên Migration Assembly chỉ định về tầng Infrastructure
                    sqlOptions.MigrationsAssembly("SHOP.CO.Infrastructure");
                    // Cấu hình chịu lỗi (Resiliency) nếu db mất kết nối tạm thời
                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 3,
                        maxRetryDelay: TimeSpan.FromSeconds(10),
                        errorNumbersToAdd: null);
                });
            });

            // Đăng ký Generic Repository
            services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));

            // Đăng ký Repositories với vòng đời Scoped
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<ICartRepository, CartRepository>();
            services.AddScoped<SHOP.CO.Domain.Repositories.IProductUiRepository, MockProductUiRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<INotificationRepository, NotificationRepository>();
            services.AddScoped<IContactRepository, ContactRepository>();
            services.AddScoped<ICommerceRecordRepository, CommerceRecordRepository>();

            return services;
        }
    }
}
