
using GymManagement.DAL.Entities.Enums;

namespace GymManagement.BLL.DTOs.Auth
{
    public class RegisterRequestDto
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public UserRole Role { get; set; } = UserRole.Receptionist;
    }
}