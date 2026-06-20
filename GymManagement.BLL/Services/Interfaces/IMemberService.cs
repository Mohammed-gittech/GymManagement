
using GymManagement.BLL.DTOs.Members;
using GymManagement.DAL.Entities;
using GymManagement.DAL.Helpers;

namespace GymManagement.BLL.Services.Interfaces
{
    public interface IMemberService
    {
        Task<PagedResponse<MemberResponseDto>> GetAllAsync(PaginationParams paginationParams);
        Task<MemberResponseDto> GetByIdAsync(int id);
        Task<MemberResponseDto> CreateAsync(CreateMemberDto dto);
        Task<MemberResponseDto> UpdateAsync(UpdateMemberDto dto, int id);
        Task DeleteAsync(int id);
    }
}