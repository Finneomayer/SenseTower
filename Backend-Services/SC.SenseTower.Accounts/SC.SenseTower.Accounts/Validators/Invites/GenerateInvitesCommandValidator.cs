using FluentValidation;
using SC.SenseTower.Accounts.Constants;
using SC.SenseTower.Accounts.Cqrs.Commands;

namespace SC.SenseTower.Accounts.Validators.Invites
{
    public class GenerateInvitesCommandValidator : AbstractValidator<GenerateInvitesCommand>
    {
        public GenerateInvitesCommandValidator()
        {
            RuleFor(x => x.Quantity).GreaterThan(0).WithMessage("Количество инвайтов должно быть больше 0.");
            RuleFor(x => x.UserId).NotEmpty().WithMessage("Пользователь не авторизован.");
            RuleFor(x => x.Role)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Роль пользователя не определена.")
                .Equal(RoleNames.VR_ADMIN).WithMessage("Генерация инвайтов разрешена только администраторам.");
        }
    }
}
