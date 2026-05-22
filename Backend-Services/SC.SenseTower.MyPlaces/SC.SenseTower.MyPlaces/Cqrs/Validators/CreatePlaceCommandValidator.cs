using FluentValidation;
using SC.SenseTower.MyPlaces.Cqrs.Commands;
using SC.SenseTower.MyPlaces.Services;

namespace SC.SenseTower.MyPlaces.Cqrs.Validators
{
    public class CreatePlaceCommandValidator : AbstractValidator<CreatePlaceCommand>
    {
        public CreatePlaceCommandValidator(PlacesService placesService)
        {
            RuleFor(x => x.PlaceName).NotEmpty().WithMessage("Не задано название помещения");
            RuleFor(x => x.SpaceId)
                .MustAsync(async (c, spaceId, cancellationToken) =>
                {
                    if (spaceId == null)
                        return true;
                    var place = await placesService.GetBySpaceId(spaceId.Value, cancellationToken);
                    return place == null;
                }).WithMessage("Указанное пространство уже привязано к другому помещению");
            RuleFor(x => x.PlaceNumber)
                .Cascade(CascadeMode.Stop)
                .GreaterThan(0).WithMessage("Номер помещения должен быть больше 0")
                .MustAsync(async (c, number, cancellationToken) =>
                {
                    var place = await placesService.GetByNumber(number, cancellationToken);
                    return place == null;
                }).WithMessage("Помещение с таким номером уже существует");
        }
    }
}
