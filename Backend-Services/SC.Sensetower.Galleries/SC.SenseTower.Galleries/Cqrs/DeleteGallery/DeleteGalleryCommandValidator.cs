using FluentValidation;
using SC.SenseTower.Galleries.Services;

namespace SC.SenseTower.Galleries.Cqrs.DeleteGallery
{
    public class DeleteGalleryCommandValidator : AbstractValidator<DeleteGalleryCommand>
    {
        public DeleteGalleryCommandValidator(GalleriesService galleriesService)
        {
            RuleFor(x => x.Id)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Не указан идентификатор галереи")
                .MustAsync(async (c, id, cancellationToken) =>
                {
                    var gallery = await galleriesService.Get(id, cancellationToken);
                    return gallery != null;
                }).WithMessage("Галерея не найдена");
        }
    }
}
