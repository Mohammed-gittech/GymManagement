
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace GymManagement.DAL.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        // Get current user id
        public int? GetUserId()
        {
            var value = _httpContextAccessor.HttpContext?
                .User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return value == null ? null : int.Parse(value);
        }

        // Get current username
        public string? GetUsername()
        {
            return _httpContextAccessor.HttpContext?
                .User.FindFirst(ClaimTypes.Name)?.Value;
        }

        // Get current user role
        public string? GetRole()
        {
            return _httpContextAccessor.HttpContext?
                .User.FindFirst(ClaimTypes.Role)?.Value;
        }


    }
}