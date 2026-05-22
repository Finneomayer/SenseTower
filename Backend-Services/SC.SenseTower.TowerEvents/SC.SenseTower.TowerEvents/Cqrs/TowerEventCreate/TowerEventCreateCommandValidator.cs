using FluentValidation;
using SC.SenseTower.TowerEvents.Services;

namespace SC.SenseTower.TowerEvents.Cqrs.TowerEventCreate
{
    public class TowerEventCreateCommandValidator : AbstractValidator<TowerEventCreateCommand>
    {
        public TowerEventCreateCommandValidator(
            SpacesService spacesService,
            ImagesService imagesService)
        {
            RuleFor(x => x.Title).NotEmpty().WithMessage("Не указано название мероприятия");
            RuleFor(x => x.Date)
                .GreaterThan(DateTime.Now).WithMessage("Начало события должно быть больше текущего времени");
            RuleFor(x => x.UpTo)
                .Cascade(CascadeMode.Stop)
                .Must((c, upTo) => upTo > c.Date).WithMessage("Окончание события должно быть позже начала");
            RuleFor(x => x.SpaceId)
                .MustAsync(async (c, spaceId, cancellationToken) =>
                {
                    if (spaceId == null)
                        return true;
                    var space = await spacesService.Get(c.AccessToken, spaceId.Value, cancellationToken);
                    return space != null;
                }).WithMessage("Указанное пространство не найдено");
            RuleFor(x => x.ImageId)
                .MustAsync(async (c, imageId, cancellationToken) =>
                {
                    if (imageId == null)
                        return true;
                    var image = await imagesService.Get(c.AccessToken, imageId.Value, cancellationToken);
                    return image != null;
                }).WithMessage("Указанное изображение не найдено");
        }
    }
}
