using Microsoft.EntityFrameworkCore;
using devDynast.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddSession(); // Add session support

// Configure Entity Framework with PostgreSQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure authentication using cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
    });

// Configure authorization policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // Enable authentication
app.UseAuthorization();  // Enable authorization

app.UseSession(); // Enable session management

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "orderHistory",
    pattern: "OrderHistory",
    defaults: new { controller = "OrderHistory", action = "Index" });

app.MapControllerRoute(
    name: "salesReport",
    pattern: "{controller=Sales}/{action=SalesReport}/{id?}");

app.MapControllerRoute(
    name: "salesReport",
    pattern: "{controller=SalesCompare}/{action=SalesCompareReport}/{id?}");

app.MapControllerRoute(
    name: "salesReport",
    pattern: "SalesReport/{action=TopSellingItemsReport}/{id?}",
    defaults: new { controller = "SalesReport" });

app.Run();
