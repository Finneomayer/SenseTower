using MediatR;

namespace SC.SenseTower.Admin.Cqrs.GalleryUpdate
{
    public class GalleryUpdateCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public bool IsVisible { get; set; }

        public Guid? ImageId { get; set; }

        public Guid? SpaceId { get; set; }

        public string AccessToken { get; set; } = null!;

        public string RefreshToken { get; set; } = null!;
    }
}
