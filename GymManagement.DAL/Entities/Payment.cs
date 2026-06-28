
using GymManagement.DAL.Entities.Common;
using GymManagement.DAL.Entities.Enums;

namespace GymManagement.DAL.Entities
{
    public class Payment : BaseEntity
    {
        public int SubscriptionId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Cash;
        public string? Notes { get; set; }

        // Navigation properties 
        public Subscription Subscription { get; set; } = null!;
    }
}