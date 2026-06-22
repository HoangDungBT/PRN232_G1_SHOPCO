global using SHOP.CO.Application.DTOs;
global using SHOP.CO.Application.Services;
global using SHOP.CO.Infrastructure.Repositories;
global using SHOP.CO.Domain.Entities;
global using SHOP.CO.Application.Utilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SHOP.CO.Application
{
    public static class ApplicationDI
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            // Cấu hình Settings
            services.Configure<JwtSettings>(configuration.GetSection("Jwt"));
            services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
            
            // khởi tạo IHttpContextAccessor
            services.AddHttpContextAccessor();

            // Register Application Services with Scoped lifetime
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ICartService, CartService>();
            services.AddScoped<IProductUiService, ProductUiService>();
            services.AddScoped<IAuthService, AuthService>();

            services.AddScoped<ITokenGenerator, TokenGenerator>();
            services.AddScoped<IEmailSender, EmailSender>();
            services.AddScoped<IPasswordHasher, PasswordHasher>();

            #region Admin
            services.AddScoped<IAdminService, AdminService>();
            services.AddScoped<IProductAdminService, ProductAdminService>();
            services.AddScoped<ICategoryAdminService, CategoryAdminService>();
            #endregion

            //services.AddValidatorsFromAssemblyContaining<CreateProductValidator>();

            //đọc cấu hình Cloudinary từ appsettings.json
            //services.Configure<CloudinarySettings>(options =>
            //    {
            //        configuration.GetSection("Cloudinary").Bind(options);
            //    });
            return services;
        }
    }
}

