using FluentValidation;
using SC.SenseTower.Galleries.Services;

namespace SC.SenseTower.Galleries.Cqrs.UpdateGallery
{
    public class UpdateGalleryCommandValidator : AbstractValidator<UpdateGalleryCommand>
    {
        public UpdateGalleryCommandValidator(GalleriesService galleriesService)
        {
            RuleFor(x => x.Id)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Не выбрана галерея")
                .MustAsync(async (c, id, cancellationToken) =>
                {
                    var gallery = await galleriesService.Get(id, cancellationToken);
                    return gallery != null;
                }).WithMessage("Галерея не найдена");
            RuleFor(x => x.Name).NotEmpty().WithMessage("Необходимо указать название галереи");
            RuleFor(x => x.AccessToken).NotEmpty().WithMessage("Недостаточно прав для создания галереи");
            RuleFor(x => x.UserId).NotEmpty().WithMessage("Пользователь не определен");
            RuleFor(x => x.SpaceId).NotEmpty().WithMessage("Необходимо выбрать пространство для галереи");
            RuleFor(x => x.ImageId)
                .Must((c, imageId) =>
                {
                    if (string.IsNullOrWhiteSpace(c.Description))
                        return (imageId != null && c.ImageFile == null) || (imageId == null && c.ImageFile != null);
                    else
                        return !(imageId != null && c.ImageFile != null);
                }).WithMessage("Необходимо выбрать либо существующее изображение, либо загрузить новое");
            RuleFor(x => x.Description)
                .Must((c, description) =>
                {
                    if (c.ImageId != null || c.ImageFile != null)
                        return true;
                    return !string.IsNullOrWhiteSpace(description);
                }).WithMessage("Необходимо либо ввести описание, либо добавить изображение");
        }
    }
}
