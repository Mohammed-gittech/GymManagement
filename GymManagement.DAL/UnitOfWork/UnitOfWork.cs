
using GymManagement.DAL.Data;
using GymManagement.DAL.Repositories;
using GymManagement.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

namespace GymManagement.DAL.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private IDbContextTransaction? _transaction;

        // Repositories
        private IUserRepository? _users;
        private IRefreshTokenRepository? _refreshToken;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        // Repositories
        public IUserRepository Users =>
            _users ??= new UserRepository(_context);

        public IRefreshTokenRepository RefreshTokens =>
            _refreshToken ??= new RefreshTokenRepository(_context);

        // Save changes
        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        // Begin transaction
        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        // Commit transaction
        public async Task CommitTransactionAsync()
        {
            await _transaction!.CommitAsync();
        }

        // Rollback transaction
        public async Task RollbackTransactionAsync()
        {
            await _transaction!.RollbackAsync();
        }

        // Dispose
        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }
}