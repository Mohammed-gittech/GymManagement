
using FluentValidation;
using GymManagement.BLL.DTOs.Payment;

namespace GymManagement.BLL.Validators.Payment
{
    public class CreatePaymentDtoValidator : AbstractValidator<CreatePaymentDto>
    {
        public CreatePaymentDtoValidator()
        {
            RuleFor(p => p.SubscriptionId)
                .GreaterThan(0)
                .WithMessage("يجب ان يكون اكبر من 0");

            RuleFor(p => p.Amount)
                .GreaterThan(0)
                .WithMessage("المبلغ يجب ان يكون اكبر من الصفر");

            RuleFor(p => p.PaymentMethod)
                .IsInEnum()
                .WithMessage("طريقه الدفع غير صحيحه");

            RuleFor(p => p.Notes)
                .MaximumLength(255)
                .WithMessage("الملاحضات لا تتجاوز 255 حرف")
                .When(p => !string.IsNullOrEmpty(p.Notes));
        }
    }
}