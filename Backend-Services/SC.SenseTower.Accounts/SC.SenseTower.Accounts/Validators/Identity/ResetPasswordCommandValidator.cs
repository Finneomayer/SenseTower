using FluentValidation;
using SC.SenseTower.Accounts.Cqrs.Commands;
using SC.SenseTower.Accounts.Data.Models;
using SC.SenseTower.Accounts.Services;

namespace SC.SenseTower.Accounts.Validators.Identity
{
    public class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
    {
        private Account? account = null;

        public ResetPasswordCommandValidator(AccountsService accountsService)
        {
            RuleFor(x => x.UserId)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Не указан пользователь.")
                .MustAsync(async (_, userId, cancellationToken) =>
                {
                    account ??= await accountsService.Get(userId ?? default, cancellationToken);
                    return account != null;
                }).WithMessage("Не найден аккаунт пользователя.");
            RuleFor(x => x.Token)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Не указан токен восстановления пароля.")
                .MustAsync(async (c, _, cancellationToken) =>
                {
                    account ??= await accountsService.Get(c.UserId ?? default, cancellationToken);
                    return !string.IsNullOrWhiteSpace(account.PasswordResetInfo?.Token);
                }).WithMessage("Токен восстановления пароля уже использован.")
                .MustAsync(async (c, _, cancellationToken) =>
                {
                    account ??= await accountsService.Get(c.UserId ?? default, cancellationToken);
                    return account.PasswordResetInfo?.ExpiredAt > DateTime.UtcNow;
                }).WithMessage("Токен восстановления пароля просрочен.")
                .MustAsync(async (c, token, cancellationToken) =>
                {
                    account ??= await accountsService.Get(c.UserId ?? default, cancellationToken);
                    return account.PasswordResetInfo?.Token == token;
                }).WithMessage("Невалидный токен восстановления пароля.");
            RuleFor(x => x.Password)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Пароль не указан")
                .MinimumLength(12).WithMessage("Пароль не может быть короче 12 символов")
                .MaximumLength(30).WithMessage("Пароль не может быть длиннее 30 символов");
        }
    }
}
