
using GymManagement.DAL.Entities;

namespace GymManagement.DAL.Repositories.Interfaces
{
    public interface IMemberRepository : IGenericRepository<Member>
    {
        Task<Member?> GetByPhoneAsync(string phone);
        Task<Member?> GetByEmailAsync(string email);
    }
}