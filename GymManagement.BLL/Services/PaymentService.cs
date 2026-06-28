
using GymManagement.BLL.DTOs.Payment;
using GymManagement.BLL.Exceptions;
using GymManagement.BLL.Services.Interfaces;
using GymManagement.DAL.Entities;
using GymManagement.DAL.UnitOfWork;

namespace GymManagement.BLL.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PaymentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PaymentResponseDto> CreateAsync(CreatePaymentDto dto)
        {
            // Check if subscription exists
            var existingSubscription = await _unitOfWork.Subscriptions.GetByIdAsync(dto.SubscriptionId);

            if (existingSubscription == null)
                throw new NotFoundException("الاشتراك غير موجود");

            var newPayment = new Payment
            {
                SubscriptionId = dto.SubscriptionId,
                Amount = dto.Amount,
                PaymentDate = DateTime.UtcNow,
                PaymentMethod = dto.PaymentMethod,
                Notes = dto.Notes
            };

            _unitOfWork.Payments.Add(newPayment);
            await _unitOfWork.SaveChangesAsync();

            return newPayment.ToDto();
        }

        public async Task<IEnumerable<PaymentResponseDto>> GetByMemberIdAsync(int memberId)
        {
            var payment = await _unitOfWork.Payments.GetByMemberIdAsync(memberId);

            return payment.Select(p => p.ToDto());
        }
    }
}