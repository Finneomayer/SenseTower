using MediatR;

namespace SC.SenseTower.Admin.Cqrs.GalleryImageRemove
{
    public class GalleryImageRemoveCommand : IRequest<Unit>
    {
        public Guid GalleryId { get; set; }

        public int Position { get; set; }

        public string AccessToken { get; set; } = null!;

        public string RefreshToken { get; set; } = null!;
    }
}
