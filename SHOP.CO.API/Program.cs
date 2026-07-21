using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using Microsoft.OpenApi.Models;
using SHOP.CO.Application;
using SHOP.CO.Application.DTOs;
using SHOP.CO.Domain.Entities;
using SHOP.CO.Infrastructure;
using System.Text;
using System.Text.Json.Serialization;
using SHOP.CO.API.Middlewares;
using SHOP.CO.Application.Common;

var builder = WebApplication.CreateBuilder(args);

#region 0. Odata Configuration
static IEdmModel GetEdmModel()
{
    var odataBuilder = new ODataConventionModelBuilder();

    odataBuilder.EntitySet<ProductDto>("Products").EntityType.HasKey(p => p.ProductId);

    odataBuilder.EntitySet<ProductDto>("AdminProductsOData").EntityType.HasKey(p => p.ProductId);
    odataBuilder.EntitySet<CategoryDto>("AdminCategoriesOData").EntityType.HasKey(c => c.CategoryId);
    odataBuilder.EntitySet<UserDto>("AdminUsersOData").EntityType.HasKey(u => u.UserId);

    odataBuilder.EntitySet<OrderDto>("AdminOrdersOData").EntityType.HasKey(u => u.OrderId);
    odataBuilder.EntitySet<ProductVariant>("AdminInventoryOData").EntityType.HasKey(v => v.VariantId);
    odataBuilder.EntitySet<InteractionLogDto>("AdminLogsOData").EntityType.HasKey(l => l.LogId);

    odataBuilder.EntitySet<VoucherDto>("AdminVouchersOData").EntityType.HasKey(v => v.RecordId);
    odataBuilder.EntitySet<NotificationAdminDto>("AdminNotificationsOData").EntityType.HasKey(n => n.LogId);
    return odataBuilder.GetEdmModel();
}
#endregion

#region 1. Controllers & JSON, Odata Options
builder.Services.AddControllers()
    .AddJsonOptions(option =>
    {
        option.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        option.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.Never;
    }).AddOData(options => options
        .Select()       // Cho phép $select
        .Filter()       // Cho phép $filter
        .OrderBy()      // Cho phép $orderby
        .SetMaxTop(100) // Giới hạn tối đa lấy 100 record/lần để chống DDoS
        .SkipToken()
        .Expand()       // Cho phép $expand (join bảng)
        .Count()        // Cho phép đếm tổng số $count
        .AddRouteComponents("odata", GetEdmModel()) // Prefix route là /odata
    ); ;
#endregion

#region 1.1 Model State Validation
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        // Rút trích tất cả thông báo lỗi từ các thuộc tính bị sai
        var errors = context.ModelState
            .Where(e => e.Value.Errors.Count > 0)
            .SelectMany(x => x.Value.Errors)
            .Select(x => x.ErrorMessage)
            .ToList();

        // Gộp tất cả các lỗi lại thành 1 chuỗi, cách nhau bởi thẻ <br> để hiển thị trên web
        string errorMessage = string.Join("<br/>", errors);

        // Trả về theo cấu trúc chuẩn của ResultModel
        var result = ResultModel<string>.Error(errorMessage, 400);

        return new BadRequestObjectResult(result);
    };
});
#endregion  

#region 2. Application & Infrastructure DI
builder.Services.AddApplication(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);
#endregion

#region 3. JWT Authentication & Authorization
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };

    // Hỗ trợ JWT cho SignalR và ghi log xác thực
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine($"[JWT Auth Failure] Token validation failed: {context.Exception.Message}");
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            Console.WriteLine("[JWT Auth Success] Token validated successfully.");
            return Task.CompletedTask;
        },
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;

            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/notificationHub"))
            {
                context.Token = accessToken;
            }
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();
#endregion

#region 4. CORS Policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazor", policy =>
    {
        policy.SetIsOriginAllowed(origin => true)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});
#endregion

#region 5. Swagger Configuration
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Nhập: Bearer {your JWT token}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});
#endregion

#region 6. Third-Party Licenses (EPPlus)
// Kích hoạt License Non-Commercial theo chuẩn EPPlus 8+
OfficeOpenXml.ExcelPackage.License.SetNonCommercialOrganization("ShopCo");
#endregion

#region 7. Logging & SignalR
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Services.AddSignalR();
#endregion

var app = builder.Build();

#region 8. Middleware Pipeline

app.UseMiddleware<GlobalExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();

app.UseStaticFiles();

// Kích hoạt CORS (Phải đặt trước Auth)
app.UseCors("AllowBlazor");

// Phân quyền
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Tự động đồng bộ số sao và lượt đánh giá thực tế từ Database
using (var scope = app.Services.CreateScope())
{
    try
    {
        var productRepo = scope.ServiceProvider.GetRequiredService<SHOP.CO.Application.Repositories.IProductRepository>();
        await productRepo.SyncAllProductRatingsAsync();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error syncing product ratings on startup: {ex.Message}");
    }
}

app.Run();
#endregion