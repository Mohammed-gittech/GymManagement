
using GymManagement.DAL.Repositories.Interfaces;

namespace GymManagement.DAL.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        // Repositories
        IUserRepository Users { get; }
        IRefreshTokenRepository RefreshTokens { get; }
        ISubscriptionPlanRepository SubscriptionPlans { get; }
        IMemberRepository Members { get; }

        // Save changes
        Task<int> SaveChangesAsync();

        // Transaction management
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}