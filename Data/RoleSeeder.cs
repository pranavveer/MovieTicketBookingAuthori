using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MovieTicketBooking.Authentication;
using MovieTicketBooking.Models;

namespace MovieTicketBooking.Data;

public static class RoleSeeder
{
    // Creates the initial Admin only when all three seed settings are supplied
    // through User Secrets or environment variables. Public registration is always Customer.
    public static async Task SeedAsync(IServiceProvider services, IConfiguration configuration)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await db.Database.MigrateAsync();

        var adminEmail = configuration["SeedAdmin:Email"];
        var adminUsername = configuration["SeedAdmin:Username"];
        var adminPassword = configuration["SeedAdmin:Password"];

        if (string.IsNullOrWhiteSpace(adminEmail) ||
            string.IsNullOrWhiteSpace(adminUsername) ||
            string.IsNullOrWhiteSpace(adminPassword))
            return;

        var normalizedEmail = adminEmail.Trim().ToLowerInvariant();
        if (await db.Users.AnyAsync(u => u.Email == normalizedEmail))
            return;

        var admin = new User
        {
            Username = adminUsername.Trim(),
            Email = normalizedEmail,
            Phone = "0000000000",
            Role = UserRoles.Admin,
            CreatedAtUtc = DateTime.UtcNow
        };
        admin.PasswordHash = new PasswordHasher<User>().HashPassword(admin, adminPassword);
        db.Users.Add(admin);
        await db.SaveChangesAsync();
    }
}
