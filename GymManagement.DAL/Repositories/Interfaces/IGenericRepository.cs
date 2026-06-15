
using GymManagement.DAL.Entities.Common;

namespace GymManagement.DAL.Repositories.Interfaces
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
        Task<T?> GetByIdAsync(int id);
        Task<T?> GetByIdTrackedAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();

        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);
    }
}