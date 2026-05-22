using FluentValidation;
using SC.SenseTower.Accounts.Cqrs.Commands;
using SC.SenseTower.Accounts.Services;

namespace SC.SenseTower.Accounts.Validators.Identity
{
    public class ResetRequestCommandValidator : AbstractValidator<ResetRequestCommand>
    {
        public ResetRequestCommandValidator(IdentityService identityService)
        {
            RuleFor(x => x.LoginOrEmail)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Не указаны логин или email.")
                .MustAsync((_, loginOrEmail, cancellationToken) => 
                    identityService.CheckLoginOrEmail(loginOrEmail, cancellationToken)).WithMessage("Несуществующие учётные данные.");
        }
    }
}
