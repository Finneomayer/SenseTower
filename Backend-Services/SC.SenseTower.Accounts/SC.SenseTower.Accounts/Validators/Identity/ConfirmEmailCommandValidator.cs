using FluentValidation;
using SC.SenseTower.Accounts.Cqrs.Commands;
using SC.SenseTower.Accounts.Services;

namespace SC.SenseTower.Accounts.Validators.Identity
{
    public class ConfirmEmailCommandValidator : AbstractValidator<ConfirmEmailCommand>
    {
        public ConfirmEmailCommandValidator(IdentityService identityService)
        {
            RuleFor(x => x.Code).NotEmpty().WithMessage("Не указан токен валидации email.");
            RuleFor(x => x.UserId)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Не указан пользователь.")
                .MustAsync(async (_, userId, cancellationToken) =>
                {
                    return !await identityService.IsEmailConfirmed(userId, cancellationToken);
                }).WithMessage("Email уже подтверждён, повторное подтверждение не требуется.");
        }
    }
}
