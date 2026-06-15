
using GymManagement.DAL.Data;
using GymManagement.DAL.Entities;
using GymManagement.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GymManagement.DAL.Repositories
{
    public class SubscriptionPlanRepository : GenericRepository<SubscriptionPlan>, ISubscriptionPlanRepository
    {
        public SubscriptionPlanRepository(AppDbContext context) : base(context) { }

        // Get subscription plan by name
        public async Task<SubscriptionPlan?> GetByNameAsync(string name)
        {
            return await _dbSet.AsNoTracking()
                .FirstOrDefaultAsync(sp => sp.Name == name);
        }
    }
}