global using SHOP.CO.Application.Services;
global using SHOP.CO.Application.DTOs;
global using SHOP.CO.Infrastructure.Repositories;
global using SHOP.CO.Domain.Entities;

using Microsoft.Extensions.DependencyInjection;

namespace SHOP.CO.Application
{
    public static class ApplicationDI
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // Register Application Services with Scoped lifetime
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ICartService, CartService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IProductUiService, ProductUiService>();
            services.AddScoped<ICouponService, CouponService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IInvoiceService, InvoiceService>();

            //services.AddValidatorsFromAssemblyContaining<CreateProductValidator>();
            return services;
        }
    }
}
