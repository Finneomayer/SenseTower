using FluentValidation;
using SC.SenseTower.MyPlaces.Cqrs.Commands;
using SC.SenseTower.MyPlaces.Data.Models;
using SC.SenseTower.MyPlaces.Services;

namespace SC.SenseTower.MyPlaces.Cqrs.Validators
{
    public class UpdateCommandValidator : AbstractValidator<UpdateCommand>
    {
        private Place? place = null;

        public UpdateCommandValidator(PlacesService placesService)
        {
            RuleFor(x => x.PlaceName).NotEmpty().WithMessage("Не указано название помещения");
            RuleFor(x => x.Id)
                .MustAsync(async (c, id, cancellationToken) =>
                {
                    place ??= (await placesService.GetPlacesByIds(new Guid[] { id }, cancellationToken))?.FirstOrDefault();
                    return place != null;
                }).WithMessage("Помещение не найдено");
            RuleFor(x => x.SpaceId)
                .MustAsync(async (c, spaceId, cancellationToken) =>
                {
                    if (spaceId == null)
                        return true;
                    var anotherPlace = await placesService.GetBySpaceId(spaceId.Value, cancellationToken);
                    return anotherPlace == null || anotherPlace.Id == c.Id;
                }).WithMessage("Указанное пространство уже привязано к другому помещению");
            RuleFor(x => x.PlaceNumber)
                .Cascade(CascadeMode.Stop)
                .GreaterThan(0).WithMessage("Номер помещения должен быть больше 0")
                .MustAsync(async (c, number, cancellationToken) =>
                {
                    var anotherPlace = await placesService.GetByNumber(number, cancellationToken);
                    return anotherPlace == null || anotherPlace.Id == c.Id;
                }).WithMessage("Помещение с таким номером уже существует");
        }
    }
}
