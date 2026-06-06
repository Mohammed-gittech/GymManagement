
using FluentValidation;
using GymManagement.BLL.DTOs.Auth;

namespace GymManagement.BLL.Validators.Auth
{
    public class RegisterRequestDtoValidator : AbstractValidator<RegisterRequestDto>
    {
        public RegisterRequestDtoValidator()
        {
            RuleFor(x => x.Username)
            .NotEmpty()
            .WithMessage("اسم المستخدم مطلوب")
            .MaximumLength(50)
            .WithMessage("اسم المستخدم يجب أن لا يتجاوز 50 حرف");

            RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("كلمة المرور مطلوبة")
            .MinimumLength(8)
            .WithMessage("كلمة المرور يجب أن تكون 8 أحرف على الأقل")
            .Matches("[A-Z]")
            .WithMessage("كلمة المرور يجب أن تحتوي حرف كبير")
            .Matches("[a-z]")
            .WithMessage("كلمة المرور يجب أن تحتوي حرف صغير")
            .Matches("[0-9]")
            .WithMessage("كلمة المرور يجب أن تحتوي رقم")
            .Matches("[^a-zA-Z0-9]")
            .WithMessage("كلمة المرور يجب أن تحتوي رمز خاص");

            RuleFor(x => x.Role)
            .IsInEnum()
            .WithMessage("الصلاحية غير صحيحة — يجب أن تكون Admin أو Receptionist");
        }
    }
}