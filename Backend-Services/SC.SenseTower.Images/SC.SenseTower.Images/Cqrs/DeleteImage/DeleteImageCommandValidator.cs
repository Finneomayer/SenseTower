using SC.SenseTower.Images.Services;

namespace SC.SenseTower.Images.Cqrs.DeleteImage
{
    public class DeleteImageCommandValidator : BaseValidator<DeleteImageCommand>
    {
        public DeleteImageCommandValidator(ImageFilesService imageFilesService) : base(imageFilesService) { }
    }
}
