using GymManagement.API.Helpers;
using GymManagement.BLL.DTOs.Members;
using GymManagement.BLL.Services.Interfaces;
using GymManagement.DAL.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymManagement.API.Controllers
{
    [ApiController]
    [Route("api/members")]
    [Authorize]
    public class MembersController : ControllerBase
    {
        private readonly IMemberService _memberService;

        public MembersController(IMemberService memberService)
        {
            _memberService = memberService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<PagedResponse<MemberResponseDto>>>> GetAll(
            [FromQuery] PaginationParams paginationParams
        )
        {
            var result = await _memberService.GetAllAsync(paginationParams);
            return Ok(ApiResponse<PagedResponse<MemberResponseDto>>.Ok(result, "تم جلب المشتركين بنجاح"));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<MemberResponseDto>>> GetById(int id)
        {
            var result = await _memberService.GetByIdAsync(id);
            return Ok(ApiResponse<MemberResponseDto>.Ok(result, "تم جلب المشترك بنجاح"));
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<MemberResponseDto>>> Create(CreateMemberDto dto)
        {
            var result = await _memberService.CreateAsync(dto);

            return CreatedAtAction(
                nameof(GetById),
                new { id = result.Id },
                ApiResponse<MemberResponseDto>.Ok(result, "تم انشاء المشترك بنجاح")
            );
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<MemberResponseDto>>> Update(UpdateMemberDto dto, int id)
        {
            var result = await _memberService.UpdateAsync(dto, id);

            return Ok(ApiResponse<MemberResponseDto>.Ok(result, "تم تعديل المشترك بنجاح"));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            await _memberService.DeleteAsync(id);
            return Ok(ApiResponse<object>.Ok("تم حذف المشترك بنجاح"));
        }
    }
}