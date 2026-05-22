using FluentValidation;

namespace SC.SenseTower.Admin.Cqrs.ResetRequest
{
    public class ResetRequestCommandValidator : AbstractValidator<ResetRequestCommand>
    {
        public ResetRequestCommandValidator()
        {
            RuleFor(x => x.LoginOrEmail).NotEmpty().WithMessage("Не указаны имя входа или регистрационный email.");
            RuleFor(x => x.CallbackUrl)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Не указан URL обратного вызова.")
                .Must(url => (url.StartsWith("http://") || url.StartsWith("https://")) && url.Contains('.') && !url.EndsWith('/')).WithMessage("Неверный формат URL обратного вызова.");
        }
    }
}
