
namespace GymManagement.DAL.Data.Seeding
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            await context.SaveChangesAsync();
        }
    }
}