
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

            // Seed default subscription plans
            if (!context.SubscriptionPlans.Any())
            {
                var plans = new List<SubscriptionPlan>

                {
                    new SubscriptionPlan
                    {
                        Name = "شهري",
                        Price = 50,
                        DurationDays = 30,
                        CreatedBy = 0
                    },
                    new SubscriptionPlan
                    {
                        Name = "ربعي",
                        Price = 140,
                        DurationDays = 90,
                        CreatedBy = 0
                    },
                    new SubscriptionPlan
                    {
                        Name = "سنوي",
                        Price = 500,
                        DurationDays = 365,
                        CreatedBy = 0
                    }
                };

                context.SubscriptionPlans.AddRange(plans);
                await context.SaveChangesAsync();
            }
        }
    }
}