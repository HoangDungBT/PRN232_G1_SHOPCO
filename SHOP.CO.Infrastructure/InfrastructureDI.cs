global using SHOP.CO.Infrastructure.Persistence;
global using SHOP.CO.Infrastructure.Repositories;
global using SHOP.CO.Domain.Entities;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using SHOP.CO.Infrastructure.Data; 
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
                    sqlOptions.MigrationsAssembly("SHOP.CO.Infrastructure");

                    // Cấu hình chịu lỗi (Resiliency) nếu db rớt kết nối tạm thời
                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 3,
                        maxRetryDelay: TimeSpan.FromSeconds(10),
                        errorNumbersToAdd: null);
                });
            });


            //đăng kí Repositories
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();


            return services;
        }
    }
}
