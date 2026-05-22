using MediatR;
using SC.SenseTower.Admin.Dto.Galleries;

namespace SC.SenseTower.Admin.Cqrs.GalleryImagesSave
{
    public class GalleryImagesSaveCommand : IRequest<Unit>
    {
        public Guid GalleryId { get; set; }

        public GalleryImageDto[] Images { get; set; } = Array.Empty<GalleryImageDto>();

        public string AccessToken { get; set; } = null!;

        public string RefreshToken { get; set; } = null!;
    }
}
