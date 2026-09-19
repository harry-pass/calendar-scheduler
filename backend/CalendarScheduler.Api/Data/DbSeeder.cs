using CalendarScheduler.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CalendarScheduler.Api.Data;

public static class DbSeeder
{
    public const string DefaultAdminEmail = "admin@local.test";
    public const string DefaultAdminPassword = "Admin123!";

    // Dev-only convenience: applies pending migrations and ensures a known
    // admin account exists, so you can log in without registering first.
    public static async Task SeedAsync(IServiceProvider services)
    {
        var db = services.GetRequiredService<AppDbContext>();
        await db.Database.MigrateAsync();

        var configuration = services.GetRequiredService<IConfiguration>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

        var email = configuration["SeedAdmin:Email"] ?? DefaultAdminEmail;
        var password = configuration["SeedAdmin:Password"] ?? DefaultAdminPassword;

        if (await userManager.FindByEmailAsync(email) is not null) return;

        var user = new ApplicationUser { UserName = email, Email = email, EmailConfirmed = true };
        var result = await userManager.CreateAsync(user, password);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Failed to seed admin user: {errors}");
        }
    }
}
