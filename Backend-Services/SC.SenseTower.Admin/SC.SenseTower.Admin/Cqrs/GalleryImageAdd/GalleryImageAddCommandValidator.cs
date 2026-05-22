using FluentValidation;

namespace SC.SenseTower.Admin.Cqrs.GalleryImageAdd
{
    public class GalleryImageAddCommandValidator : AbstractValidator<GalleryImageAddCommand>
    {
        public GalleryImageAddCommandValidator()
        {
            RuleFor(x => x.GalleryId).NotEmpty().WithMessage("Не указана галерея");
            RuleFor(x => x.ImageId).NotEmpty().WithMessage("Не выбрано изображение");
            RuleFor(x => x.Name).NotEmpty().WithMessage("Не указано название");
            RuleFor(x => x.Author).NotEmpty().WithMessage("Не указан автор");
        }
    }
}
