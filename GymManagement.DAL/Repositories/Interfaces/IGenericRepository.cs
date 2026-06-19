
using GymManagement.DAL.Entities.Common;
using GymManagement.DAL.Helpers;

namespace GymManagement.DAL.Repositories.Interfaces
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
        Task<T?> GetByIdAsync(int id);
        Task<T?> GetByIdTrackedAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<PagedResponse<T>> GetPagedAsync(PaginationParams paginationParams);

        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);
    }
}