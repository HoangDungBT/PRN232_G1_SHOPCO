global using SHOP.CO.Application.Services;
global using SHOP.CO.Application.DTOs;
global using SHOP.CO.Infrastructure.Repositories;

using Microsoft.Extensions.DependencyInjection;

namespace SHOP.CO.Application
{
    public static class ApplicationDI
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IProductService, ProductService>();
            //services.AddValidatorsFromAssemblyContaining<CreateProductValidator>();
            return services;
        }
    }
}
