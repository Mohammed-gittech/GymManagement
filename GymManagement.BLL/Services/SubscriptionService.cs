
using GymManagement.BLL.DTOs.Subscription;
using GymManagement.BLL.Exceptions;
using GymManagement.BLL.Services.Interfaces;
using GymManagement.DAL.Entities;
using GymManagement.DAL.UnitOfWork;

namespace GymManagement.BLL.Services
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly IUnitOfWork _unitOfWork;
        public SubscriptionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<SubscriptionResponseDto> CreateAsync(CreateSubscriptionDto dto)
        {
            var existingPlan = await _unitOfWork.SubscriptionPlans.GetByIdAsync(dto.SubscriptionPlanId);

            if (existingPlan == null)
                throw new NotFoundException("الباقة غير موجودة");

            var activeSubscription = await _unitOfWork.Subscriptions.GetActiveByMemberIdAsync(dto.MemberId);

            if (activeSubscription != null)
            {
                var remainingDays = (activeSubscription.EndDate - DateTime.UtcNow).Days;

                activeSubscription.EndDate = DateTime.UtcNow;

                var newSubscription = new Subscription
                {
                    MemberId = dto.MemberId,
                    SubscriptionPlanId = dto.SubscriptionPlanId,
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddDays(existingPlan.DurationDays + remainingDays)
                };

                await _unitOfWork.BeginTransactionAsync();
                try
                {
                    _unitOfWork.Subscriptions.Update(activeSubscription);
                    _unitOfWork.Subscriptions.Add(newSubscription);
                    await _unitOfWork.SaveChangesAsync();
                    await _unitOfWork.CommitTransactionAsync();
                }
                catch
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    throw;
                }

                newSubscription.SubscriptionPlan = existingPlan;
                return newSubscription.ToDto();
            }
            else
            {
                var newSubscription = new Subscription
                {
                    MemberId = dto.MemberId,
                    SubscriptionPlanId = dto.SubscriptionPlanId,
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddDays(existingPlan.DurationDays)
                };

                _unitOfWork.Subscriptions.Add(newSubscription);
                await _unitOfWork.SaveChangesAsync();

                newSubscription.SubscriptionPlan = existingPlan;
                return newSubscription.ToDto();
            }
        }
        public async Task<IEnumerable<SubscriptionResponseDto>> GetByMemberIdAsync(int memberId)
        {
            var subscription = await _unitOfWork.Subscriptions.GetByMemberIdAsync(memberId);

            return subscription.Select(s => s.ToDto());
        }
    }
}