global using SHOP.CO.Application.DTOs;
global using SHOP.CO.Application.Services;
global using SHOP.CO.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace SHOP.CO.Application
{
    public static class ApplicationDI
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IContactService, ContactService>();
            services.AddScoped<IPasswordHasher<SHOP.CO.Domain.Entities.User>, PasswordHasher<SHOP.CO.Domain.Entities.User>>();
            //services.AddValidatorsFromAssemblyContaining<CreateProductValidator>();
            return services;
        }
    }
}
