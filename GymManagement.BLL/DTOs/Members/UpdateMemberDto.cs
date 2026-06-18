
namespace GymManagement.BLL.DTOs.Members
{
    public class UpdateMemberDto
    {
        public string FullName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string? Email { get; set; }
    }
}