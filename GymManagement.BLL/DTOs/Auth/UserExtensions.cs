
using GymManagement.DAL.Entities;

namespace GymManagement.BLL.DTOs.Auth
{
    public static class UserExtensions
    {
        public static LoginResponseDto ToDto(this User user, string token, string refreshToken)
        {
            return new LoginResponseDto
            {
                Id = user.Id,
                Username = user.Username,
                Role = user.Role.ToString(),
                Token = token,
                RefreshToken = refreshToken
            };
        }
    }
}