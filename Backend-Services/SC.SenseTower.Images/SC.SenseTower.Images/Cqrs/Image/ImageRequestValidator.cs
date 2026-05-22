using SC.SenseTower.Images.Services;

namespace SC.SenseTower.Images.Cqrs.Image
{
    public class ImageRequestValidator : BaseValidator<ImageRequest>
    {
        public ImageRequestValidator(ImageFilesService imageFilesService) : base(imageFilesService) { }
    }
}
