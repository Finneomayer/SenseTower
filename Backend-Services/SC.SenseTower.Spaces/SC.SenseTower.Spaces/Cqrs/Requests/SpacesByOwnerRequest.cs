using MediatR;
using SC.SenseTower.Spaces.Dto.Spaces;

namespace SC.SenseTower.Spaces.Cqrs.Requests
{
    public class SpacesByOwnerRequest : IRequest<IEnumerable<SpaceDto>>
    {
        public Guid UserId { get; set; }
    }
}
