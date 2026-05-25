global using SHOP.CO.Domain.Entities;
global using SHOP.CO.Infrastructure.Data;
global using SHOP.CO.Infrastructure.Repositories;
global using Microsoft.EntityFrameworkCore;
global using SHOP.CO.Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
                    // Đặt tên Migration Assembly chỉ định về tầng API (nếu bạn muốn chạy lệnh migration ở API)
                    // Hoặc để trống nếu bạn chạy migration trực tiếp trên Infrastructure
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

            //đọc cấu hình Cloudinary từ appsettings.json
            //services.Configure<CloudinarySettings>(options =>
            //    {
            //        configuration.GetSection("Cloudinary").Bind(options);
            //    });
            return services;
        }
    }
}
