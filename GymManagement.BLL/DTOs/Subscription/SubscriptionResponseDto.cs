
namespace GymManagement.BLL.DTOs.Subscription
{
    public class SubscriptionResponseDto
    {
        public int Id { get; set; }
        public int MemberId { get; set; }
        public int SubscriptionPlanId { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }

        public string PlanName { get; set; } = string.Empty;
    }
}