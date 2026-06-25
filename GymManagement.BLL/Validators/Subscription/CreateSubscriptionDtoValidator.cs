
using FluentValidation;
using GymManagement.BLL.DTOs.Subscription;

namespace GymManagement.BLL.Validators.Subscription
{
    public class CreateSubscriptionDtoValidator : AbstractValidator<CreateSubscriptionDto>
    {
        public CreateSubscriptionDtoValidator()
        {
            RuleFor(s => s.MemberId)
                .GreaterThan(0)
                .WithMessage("يجب ان يكون اكبر من 0");

            RuleFor(s => s.SubscriptionPlanId)
                .GreaterThan(0)
                .WithMessage("يجب ان يكون اكبر من 0");
        }
    }
}