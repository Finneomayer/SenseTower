using FluentValidation;
using SC.SenseTower.MyPlaces.Cqrs.Requests;

namespace SC.SenseTower.MyPlaces.Cqrs.Validators
{
    public class UserPlacesRequestValidator : AbstractValidator<UserPlacesRequest>
    {
        public UserPlacesRequestValidator()
        {
            RuleFor(x => x.UserId).NotEqual(default(Guid)).WithMessage("Пользователь не авторизован.");
            RuleFor(x => x.AccessToken).NotEmpty().WithMessage("Не определён токен доступа пользователя.");
        }
    }
}
