using MediatR;
using SC.SenseTower.Galleries.Dto.Galleries;

namespace SC.SenseTower.Galleries.Cqrs.GalleryBySpaceId
{
    public class GalleryBySpaceIdRequest : IRequest<GalleryDto?>
    {
        public Guid SpaceId { get; set; }
    }
}
