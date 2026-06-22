var builder = WebApplication.CreateBuilder(args);

var apiBaseUrl = builder.Configuration.GetSection("ApiSettings:BaseUrl").Value;
builder.Services.AddHttpClient("ShopCoApi", client =>
{
    client.BaseAddress = new Uri(apiBaseUrl!);
});

builder.Services.AddHttpContextAccessor();

builder.Services.AddSession(option =>
{
    option.IdleTimeout = TimeSpan.FromHours(2);
    option.Cookie.HttpOnly = true;
    option.Cookie.IsEssential = true;
});


// 3. Đăng ký HttpClient để kết nối tới Web API
builder.Services.AddHttpClient("ShopCoApi", client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});



// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSession();

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
