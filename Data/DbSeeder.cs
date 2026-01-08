using expensemanager.Models;
using Microsoft.EntityFrameworkCore;

namespace expensemanager.Data
{
    /// <summary>
    /// Seeds initial demo data for development and academic demonstration.
    /// </summary>
    public static class DbSeeder
    {
        /// <summary>
        /// Applies migrations and seeds users and categories if database is empty.
        /// </summary>
        public static async Task SeedAsync(ApplicationDbContext db)
        {
            await db.Database.MigrateAsync();

            if (!await db.Users.AnyAsync())
            {
                db.Users.Add(new User
                {
                    Name = "Administrator",
                    Email = "admin@example.com",
                    PasswordHash = expensemanager.Security.PasswordHasher.Hash("Admin@123"),
                    Role = "Admin"
                });
            }

            if (!await db.Categories.AnyAsync())
            {
                db.Categories.AddRange(
                    new Category { Title = "Salary", Icon = "bi-cash-stack", Type = "Income" },
                    new Category { Title = "Freelance", Icon = "bi-briefcase", Type = "Income" },
                    new Category { Title = "Food", Icon = "bi-egg-fried", Type = "Expense" },
                    new Category { Title = "Transport", Icon = "bi-car-front", Type = "Expense" }
                );
            }

            await db.SaveChangesAsync();
        }
    }
}
