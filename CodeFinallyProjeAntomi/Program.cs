using CodeFinallyProjeAntomi.DataAccesLayer;
using CodeFinallyProjeAntomi.Interfaces;
using CodeFinallyProjeAntomi.Models;
using CodeFinallyProjeAntomi.Services;
using CodeFinallyProjeAntomi.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));
builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
    options.User.RequireUniqueEmail = true;

    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;

    options.Lockout.AllowedForNewUsers= true;
    options.Lockout.MaxFailedAccessAttempts = 8;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(2);
}).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();

builder.Services.Configure<SmtpSetting>(builder.Configuration.GetSection("SmptpSetting"));

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromSeconds(2);
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IlayoutService,LayoutService>();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles();
app.UseSession();

app.MapControllerRoute("area", "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");
app.MapControllerRoute("Default","{controller=Home}/{action=Index}/{id?}");


app.Run();
