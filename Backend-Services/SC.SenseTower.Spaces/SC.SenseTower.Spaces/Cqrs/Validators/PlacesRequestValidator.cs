using FluentValidation;
using SC.SenseTower.Spaces.Cqrs.Requests;

namespace SC.SenseTower.Spaces.Cqrs.Validators
{
    public class PlacesRequestValidator : AbstractValidator<PlacesRequest>
    {
        public PlacesRequestValidator()
        {
            RuleFor(x => x.PlaceIds).NotEmpty().WithMessage("Не выбраны помещения.");
            RuleFor(x => x.AccessToken).NotEmpty().WithMessage("Не определён токен доступа пользователя.");
        }
    }
}
