
using GymManagement.BLL.DTOs.Members;
using GymManagement.BLL.Exceptions;
using GymManagement.BLL.Services.Interfaces;
using GymManagement.DAL.Entities;
using GymManagement.DAL.Helpers;
using GymManagement.DAL.UnitOfWork;

namespace GymManagement.BLL.Services
{
    public class MemberService : IMemberService
    {
        private readonly IUnitOfWork _unitOfWork;

        public MemberService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PagedResponse<MemberResponseDto>> GetAllAsync(PaginationParams paginationParams)
        {
            // Get paged members
            var pagedMembers = await _unitOfWork.Members.GetPagedAsync(paginationParams);

            // Convert to response
            return new PagedResponse<MemberResponseDto>
            {
                Items = pagedMembers.Items.Select(m => m.ToDto()),
                TotalCount = pagedMembers.TotalCount,
                PageNumber = pagedMembers.PageNumber,
                PageSize = pagedMembers.PageSize
            };
        }

        public async Task<MemberResponseDto> GetByIdAsync(int id)
        {
            var member = await _unitOfWork.Members.GetByIdAsync(id);

            if (member == null)
                throw new NotFoundException("المشترك غير موجود");

            return member.ToDto();
        }

        public async Task<MemberResponseDto> CreateAsync(CreateMemberDto dto)
        {
            // Check if phone already exists
            var existingPhone = await _unitOfWork.Members.GetByPhoneAsync(dto.Phone);

            if (existingPhone != null)
                throw new ValidationException("رقم الهاتف موجود مسبقاً");

            // Check if email already exists (only if provided)
            if (!string.IsNullOrEmpty(dto.Email))
            {
                var existingEmail = await _unitOfWork.Members.GetByEmailAsync(dto.Email);
                if (existingEmail != null)
                    throw new ValidationException("البريد الإلكتروني موجود مسبقاً");
            }
            // Create new member
            var member = new Member
            {
                FullName = dto.FullName,
                Phone = dto.Phone,
                Email = dto.Email
            };
            // Save member
            _unitOfWork.Members.Add(member);
            await _unitOfWork.SaveChangesAsync();

            // Return response
            return member.ToDto();
        }

        public async Task<MemberResponseDto> UpdateAsync(UpdateMemberDto dto, int id)
        {
            // Get member by id
            var member = await _unitOfWork.Members.GetByIdTrackedAsync(id);

            if (member == null)
                throw new NotFoundException("المشترك غير موجود");

            // Check if phone belongs to another member
            var existingPhone = await _unitOfWork.Members.GetByPhoneAsync(dto.Phone);
            if (existingPhone != null && existingPhone.Id != id)
                throw new ValidationException("رقم الهاتف موجود مسبقاً");

            // Check if email belongs to another member (only if provided)
            if (!string.IsNullOrEmpty(dto.Email))
            {
                var existingEmail = await _unitOfWork.Members.GetByEmailAsync(dto.Email);
                if (existingEmail != null && existingEmail.Id != id)
                    throw new ValidationException("البريد الإلكتروني موجود مسبقاً");
            }

            // Update member fields
            member.FullName = dto.FullName;
            member.Phone = dto.Phone;
            member.Email = dto.Email;

            // Save changes
            _unitOfWork.Members.Update(member);
            await _unitOfWork.SaveChangesAsync();

            // Return response
            return member.ToDto();
        }

        public async Task DeleteAsync(int id)
        {
            // Get member by id
            var member = await _unitOfWork.Members.GetByIdTrackedAsync(id);
            if (member == null)
                throw new NotFoundException("المشترك غير موجود");

            // Soft delete member
            _unitOfWork.Members.Delete(member);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}