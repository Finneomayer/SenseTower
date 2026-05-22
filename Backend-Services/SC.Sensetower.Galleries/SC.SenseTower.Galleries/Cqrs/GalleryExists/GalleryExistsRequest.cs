using MediatR;

namespace SC.SenseTower.Galleries.Cqrs.GalleryExists
{
    public class GalleryExistsRequest : IRequest<bool>
    {
        public Guid Id { get; set; }
    }
}
