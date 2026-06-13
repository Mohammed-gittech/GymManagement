
using GymManagement.DAL.Entities;
using GymManagement.DAL.Entities.Enums;

namespace GymManagement.DAL.Data.Seeding
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            // Seed default admin user
            if (!context.Users.Any())
            {
                var admin = new User
                {
                    Username = "admin",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                    Role = UserRole.Admin,
                    CreatedBy = 0,
                };
                context.Users.Add(admin);
                await context.SaveChangesAsync();
            }
        }
    }
}