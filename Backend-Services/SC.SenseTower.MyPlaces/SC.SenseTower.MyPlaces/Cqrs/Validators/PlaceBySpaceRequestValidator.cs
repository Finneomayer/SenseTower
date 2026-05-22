using FluentValidation;
using SC.SenseTower.MyPlaces.Cqrs.Requests;

namespace SC.SenseTower.MyPlaces.Cqrs.Validators
{
    public class PlaceBySpaceRequestValidator : AbstractValidator<PlaceBySpaceRequest>
    {
        public PlaceBySpaceRequestValidator()
        {
            RuleFor(x => x.SpaceId).NotEmpty().WithMessage("Не указан идентификатор пространства");
        }
    }
}
