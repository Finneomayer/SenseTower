using FluentValidation;
using SC.SenseTower.Admin.Services;

namespace SC.SenseTower.Admin.Cqrs.UserDelete
{
    public class UserDeleteCommandValidator : AbstractValidator<UserDeleteCommand>
    {
        public UserDeleteCommandValidator(IdentityService identityService)
        {
            RuleFor(x => x.AccessToken)
                .Must((c, token) =>
                {
                    return !string.IsNullOrEmpty(token) || !string.IsNullOrEmpty(c.RefreshToken);
                }).WithMessage("Токен доступа отсутствует, выполните повторный вход.");
            RuleFor(x => x.UserId)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Не выбран пользователь.")
                .MustAsync(async (c, userId, cancellationToken) =>
                {
                    var user = await identityService.Get(userId, cancellationToken);
                    return user != null;
                }).WithMessage("Пользователь не найден.");
        }
    }
}
