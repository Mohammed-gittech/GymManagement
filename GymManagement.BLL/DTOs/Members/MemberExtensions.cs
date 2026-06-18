
using GymManagement.DAL.Entities;

namespace GymManagement.BLL.DTOs.Members
{
    public static class MemberExtensions
    {
        public static MemberResponseDto ToDto(this Member member)
        {
            return new MemberResponseDto
            {
                Id = member.Id,
                FullName = member.FullName,
                Phone = member.Phone,
                Email = member.Email
            };
        }
    }
}