
using GymManagement.DAL.Entities.Common;
using GymManagement.DAL.Entities.Enums;

namespace GymManagement.DAL.Entities
{
    public class User : BaseEntity
    {
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public UserRole Role { get; set; } = UserRole.Receptionist;
    }
}