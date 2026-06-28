
using GymManagement.DAL.Data;
using GymManagement.DAL.Entities;
using GymManagement.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GymManagement.DAL.Repositories
{
    public class PaymentRepository : GenericRepository<Payment>, IPaymentRepository
    {
        public PaymentRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<Payment>> GetByMemberIdAsync(int memberId)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(p => p.Subscription.MemberId == memberId)
                .Include(p => p.Subscription)
                .ToListAsync();
        }
    }
}