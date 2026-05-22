using MediatR;
using SC.SenseTower.Spaces.Dto.Spaces;

namespace SC.SenseTower.Spaces.Cqrs.Requests
{
    public class SpaceRequest : IRequest<SpaceDto>
    {
        public Guid SpaceId { get; set; }
    }
}
