using MediatR;
using SC.SenseTower.Images.Dto.Images;

namespace SC.SenseTower.Images.Cqrs.ImagesByIds
{
    public class ImagesByIdsRequest : IRequest<IEnumerable<ImageInfoDto>>
    {
        public Guid[] ImageIds { get; set; } = Array.Empty<Guid>();

        public string RequestUrl { get; set; } = null!;
    }
}
