using MediatR;
using SC.SenseTower.Admin.Dto.Galleries;

namespace SC.SenseTower.Admin.Cqrs.GalleryImages
{
    public class GalleryImagesRequest : IRequest<IEnumerable<GalleryImageDto>>
    {
        public Guid GalleryId { get; set; }

        public string? AccessToken { get; set; }

        public string? RefreshToken { get; set; }
    }
}
