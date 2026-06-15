
using GymManagement.DAL.Entities;

namespace GymManagement.DAL.Repositories.Interfaces
{
    public interface ISubscriptionPlanRepository : IGenericRepository<SubscriptionPlan>
    {
        Task<SubscriptionPlan?> GetByNameAsync(string name);
    }
}