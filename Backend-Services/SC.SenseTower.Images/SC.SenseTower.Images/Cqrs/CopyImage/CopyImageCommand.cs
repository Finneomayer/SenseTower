using MediatR;
using SC.SenseTower.Images.Dto.Images;

namespace SC.SenseTower.Images.Cqrs.CopyImage
{
    public class CopyImageCommand : IRequest<ImageInfoDto>
    {
        public Guid Id { get; set; }

        public Guid? UserId { get; set; }
    }
}
