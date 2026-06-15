
namespace GymManagement.BLL.DTOs.SubscriptionPlans
{
    public class SubscriptionPlanResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int DurationDays { get; set; }
    }
}