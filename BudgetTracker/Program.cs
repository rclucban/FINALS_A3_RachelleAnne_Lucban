using BudgetTracker.Services;
using Microsoft.AspNetCore.Identity;
using BudgetTracker.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// === Configuration ==========================================

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found in appsettings.json.");

// Register the ApplicationDbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));


// === Service Registration (The Switching Logic) =============

// 💡 Gamitin ang IBudgetService interface at piliin ang implementation
if (builder.Environment.IsDevelopment())
{
    // DEVELOPMENT: Use EF Core implementation (database)
    builder.Services.AddScoped<IBudgetService, BudgetService>();
}
else
{
    // PRODUCTION/TESTING: Use In-Memory implementation (for demonstration/quick testing)
    builder.Services.AddSingleton<IBudgetService, InMemoryBudgetService>();
}

// ==========================================================

// Configure Identity with password policy
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
    options.Password.RequireDigit = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 10;
    options.Password.RequiredUniqueChars = 1;

}).AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();


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

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();