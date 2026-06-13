using GymManagement.API.Helpers;
using GymManagement.BLL.DTOs.Auth;
using GymManagement.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace GymManagement.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        [EnableRateLimiting("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        public async Task<IActionResult> Login(LoginRequestDto dto)
        {
            var result = await _authService.Login(dto);
            return Ok(ApiResponse<LoginResponseDto>.Ok(result, "تم تسجيل الدخول بنجاح"));
        }

        [HttpPost("refresh")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Refresh(RefreshTokenRequestDto dto)
        {
            var result = await _authService.Refresh(dto);
            return Ok(ApiResponse<LoginResponseDto>.Ok(result));
        }

        [HttpPost("register")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register(RegisterRequestDto dto)
        {
            var result = await _authService.Register(dto);
            return Ok(ApiResponse<LoginResponseDto>.Ok(result, "تم إنشاء المستخدم بنجاح"));
        }

        [HttpPost("revoke")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Revoke(RevokeTokenRequestDto dto)
        {
            await _authService.Revoke(dto);
            return Ok(ApiResponse<object>.Ok("تم تسجيل الخروج بنجاح"));
        }
    }
}