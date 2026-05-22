using MediatR;
using SC.SenseTower.Admin.Dto.Images;

namespace SC.SenseTower.Admin.Cqrs.AvailableImages
{
    public class AvailableImagesRequest : IRequest<IEnumerable<ImageInfoDto>>
    {
        public string AccessToken { get; set; } = null!;

        public string RefreshToken { get; set; } = null!;
    }
}
