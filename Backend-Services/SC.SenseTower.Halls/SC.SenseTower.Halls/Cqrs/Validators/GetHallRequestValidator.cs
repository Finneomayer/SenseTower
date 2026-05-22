using FluentValidation;
using SC.SenseTower.Halls.Cqrs.Requests;

namespace SC.SenseTower.Halls.Cqrs.Validators
{
    public class GetHallRequestValidator : AbstractValidator<GetHallRequest>
    {
        public GetHallRequestValidator()
        {
            RuleFor(x => x.HallId).NotEmpty().WithMessage("Не задан идентификатор холла.");
            RuleFor(x => x.UserId).NotEmpty().WithMessage("Не определён идентификатор пользователя.");
            RuleFor(x => x.AccessToken).NotEmpty().WithMessage("Пользователь не авторизован.");
        }
    }
}
