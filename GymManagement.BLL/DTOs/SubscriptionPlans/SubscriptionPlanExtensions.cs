using GymManagement.DAL.Entities;
namespace GymManagement.BLL.DTOs.SubscriptionPlans
{
    public static class SubscriptionPlanExtensions
    {
        public static SubscriptionPlanResponseDto ToDto(this SubscriptionPlan subscriptionPlan)
        {
            return new SubscriptionPlanResponseDto
            {
                Id = subscriptionPlan.Id,
                Name = subscriptionPlan.Name,
                Price = subscriptionPlan.Price,
                DurationDays = subscriptionPlan.DurationDays
            };
        }
    }
}

