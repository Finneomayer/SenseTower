using FluentValidation;
using SC.SenseTower.Accounts.Cqrs.Requests;

namespace SC.SenseTower.Accounts.Validators.UserInfo
{
    public class GetCurrentUserInfoRequestValidator : AbstractValidator<GetCurrentUserInfoRequest>
    {
        public GetCurrentUserInfoRequestValidator()
        {
            RuleFor(x => x.Token).NotEmpty().WithMessage("Пользователь не авторизован");
        }
    }
}
