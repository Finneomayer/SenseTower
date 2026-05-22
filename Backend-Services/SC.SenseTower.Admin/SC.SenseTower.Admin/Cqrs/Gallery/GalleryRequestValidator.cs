using FluentValidation;
using SC.SenseTower.Admin.Services;

namespace SC.SenseTower.Admin.Cqrs.Gallery
{
    public class GalleryRequestValidator : AbstractValidator<GalleryRequest>
    {
        public GalleryRequestValidator(GalleriesService galleriesService)
        {
            RuleFor(x => x.Id)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Не выбрана галерея")
                .MustAsync(async (c, id, cancellationToken) =>
                {
                    return await galleriesService.Exists(c.AccessToken, id, cancellationToken);
                }).WithMessage("Галерея не найдена");
            RuleFor(x => x.AccessToken).NotEmpty().WithMessage("Недостаточно прав для редактирования галереи");
        }
    }
}
