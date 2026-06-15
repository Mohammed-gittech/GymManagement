
using FluentValidation;
using GymManagement.BLL.DTOs.SubscriptionPlans;

namespace GymManagement.BLL.Validators.SubscriptionPlan
{
    public class CreateSubscriptionPlanDtoValidator : AbstractValidator<CreateSubscriptionPlanDto>
    {
        public CreateSubscriptionPlanDtoValidator()
        {
            RuleFor(sp => sp.Name)
                .NotEmpty()
                .WithMessage("اسم الباقة مطلوب")
                .MaximumLength(100)
                .WithMessage("اسم الباقة يجب أن لا يتجاوز 100 حرف");

            RuleFor(sp => sp.Price)
                .GreaterThan(0)
                .WithMessage("سعر الباقة يجب أن يكون أكبر من صفر");

            RuleFor(sp => sp.DurationDays)
                .GreaterThan(0)
                .WithMessage("مدة الباقة يجب أن تكون أكبر من صفر");
        }
    }
}