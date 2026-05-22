using FluentValidation;
using SC.SenseTower.Galleries.Services;

namespace SC.SenseTower.Galleries.Cqrs.GalleryImages
{
    public class GalleryImagesRequestValidator : AbstractValidator<GalleryImagesRequest>
    {
        public GalleryImagesRequestValidator(GalleriesService galleriesService)
        {
            RuleFor(x => x.Id)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Не указана галерея")
                .MustAsync(async (c, id, cancellationToken) =>
                {
                    var gallery = await galleriesService.Get(id, cancellationToken);
                    return gallery != null;
                }).WithMessage("Галерея не найдена");
            RuleFor(x => x.AccessToken).NotEmpty().WithMessage("Недостаточно прав для выполнения операции");
        }
    }
}
