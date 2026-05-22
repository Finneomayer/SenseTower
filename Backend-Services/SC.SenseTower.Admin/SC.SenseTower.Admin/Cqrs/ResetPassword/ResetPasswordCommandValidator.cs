using FluentValidation;
using Microsoft.AspNetCore.Identity;
using SC.SenseTower.Admin.Constants;
using SC.SenseTower.Admin.Data.Models.Identity;

namespace SC.SenseTower.Admin.Cqrs.ResetPassword
{
    public class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
    {
        private ApplicationUser? user = null;

        public ResetPasswordCommandValidator(UserManager<ApplicationUser> userManager)
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
            RuleFor(x => x.Token).NotEmpty().WithMessage("Не указан код восстановления пароля.");
            RuleFor(x => x.Password)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Пароль не указан")
                .MinimumLength(12).WithMessage("Пароль не может быть короче 12 символов")
                .MaximumLength(30).WithMessage("Пароль не может быть длиннее 30 символов");
        }
    }
}
