
using GymManagement.DAL.Data;
using GymManagement.DAL.Entities;
using GymManagement.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GymManagement.DAL.Repositories
{
    public class SubscriptionRepository : GenericRepository<Subscription>, ISubscriptionRepository
    {
        public SubscriptionRepository(AppDbContext context) : base(context) { }
        public async Task<IEnumerable<Subscription>> GetByMemberIdAsync(int memberId)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(s => s.MemberId == memberId)
                .Include(s => s.Member)
                .Include(s => s.SubscriptionPlan)
                .ToListAsync();
        }
        public async Task<Subscription?> GetActiveByMemberIdAsync(int memberId)
        {
            return await _dbSet
                .FirstOrDefaultAsync(s => s.MemberId == memberId && s.EndDate >= DateTime.UtcNow);
        }
    }
}