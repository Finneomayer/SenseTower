using MediatR;

namespace SC.SenseTower.Admin.Cqrs.GalleryImageAdd
{
    public class GalleryImageAddCommand : IRequest<Unit>
    {
        public Guid GalleryId { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public string Author { get; set; } = null!;

        public Guid ImageId { get; set; }

        public decimal PictureWidthInMeters { get; set; }

        public decimal PassepartoutWidthInMeters { get; set; }

        public string AccessToken { get; set; } = null!;

        public string RefreshToken { get; set; } = null!;
    }
}
