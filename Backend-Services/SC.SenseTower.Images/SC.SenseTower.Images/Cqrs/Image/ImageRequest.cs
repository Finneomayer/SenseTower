using MediatR;
using SC.SenseTower.Images.Dto.Images;

namespace SC.SenseTower.Images.Cqrs.Image
{
    public class ImageRequest : BaseRequest, IRequest<ImageDto?>
    {
        public string RequestUrl { get; set; } = null!;
    }
}
