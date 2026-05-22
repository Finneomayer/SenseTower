using FluentValidation;

namespace SC.SenseTower.Admin.Cqrs.GalleryDelete
{
    public class GalleryDeleteCommandValidator : AbstractValidator<GalleryDeleteCommand>
    {
        public GalleryDeleteCommandValidator()
        {
            RuleFor(x => x.GalleryId).NotEmpty().WithMessage("Не указана галерея");
            RuleFor(x => x.AccessToken).NotEmpty().WithMessage("Недостаточно прав для удаления галереи");
        }
    }
}
