using FluentValidation;
using SC.SenseTower.Admin.Data.Models.Identity;
using SC.SenseTower.Admin.Services;

namespace SC.SenseTower.Admin.Cqrs.UserUnban
{
    public class UserUnbanCommandValidator : AbstractValidator<UserUnbanCommand>
    {
        private ApplicationUser? user = null;

        public UserUnbanCommandValidator(IdentityService identityService)
        {
            RuleFor(x => x.UserId)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Не выбран пользователь.")
                .MustAsync(async (c, userId, cancellationToken) =>
                {
                    user ??= await identityService.Get(userId, cancellationToken);
                    return user != null;
                }).WithMessage("Пользователь не найден.");
        }
    }
}
