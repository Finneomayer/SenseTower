using MediatR;
using SC.SenseTower.Galleries.Dto.Galleries;

namespace SC.SenseTower.Galleries.Cqrs.GalleryImages
{
    public class GalleryImagesRequest : IRequest<Dictionary<int, GalleryImageDto>>
    {
        public Guid Id { get; set; }

        public string AccessToken { get; set; } = null!;
    }
}
