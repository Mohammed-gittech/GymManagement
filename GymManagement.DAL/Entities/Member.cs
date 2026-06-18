
using GymManagement.DAL.Entities.Common;

namespace GymManagement.DAL.Entities
{
    public class Member : BaseEntity
    {
        public string FullName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string? Email { get; set; }
    }
}