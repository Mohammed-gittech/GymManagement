
using System.Linq.Expressions;
using GymManagement.DAL.Entities;
using GymManagement.DAL.Entities.Common;
using GymManagement.DAL.Services;
using Microsoft.EntityFrameworkCore;

namespace GymManagement.DAL.Data
{
    public class AppDbContext : DbContext
    {
        private readonly ICurrentUserService _currentUserService;
        public AppDbContext(
            DbContextOptions<AppDbContext> options,
            ICurrentUserService currentUserService) : base(options)
        {
            _currentUserService = currentUserService;
        }

        // DbSets
        public DbSet<User> Users { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var currentUserId = _currentUserService.GetUserId() ?? 0;
            var now = DateTime.UtcNow;

            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                // New entity
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAt = now;
                    entry.Entity.CreatedBy = currentUserId;
                }

                // Modified entity
                if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedAt = now;
                    entry.Entity.UpdatedBy = currentUserId;
                }

                // Soft deleted entity
                if (entry.State == EntityState.Modified
                    && entry.Entity.IsDeleted
                    && entry.Entity.DeletedBy == null)
                {
                    entry.Entity.DeletedAt = now;
                    entry.Entity.DeletedBy = currentUserId;
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply global soft delete filter for all entities
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
                {
                    var parameter = Expression.Parameter(entityType.ClrType, "e");
                    var property = Expression.Property(parameter, "IsDeleted");
                    var condition = Expression.Equal(property, Expression.Constant(false));
                    var lambda = Expression.Lambda(condition, parameter);

                    modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
                }
            }

            // Apply all configurations from assembly
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
}