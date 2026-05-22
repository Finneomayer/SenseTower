using FluentValidation;
using SC.SenseTower.Accounts.Cqrs.Requests;

namespace SC.SenseTower.Accounts.Validators.Identity
{
    public class CheckIdentityRequestValidator : AbstractValidator<CheckIdentityRequest>
    {
        public CheckIdentityRequestValidator()
        {
            RuleFor(x => x.AccessToken).NotEmpty().WithMessage("Пользователь не авторизован.");
            RuleFor(x => x.Tokens)
                .Must((tokens) =>
                {
                    return !tokens.Any(x => x.UserId == Guid.Empty || string.IsNullOrWhiteSpace(x.Token));
                }).WithMessage("Не указаны данные для проверки.");
        }
    }
}
