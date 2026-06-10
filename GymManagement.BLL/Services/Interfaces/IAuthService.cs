
using GymManagement.BLL.DTOs.Auth;

namespace GymManagement.BLL.Services.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponseDto> Login(LoginRequestDto dto);
        Task<LoginResponseDto> Register(RegisterRequestDto dto);
        Task<LoginResponseDto> Refresh(RefreshTokenRequestDto dto);
        Task Revoke(RevokeTokenRequestDto dto);
    }
}