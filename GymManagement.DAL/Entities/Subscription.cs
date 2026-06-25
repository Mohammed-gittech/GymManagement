
using GymManagement.DAL.Entities.Common;

namespace GymManagement.DAL.Entities
{
    public class Subscription : BaseEntity
    {
        public int MemberId { get; set; }
        public int SubscriptionPlanId { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        // Navigation properties (required relationships)
        public Member Member { get; set; } = null!;
        public SubscriptionPlan SubscriptionPlan { get; set; } = null!;
    }
}