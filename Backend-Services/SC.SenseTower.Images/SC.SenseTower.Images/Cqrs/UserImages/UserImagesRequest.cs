using MediatR;
using SC.SenseTower.Images.Dto.Images;

namespace SC.SenseTower.Images.Cqrs.UserImages
{
    public class UserImagesRequest : IRequest<IEnumerable<ImageListItemDto>>
    {
        public Guid OwnerId { get; set; }

        public string? AccessToken { get; set; }

        public string RequestUrl { get; set; } = null!;

        public string Role { get; set; } = null!;

        public Guid? UserId { get; set; }
    }
}
