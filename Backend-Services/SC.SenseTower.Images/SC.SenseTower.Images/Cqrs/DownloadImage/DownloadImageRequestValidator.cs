using FluentValidation;
using SC.SenseTower.Images.Data.Models;
using SC.SenseTower.Images.Services;

namespace SC.SenseTower.Images.Cqrs.DownloadImage
{
    public class DownloadImageRequestValidator : AbstractValidator<DownloadImageRequest>
    {
        private ImageFile? imageFile = null;

        public DownloadImageRequestValidator(ImageFilesService imageFilesService)
        {
            RuleFor(x => x.Id)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Не указано изображение")
                .MustAsync(async (c, id, cancellationToken) =>
                {
                    imageFile ??= await imageFilesService.Get(id, cancellationToken);
                    return imageFile != null;
                }).WithMessage("Изображение не найдено");
        }
    }
}
