using onlineCurses.Domain.Entities;
using onlineCurses.Infrastructure.Data;

namespace onlineCurses.Infrastructure;

public class DataSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        if (!context.Users.Any())
        {
            var admin = new User
            {
                Email = "admin@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!")
            };
            context.Users.Add(admin);
            await context.SaveChangesAsync();
        }
    }
}