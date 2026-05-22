using FluentValidation;
using SC.SenseTower.Galleries.Data.Models;
using SC.SenseTower.Galleries.Services;

namespace SC.SenseTower.Galleries.Cqrs.DeleteGalleryImage
{
    public class DeleteGalleryImageCommandValidator : AbstractValidator<DeleteGalleryImageCommand>
    {
        private Data.Models.Gallery? gallery = null;
        public DeleteGalleryImageCommandValidator(GalleriesService galleriesService)
        {
            RuleFor(x => x.GalleryId)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Не указан идентификатор галереи")
                .MustAsync(async (c, galleryId, cancellationToken) =>
                {
                    gallery ??= await galleriesService.Get(galleryId, cancellationToken);
                    return gallery != null;
                }).WithMessage("Галерея не найдена");
            RuleFor(x => x.Position)
                .Cascade(CascadeMode.Stop)
                .GreaterThanOrEqualTo(0).WithMessage("Индекс позиции изображения не может быть отрицательным")
                .MustAsync(async (c, position, cancellationToken) =>
                {
                    gallery ??= await galleriesService.Get(c.GalleryId, cancellationToken);
                    return gallery?.Pictures.Any(x => x.Position == position) ?? true;
                }).WithMessage("Изображение в заданной позиции не найдено");
        }
    }
}
