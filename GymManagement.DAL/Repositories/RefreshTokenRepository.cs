
using GymManagement.DAL.Data;
using GymManagement.DAL.Entities;
using GymManagement.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GymManagement.DAL.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly AppDbContext _context;

        public RefreshTokenRepository(AppDbContext context)
        {
            _context = context;
        }


        // Get refresh token by token value
        public async Task<RefreshToken?> GetByTokenAsync(string token)
        {
            return await _context.RefreshTokens.FirstOrDefaultAsync(r => r.Token == token);
        }

        // Add new refresh token
        public void Add(RefreshToken refreshToken)
        {
            _context.RefreshTokens.Add(refreshToken);
        }

        // Update refresh token
        public void Update(RefreshToken refreshToken)
        {
            _context.RefreshTokens.Update(refreshToken);
        }
    }
}