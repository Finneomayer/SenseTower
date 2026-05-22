using FluentValidation;
using Microsoft.AspNetCore.Identity;
using SC.SenseTower.Admin.Constants;
using SC.SenseTower.Admin.Data.Models.Identity;

namespace SC.SenseTower.Admin.Cqrs.SetPassword
{
    public class SetPasswordCommandValidator : AbstractValidator<SetPasswordCommand>
    {
        private ApplicationUser? user = null;

        public SetPasswordCommandValidator(UserManager<ApplicationUser> userManager)
        {
            RuleFor(x => x.UserId)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Не указан пользователь.")
                .MustAsync(async (c, userId, cancellationToken) =>
                {
                    user ??= await userManager.FindByIdAsync(userId.ToString());
                    return user != null;
                }).WithMessage("Пользователь не найден.")
                .MustAsync(async (c, userId, cancellationToken) =>
                {
                    user ??= await userManager.FindByIdAsync(userId.ToString());
                    return await userManager.IsInRoleAsync(user, RoleNames.VR_ADMIN);
                }).WithMessage("Пользователь должен обладать правами администратора.");
            RuleFor(x => x.CurrentPassword)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Не указан текущий пароль.")
                .MustAsync(async (c, password, cancellationToken) =>
                {
                    user ??= await userManager.FindByIdAsync(c.UserId.ToString());
                    return await userManager.CheckPasswordAsync(user, password);
                }).WithMessage("Неверный текущий пароль.");
        }
    }
}
