using SubscriptionEntity = GymManagement.DAL.Entities.Subscription;

namespace GymManagement.BLL.DTOs.Subscription
{
    public static class SubscriptionExtensions
    {
        public static SubscriptionResponseDto ToDto(this SubscriptionEntity subscription)
        {
            return new SubscriptionResponseDto
            {
                Id = subscription.Id,
                MemberId = subscription.MemberId,
                SubscriptionPlanId = subscription.SubscriptionPlanId,
                StartDate = subscription.StartDate,
                EndDate = subscription.EndDate,
                IsActive = subscription.EndDate >= DateTime.UtcNow,
                PlanName = subscription.SubscriptionPlan.Name,
            };
        }
    }
}