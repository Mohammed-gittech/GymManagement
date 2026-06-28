using GymManagement.API.Helpers;
using GymManagement.BLL.DTOs.SubscriptionPlans;
using GymManagement.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymManagement.API.Controllers
{
    [ApiController]
    [Route("api/plans")]
    [Authorize]
    public class SubscriptionPlansController : ControllerBase
    {
        private readonly ISubscriptionPlanService _planService;

        public SubscriptionPlansController(ISubscriptionPlanService planService)
        {
            _planService = planService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<IEnumerable<SubscriptionPlanResponseDto>>>> GetAll()
        {
            var result = await _planService.GetAllAsync();
            return Ok(ApiResponse<IEnumerable<SubscriptionPlanResponseDto>>.Ok(result, "تم جلب الباقات بنجاح"));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<SubscriptionPlanResponseDto>> GetById(int id)
        {
            var result = await _planService.GetByIdAsync(id);
            return Ok(ApiResponse<SubscriptionPlanResponseDto>.Ok(result, "تم جلب الباقة بنجاح"));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<SubscriptionPlanResponseDto>> Create(CreateSubscriptionPlanDto dto)
        {
            var result = await _planService.CreateAsync(dto);
            return CreatedAtAction(
                nameof(GetById),
                new { id = result.Id },
                ApiResponse<SubscriptionPlanResponseDto>.Ok(result, "تم انشاء الباقة بنجاح"));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<SubscriptionPlanResponseDto>> Update(UpdateSubscriptionPlanDto dto, int id)
        {
            var result = await _planService.UpdateAsync(dto, id);
            return Ok(ApiResponse<SubscriptionPlanResponseDto>.Ok(result, "تم تحديث الباقة بنجاح"));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Delete(int id)
        {
            await _planService.DeleteAsync(id);
            return Ok(ApiResponse<object>.Ok("تم حذف الباقة بنجاح"));
        }

    }
}