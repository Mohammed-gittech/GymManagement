
using GymManagement.DAL.Entities;

namespace GymManagement.DAL.Repositories.Interfaces
{
    public interface ISubscriptionRepository : IGenericRepository<Subscription>
    {
        Task<IEnumerable<Subscription>> GetByMemberIdAsync(int memberId);

        Task<Subscription?> GetActiveByMemberIdAsync(int memberId);
    }
}