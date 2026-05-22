using FluentValidation;
using SC.SenseTower.Accounts.Cqrs.Commands;
using SC.SenseTower.Accounts.Services;

namespace SC.SenseTower.Accounts.Validators.UserInfo
{
    public class SetAvatarCommandValidator : AbstractValidator<SetAvatarCommand>
    {
        public SetAvatarCommandValidator(AccountsService accountsService)
        {
            RuleFor(x => x.UserId)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Не задан пользователь")
                .MustAsync(async (c, userId, cancellationToken) =>
                {
                    var result = await accountsService.Exists(userId, cancellationToken);
                    return result;
                }).WithMessage("Учётная запись пользователя не найдена");
            RuleFor(x => x.CurrentUserId)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Пользователь не авторизован");
            RuleFor(x => x.AvatarNumber)
                .Must((num) => num == null || num >= 0).WithMessage("Недопустимое значение индекса аватара в коллекции");
        }
    }
}
