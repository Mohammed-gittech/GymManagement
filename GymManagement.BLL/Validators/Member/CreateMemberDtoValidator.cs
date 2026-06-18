
using FluentValidation;
using GymManagement.BLL.DTOs.Members;

namespace GymManagement.BLL.Validators.Member
{
    public class CreateMemberDtoValidator : AbstractValidator<CreateMemberDto>
    {
        public CreateMemberDtoValidator()
        {
            RuleFor(m => m.FullName)
                .NotEmpty()
                .WithMessage("الاسم الكامل مطلوب")
                .MaximumLength(100)
                .WithMessage("الاسم يجب أن لا يتجاوز 100 حرف");

            RuleFor(m => m.Phone)
                .NotEmpty()
                .WithMessage("رقم الهاتف مطلوب")
                .MaximumLength(50)
                .WithMessage("رقم الهاتف يجب أن لا يتجاوز 50 حرف");

            RuleFor(m => m.Email)
                .EmailAddress()
                .WithMessage("البريد الإلكتروني غير صحيح")
                .MaximumLength(100)
                .WithMessage("البريد الإلكتروني يجب أن لا يتجاوز 100 حرف")
                .When(m => !string.IsNullOrEmpty(m.Email));
        }
    }
}