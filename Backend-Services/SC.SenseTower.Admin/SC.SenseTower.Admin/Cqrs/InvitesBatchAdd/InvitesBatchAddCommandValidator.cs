using FluentValidation;

namespace SC.SenseTower.Admin.Cqrs.InvitesBatchAdd
{
    public class InvitesBatchAddCommandValidator : AbstractValidator<InvitesBatchAddCommand>
    {
        public InvitesBatchAddCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("Не указан пользователь.");
            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("Количество приглашений должно быть больше 0.")
                .LessThanOrEqualTo(20).WithMessage("Количество приглашений не может превышать 20.");
        }
    }
}
