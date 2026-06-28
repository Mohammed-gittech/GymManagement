
using GymManagement.DAL.Entities.Enums;

namespace GymManagement.BLL.DTOs.Payment
{
    public class PaymentResponseDto
    {
        public int Id { get; set; }
        public int SubscriptionId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Cash;
        public string? Notes { get; set; }
    }
}