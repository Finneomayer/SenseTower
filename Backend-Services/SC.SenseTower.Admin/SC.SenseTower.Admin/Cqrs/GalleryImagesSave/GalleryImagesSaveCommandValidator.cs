using FluentValidation;

namespace SC.SenseTower.Admin.Cqrs.GalleryImagesSave
{
    public class GalleryImagesSaveCommandValidator : AbstractValidator<GalleryImagesSaveCommand>
    {
        public GalleryImagesSaveCommandValidator()
        {
            RuleFor(x => x.GalleryId).NotEmpty().WithMessage("Не указана галерея");
            RuleFor(x => x.AccessToken).NotEmpty().WithMessage("Недостаточно прав для выполнения операции");
        }
    }
}
