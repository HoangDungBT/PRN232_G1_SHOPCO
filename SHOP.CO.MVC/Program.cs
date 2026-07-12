using SHOP.CO.MVC.Services;

var builder = WebApplication.CreateBuilder(args);

var apiBaseUrl = builder.Configuration.GetSection("ApiSettings:BaseUrl").Value;

builder.Services.AddHttpContextAccessor();

builder.Services.AddSession(option =>
{
    option.IdleTimeout = TimeSpan.FromHours(2);
    option.Cookie.HttpOnly = true;
    option.Cookie.IsEssential = true;
});

// Configure HttpClient to call SHOP.CO.API
builder.Services.AddHttpClient("ShopCoApi", client =>
{
    client.BaseAddress = new Uri(apiBaseUrl ?? "https://localhost:7196/");
});

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSession();
// Configure HttpClient to call SHOP.CO.API
builder.Services.AddHttpClient("ShopApi", client =>
{
    // Base URL for the API - fallback to localhost API if not configured
    client.BaseAddress = new Uri(builder.Configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7196/");
    client.Timeout = TimeSpan.FromSeconds(30);
});
builder.Services.AddHttpClient<IProductApiClient, ProductApiClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7196/");
    client.Timeout = TimeSpan.FromSeconds(30);
});
builder.Services.AddHttpClient<ICartApiClient, CartApiClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7196/");
    client.Timeout = TimeSpan.FromSeconds(30);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// LƯU Ý: UseSession PHẢI nằm giữa UseRouting và UseAuthorization
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

#region Route
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/ {controller=Dashboard}/{action=Index}/{id?}");

#endregion

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.UseSession();

app.Run();
