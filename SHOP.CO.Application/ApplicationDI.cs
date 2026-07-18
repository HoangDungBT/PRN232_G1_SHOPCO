global using SHOP.CO.Application.DTOs;
global using SHOP.CO.Application.Services;
global using SHOP.CO.Application.Utilities;
global using SHOP.CO.Domain.Entities;
global using AutoMapper;
global using AutoMapper.QueryableExtensions;
global using SHOP.CO.Application.Common;
global using SHOP.CO.Application.Repositories;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.AspNetCore.Hosting;
global using Microsoft.EntityFrameworkCore;
global using SHOP.CO.Infrastructure.Persistence;
global using SHOP.CO.Infrastructure.Repositories;
global using System.Reflection;
using Microsoft.AspNetCore.Identity;

namespace SHOP.CO.Application
{
    public static class ApplicationDI
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            // 🟢 1. ĐĂNG KÝ AUTOMAPPER (Tự động quét file MappingProfile)
            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            // 2. Cấu hình Settings
            services.Configure<JwtSettings>(configuration.GetSection("Jwt"));
            services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));

            services.AddHttpContextAccessor();
            // Đăng ký Services
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ICartService, CartService>();
            services.AddScoped<IProductUiService, ProductUiService>();

            // 3. Đăng ký Utilities & Auth
            services.AddScoped<ITokenGenerator, TokenGenerator>();
            services.AddScoped<IEmailSender, EmailSender>();
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<IAuthService, AuthService>();

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IContactService, ContactService>();
            services.AddScoped<IPasswordHasher<SHOP.CO.Domain.Entities.User>, PasswordHasher<SHOP.CO.Domain.Entities.User>>();

            // 5. Đăng ký Admin Services
            #region Admin Services
            services.AddScoped<IAdminDashboardService, AdminDashboardService>();
            services.AddScoped<IAdminProductService, AdminProductService>();
            services.AddScoped<IAdminCategoryService, AdminCategoryService>();
            services.AddScoped<IAdminUserService, AdminUserService>();
            services.AddScoped<IAdminOrderService, AdminOrderService>();
            services.AddScoped<IAdminLogService, AdminLogService>();
            #endregion
            return services;
        }
    }
}

