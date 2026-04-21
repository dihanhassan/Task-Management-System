using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace TaskManagementSystem.Infrastructure.Seed;

public static class DbSeeder
{
    public static async Task SeedAsync(IServiceProvider services, IConfiguration configuration)
    {
        using var scope = services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
        var loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger(nameof(DbSeeder));

        var email = configuration["Seed:AdminEmail"] ?? "admin@taskmanager.com";
        var password = configuration["Seed:AdminPassword"] ?? "Admin@123";
        var userName = configuration["Seed:AdminUserName"] ?? "admin";

        var existing = await userManager.FindByEmailAsync(email);
        if (existing is not null) return;

        var user = CreateAdminUser(userName, email);
        var result = await userManager.CreateAsync(user, password);

        LogSeedResult(logger, result, email);
    }

    private static IdentityUser CreateAdminUser(string userName, string email)
    {
        return new IdentityUser
        {
            UserName = userName,
            Email = email,
            EmailConfirmed = true
        };
    }

    private static void LogSeedResult(ILogger logger, IdentityResult result, string email)
    {
        if (result.Succeeded)
        {
            logger.LogInformation("Admin user seeded. Email={Email}", email);
            return;
        }

        var errors = string.Join(", ", result.Errors.Select(e => e.Description));
        logger.LogError("Admin seed failed. Errors={Errors}", errors);
    }
}
