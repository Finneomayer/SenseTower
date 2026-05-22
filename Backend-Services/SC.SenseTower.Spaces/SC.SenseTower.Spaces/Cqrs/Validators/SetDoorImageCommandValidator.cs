using FluentValidation;
using SC.SenseTower.Spaces.Cqrs.Commands;
using SC.SenseTower.Spaces.Services;

namespace SC.SenseTower.Spaces.Cqrs.Validators
{
    public class SetDoorImageCommandValidator : AbstractValidator<SetDoorImageCommand>
    {
        public SetDoorImageCommandValidator(ISpacesService spacesService, ImagesService imagesService)
        {
            RuleFor(x => x.SpaceId)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Не указан идентификатор пространства")
                .MustAsync(async (c, spaceId, cancellationToken) =>
                {
                    var space = await spacesService.Get(spaceId, cancellationToken);
                    return space != null;
                }).WithMessage("Пространство не найдено");
            RuleFor(x => x.ImageId)
                .MustAsync(async (c, imageId, cancellationToken) =>
                {
                    if (imageId == null)
                        return true;
                    var image = await imagesService.GetInfo(c.AccessToken, imageId.Value, cancellationToken);
                    return image != null;
                }).WithMessage("Изображение не найдено");
        }
    }
}
