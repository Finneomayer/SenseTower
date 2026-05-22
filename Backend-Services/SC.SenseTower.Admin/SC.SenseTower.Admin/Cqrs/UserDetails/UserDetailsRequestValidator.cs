using FluentValidation;
using SC.SenseTower.Admin.Services;

namespace SC.SenseTower.Admin.Cqrs.UserDetails
{
    public class UserDetailsRequestValidator : AbstractValidator<UserDetailsRequest>
    {
        public UserDetailsRequestValidator(IdentityService identityService)
        {
            RuleFor(x => x.UserId)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Не указан идентификатор пользователя.")
                .MustAsync(async (c, userId, cancellationToken) =>
                {
                    var user = await identityService.Get(userId, cancellationToken);
                    return user != null;
                }).WithMessage("Пользователь не найден.");
        }
    }
}
