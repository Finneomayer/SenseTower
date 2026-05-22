using FluentValidation;
using SC.SenseTower.Images.Services;

namespace SC.SenseTower.Images.Cqrs.UpdateImage
{
    public class UpdateImageCommandValidator : BaseValidator<UpdateImageCommand>
    {
        public UpdateImageCommandValidator(ImageFilesService imageFilesService) : base(imageFilesService)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Не задано название изображения");
        }
    }
}
