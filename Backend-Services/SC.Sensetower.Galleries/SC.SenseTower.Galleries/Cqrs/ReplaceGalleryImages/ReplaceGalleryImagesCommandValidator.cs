using FluentValidation;

namespace SC.SenseTower.Galleries.Cqrs.ReplaceGalleryImages
{
    public class ReplaceGalleryImagesCommandValidator : AbstractValidator<ReplaceGalleryImagesCommand>
    {
        public ReplaceGalleryImagesCommandValidator()
        {
            RuleFor(x => x.GalleryId).NotEmpty().WithMessage("Не указана галерея");
            RuleFor(x => x.AccessToken).NotEmpty().WithMessage("Недостаточно прав для совершения операции");
            RuleFor(x => x.Images)
                .Must((c, images) =>
                {
                    var duplicatesExist = images
                        .GroupBy(x => x.Position)
                        .Select(x => x.Count())
                        .Any(x => x > 1);
                    return !duplicatesExist;
                }).WithMessage("На одной позиции находятся разные изображения");
        }
    }
}
