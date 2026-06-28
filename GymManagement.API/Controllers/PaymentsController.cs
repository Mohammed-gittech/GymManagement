using GymManagement.API.Helpers;
using GymManagement.BLL.DTOs.Payment;
using GymManagement.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymManagement.API.Controllers
{
    [ApiController]
    [Route("api/payments")]
    [Authorize]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentsController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpGet("{memberId}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<ApiResponse<IEnumerable<PaymentResponseDto>>>> GetByMemberId(int memberId)
        {
            var result = await _paymentService.GetByMemberIdAsync(memberId);

            return Ok(ApiResponse<IEnumerable<PaymentResponseDto>>.Ok(result, "تم جلب الدفعات بنجاح"));
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<PaymentResponseDto>>> Create(CreatePaymentDto dto)
        {
            var result = await _paymentService.CreateAsync(dto);

            return StatusCode(StatusCodes.Status201Created, ApiResponse<PaymentResponseDto>.Ok(result, "تم إنشاء دفعة بنجاح"));
        }

    }
}