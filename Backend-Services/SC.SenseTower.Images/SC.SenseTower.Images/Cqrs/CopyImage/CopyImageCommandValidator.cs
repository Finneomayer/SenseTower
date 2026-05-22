using FluentValidation;
using SC.SenseTower.Images.Services;

namespace SC.SenseTower.Images.Cqrs.CopyImage
{
    public class CopyImageCommandValidator : AbstractValidator<CopyImageCommand>
    {
        public CopyImageCommandValidator(ImageFilesService imageFilesService)
        {
            RuleFor(x => x.Id)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Не указано изображение")
                .MustAsync(async (c, id, cancellationToken) =>
                {
                    var imageFile = await imageFilesService.Get(id, cancellationToken);
                    return imageFile != null;
                }).WithMessage("Изображение не найдено");
        }
    }
}
