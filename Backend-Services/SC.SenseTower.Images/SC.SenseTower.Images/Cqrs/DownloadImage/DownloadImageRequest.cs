using MediatR;
using SC.SenseTower.Images.Dto.Images;

namespace SC.SenseTower.Images.Cqrs.DownloadImage
{
    public class DownloadImageRequest : BaseRequest, IRequest<DownloadImageDto>
    {
        public bool IsPreview { get; set; }
    }
}
