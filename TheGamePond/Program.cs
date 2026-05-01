using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TheGamePond.Data;
using TheGamePond.Models;
using TheGamePond.Services.Cart;
using TheGamePond.Services.Payments;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' was not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services
    .AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.User.RequireUniqueEmail = true;
        options.SignIn.RequireConfirmedAccount = false;
        options.Password.RequiredLength = 8;
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = false;
        options.Password.RequireNonAlphanumeric = false;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

builder.Services.AddControllersWithViews();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICartSessionService, CartSessionService>();
builder.Services.AddScoped<IPaymentGateway, LocalTestPaymentGateway>();
builder.Services.AddScoped<IOrderPaymentService, OrderPaymentService>();
builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.IdleTimeout = TimeSpan.FromHours(2);
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var seedLogger = scope.ServiceProvider
        .GetRequiredService<ILoggerFactory>()
        .CreateLogger("IdentitySeedData");

    await IdentitySeedData.SeedAsync(scope.ServiceProvider, app.Configuration, seedLogger);

    if (app.Configuration.GetValue<bool>("SeedMockInventory:Delete"))
    {
        var mockInventoryLogger = scope.ServiceProvider
            .GetRequiredService<ILoggerFactory>()
            .CreateLogger("MockInventorySeedData");

        await MockInventorySeedData.DeleteAsync(scope.ServiceProvider, mockInventoryLogger);
    }
    else if (app.Configuration.GetValue<bool>("SeedMockInventory:Enabled"))
    {
        var mockInventoryLogger = scope.ServiceProvider
            .GetRequiredService<ILoggerFactory>()
            .CreateLogger("MockInventorySeedData");

        await MockInventorySeedData.SeedAsync(scope.ServiceProvider, mockInventoryLogger);
    }
}

if (app.Configuration.GetValue<bool>("SeedMockInventory:ExitAfterSeed"))
{
    return;
}

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

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
