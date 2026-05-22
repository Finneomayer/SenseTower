using FluentValidation;
using SC.SenseTower.MyPlaces.Cqrs.Requests;

namespace SC.SenseTower.MyPlaces.Cqrs.Validators
{
    public class PlacesByIdsRequestValidator : AbstractValidator<PlacesByIdsRequest>
    {
        public PlacesByIdsRequestValidator()
        {
            RuleFor(r => r.PlaceIds).NotEmpty().WithMessage("Необходимо указать список идентификаторов.");
            RuleFor(r => r.AccessToken).NotEmpty().WithMessage("Пользователь не определён.");
        }
    }
}
