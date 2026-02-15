using HospitalManagement.Application;
using HospitalManagement.Domain.Entities;
using HospitalManagement.Infrastructure;
using HospitalManagement.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

using (IServiceScope scope = app.Services.CreateScope())
{
    RoleManager<IdentityRole> roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    UserManager<AppUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();

    string[] roles = { "Admin", "Doctor", "Patient" };

    foreach (string role in roles)
    {
        bool roleExists = await roleManager.RoleExistsAsync(role);
        if (!roleExists)
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }
}

app.Run();