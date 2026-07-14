using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Core.Interfaces;
using TaskManagementSystem.Infrastructure.Services;
using TaskManagementSystem.Infrastructure.Data;
using TaskManagementSystem.Infrastructure.Repositories;
using TaskManagementSystem.Infrastructure.Seed;
using TaskManagementSystem.Web.Middleware;
using System.Linq;

var builder = WebApplication.CreateBuilder(args);

// Configure port from environment variable (for Render)
var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

builder.Services.AddControllersWithViews();

// Get connection string from environment variable first, then fall back to config
var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
    ?? builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("No database connection string found in environment variables or appsettings.json");
}

// Log connection string (hide password)
var connLog = connectionString.Replace(connectionString.Split("Password=").Last().Split(";").First(), "***");
Console.WriteLine($"Database Connection String: {connLog}");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/Login";
    options.SlidingExpiration = true;
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
});

builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<ITaskService, TaskService>();

var app = builder.Build();

app.UseMiddleware<GlobalExceptionMiddleware>();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}")
    .WithStaticAssets();

// Apply migrations automatically with retry logic
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    int retries = 3;
    while (retries > 0)
    {
        try
        {
            logger.LogInformation("Attempting database migration (attempts remaining: {Retries})", retries);
            await db.Database.MigrateAsync();
            logger.LogInformation("Database migration completed successfully");
            break;
        }
        catch (Exception ex)
        {
            retries--;
            if (retries == 0)
            {
                logger.LogWarning(ex, "Database migration failed after 3 attempts. App will continue but database may not be initialized.");
                // Don't throw - allow app to start anyway
            }
            else
            {
                logger.LogWarning(ex, "Database migration failed, retrying in 5 seconds...");
                await Task.Delay(5000);
            }
        }
    }
}

try
{
    await DbSeeder.SeedAsync(app.Services, app.Configuration);
}
catch (Exception ex)
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogWarning(ex, "Database seeding failed");
}

app.Run();
