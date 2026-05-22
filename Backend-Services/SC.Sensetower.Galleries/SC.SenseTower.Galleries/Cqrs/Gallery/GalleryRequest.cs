using MediatR;
using SC.SenseTower.Galleries.Dto.Galleries;

namespace SC.SenseTower.Galleries.Cqrs.Gallery
{
    public class GalleryRequest : IRequest<GalleryDto?>
    {
        public Guid Id { get; set; }

        public string AccessToken { get; set; } = null!;
    }
}
