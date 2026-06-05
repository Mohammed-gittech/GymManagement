
using GymManagement.DAL.Data;
using GymManagement.DAL.Entities.Common;
using GymManagement.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GymManagement.DAL.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        private readonly AppDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        // Get entity by id
        public async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id);
        }

        // Get all entities
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .ToListAsync();
        }

        // Add new entity
        public void Add(T entity)
        {
            _dbSet.Add(entity);
        }

        // Soft delete entity
        public void Delete(T entity)
        {
            entity.IsDeleted = true;
            entity.DeletedAt = DateTime.UtcNow;
            _dbSet.Update(entity);
        }

        // Update entity
        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }
    }
}