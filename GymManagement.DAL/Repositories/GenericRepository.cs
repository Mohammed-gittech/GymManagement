
using GymManagement.DAL.Data;
using GymManagement.DAL.Entities.Common;
using GymManagement.DAL.Helpers;
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

        // Get entity by id with tracking for updates
        public async Task<T?> GetByIdTrackedAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        // Get all entities
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .ToListAsync();
        }

        // Get paged entities
        public async Task<PagedResponse<T>> GetPagedAsync(PaginationParams paginationParams)
        {
            // Get Total count 
            var totalCount = await _dbSet.CountAsync();

            // Get items for the requested page
            var items = await _dbSet
                .AsNoTracking()
                .OrderBy(e => e.Id)
                .Skip((paginationParams.PageNumber - 1) * paginationParams.PageSize)
                .Take(paginationParams.PageSize)
                .ToListAsync();

            // Build response
            return new PagedResponse<T>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = paginationParams.PageNumber,
                PageSize = paginationParams.PageSize
            };
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
            _dbSet.Update(entity);
        }

        // Update entity
        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }
    }
}