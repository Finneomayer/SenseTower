using MediatR;

namespace SC.SenseTower.Admin.Cqrs.GalleryCreate
{
    public class GalleryCreateCommand : IRequest<Guid?>
    {
        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public bool IsVisible { get; set; }

        public Guid? ImageId { get; set; }

        public Guid? SpaceId { get; set; }

        public string AccessToken { get; set; } = null!;

        public string RefreshToken { get; set; } = null!;
    }
}
