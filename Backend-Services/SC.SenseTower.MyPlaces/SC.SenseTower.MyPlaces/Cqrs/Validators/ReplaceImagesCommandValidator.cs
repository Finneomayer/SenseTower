using FluentValidation;
using SC.SenseTower.MyPlaces.Constants;
using SC.SenseTower.MyPlaces.Cqrs.Commands;
using SC.SenseTower.MyPlaces.Data.Models;
using SC.SenseTower.MyPlaces.Services;

namespace SC.SenseTower.MyPlaces.Cqrs.Validators
{
    public class ReplaceImagesCommandValidator : AbstractValidator<ReplaceImagesCommand>
    {
        private Place? place = null;

        public ReplaceImagesCommandValidator(PlacesService placesService)
        {
            RuleFor(x => x.PlaceId)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Не указано помещение")
                .MustAsync(async (c, id, cancellationToken) =>
                {
                    place ??= await placesService.Get(id, cancellationToken);
                    return place != null;
                }).WithMessage("Помещение не найдено")
                .MustAsync(async (c, id, cancellationToken) =>
                {
                    if (c.Role == RoleNames.VR_ADMIN)
                        return true;
                    place ??= await placesService.Get(id, cancellationToken);
                    return place?.OwnerId == c.UserId;
                }).WithMessage("Вы не владелец помещения");
        }
    }
}
