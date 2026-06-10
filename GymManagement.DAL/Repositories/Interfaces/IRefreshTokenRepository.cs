
using GymManagement.DAL.Entities;

namespace GymManagement.DAL.Repositories.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken?> GetByTokenAsync(string token);
        void Add(RefreshToken refreshToken);
        void Update(RefreshToken refreshToken);
    }
}