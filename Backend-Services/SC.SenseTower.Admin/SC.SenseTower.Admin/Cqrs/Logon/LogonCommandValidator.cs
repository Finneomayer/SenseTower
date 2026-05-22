using FluentValidation;

namespace SC.SenseTower.Admin.Cqrs.Logon
{
    public class LogonCommandValidator : AbstractValidator<LogonCommand>
    {
        public LogonCommandValidator()
        {
            RuleFor(x => x.UserName).NotEmpty().WithMessage("Не указано имя входа.");
            RuleFor(x => x.Password).NotEmpty().WithMessage("Не указан пароль.");
        }
    }
}
