using GymManagement.API.Helpers;
using GymManagement.BLL.DTOs.Subscription;
using GymManagement.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymManagement.API.Controllers
{
    [ApiController]
    [Route("api/subscriptions")]
    [Authorize]
    public class SubscriptionsController : ControllerBase
    {
        private readonly ISubscriptionService _subscriptionService;

        public SubscriptionsController(ISubscriptionService subscriptionService)
        {
            _subscriptionService = subscriptionService;
        }

        [HttpGet("{memberId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<IEnumerable<SubscriptionResponseDto>>>> GetByMemberId(int memberId)
        {
            var result = await _subscriptionService.GetByMemberIdAsync(memberId);

            return Ok(ApiResponse<IEnumerable<SubscriptionResponseDto>>.Ok(result, "تم جلب الاشتراكات"));
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<SubscriptionResponseDto>>> Create(CreateSubscriptionDto dto)
        {
            var result = await _subscriptionService.CreateAsync(dto);

            return StatusCode(StatusCodes.Status201Created, ApiResponse<SubscriptionResponseDto>.Ok(result, "تم انشاء الاشتراك بنجاح"));
        }

    }
}