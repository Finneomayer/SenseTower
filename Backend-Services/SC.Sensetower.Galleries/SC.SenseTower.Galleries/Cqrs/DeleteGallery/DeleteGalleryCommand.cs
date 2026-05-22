using MediatR;

namespace SC.SenseTower.Galleries.Cqrs.DeleteGallery
{
    public class DeleteGalleryCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
    }
}
