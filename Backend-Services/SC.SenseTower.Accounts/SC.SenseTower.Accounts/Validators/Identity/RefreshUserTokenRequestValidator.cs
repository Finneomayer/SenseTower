using FluentValidation;
using SC.SenseTower.Accounts.Cqrs.Requests;

namespace SC.SenseTower.Accounts.Validators.Identity
{
    public class RefreshUserTokenRequestValidator : AbstractValidator<RefreshUserTokenRequest>
    {
        public RefreshUserTokenRequestValidator()
        {
            RuleFor(x => x.RefreshToken).NotEmpty().WithMessage("Не указан токен обновления");
        }
    }
}
