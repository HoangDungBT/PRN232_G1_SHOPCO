global using SHOP.CO.Infrastructure.Persistence;
global using SHOP.CO.Infrastructure.Repositories;
global using SHOP.CO.Domain.Entities;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
<<<<<<< HEAD
global using SHOP.CO.Infrastructure.Data;
=======
global using SHOP.CO.Infrastructure.Data; 

>>>>>>> origin/duy
namespace SHOP.CO.Infrastructure
{
    public static class InfrastructureDI
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            // Đăng ký DbContext với vòng đời Scoped (mặc định)
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

            // Đăng ký Repositories với vòng đời Scoped
            services.AddScoped<IProductRepository, ProductRepository>();
<<<<<<< HEAD
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<ICartRepository, CartRepository>();
            services.AddScoped<SHOP.CO.Domain.Repositories.IProductUiRepository, MockProductUiRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<INotificationRepository, NotificationRepository>();
            services.AddScoped<IContactRepository, ContactRepository>();
=======
            services.AddScoped<ICartRepository, CartRepository>();
            services.AddScoped<SHOP.CO.Domain.Repositories.IProductUiRepository, MockProductUiRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
>>>>>>> origin/duy

            return services;
        }
    }
}
