
using GymManagement.BLL.DTOs.Payment;

namespace GymManagement.BLL.Services.Interfaces
{
    public interface IPaymentService
    {
        Task<PaymentResponseDto> CreateAsync(CreatePaymentDto dto);
        Task<IEnumerable<PaymentResponseDto>> GetByMemberIdAsync(int memberId);
    }
}