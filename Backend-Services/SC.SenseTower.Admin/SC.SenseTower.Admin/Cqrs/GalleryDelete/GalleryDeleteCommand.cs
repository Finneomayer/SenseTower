using MediatR;

namespace SC.SenseTower.Admin.Cqrs.GalleryDelete
{
    public class GalleryDeleteCommand : IRequest<Unit>
    {
        public Guid GalleryId { get; set; }

        public string AccessToken { get; set; } = null!;

        public string RefreshToken { get; set; } = null!;
    }
}
