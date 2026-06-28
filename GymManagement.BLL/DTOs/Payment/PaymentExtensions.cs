using PaymentEntity = GymManagement.DAL.Entities.Payment;
namespace GymManagement.BLL.DTOs.Payment
{
    public static class PaymentExtensions
    {
        public static PaymentResponseDto ToDto(this PaymentEntity payment)
        {
            return new PaymentResponseDto
            {
                Id = payment.Id,
                SubscriptionId = payment.SubscriptionId,
                Amount = payment.Amount,
                PaymentDate = payment.PaymentDate,
                PaymentMethod = payment.PaymentMethod,
                Notes = payment.Notes
            };
        }
    }
}