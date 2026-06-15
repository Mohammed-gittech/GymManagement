
using GymManagement.DAL.Entities.Common;

namespace GymManagement.DAL.Entities
{
    public class SubscriptionPlan : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int DurationDays { get; set; }
    }
}