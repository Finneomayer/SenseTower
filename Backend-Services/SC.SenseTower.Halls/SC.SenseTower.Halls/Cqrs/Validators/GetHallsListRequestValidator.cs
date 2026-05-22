using FluentValidation;
using SC.SenseTower.Halls.Cqrs.Requests;

namespace SC.SenseTower.Halls.Cqrs.Validators
{
    public class GetHallsListRequestValidator : AbstractValidator<GetHallsListRequest>
    {
        public GetHallsListRequestValidator()
        {
            RuleFor(x => x.UserId).NotEmpty().WithMessage("Пользователь не определён.");
            RuleFor(x => x.AccessToken).NotEmpty().WithMessage("Пользователь не авторизован.");
        }
    }
}
