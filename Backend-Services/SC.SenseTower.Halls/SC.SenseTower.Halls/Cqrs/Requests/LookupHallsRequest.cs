using MediatR;
using SC.SenseTower.Common.Models;

namespace SC.SenseTower.Halls.Cqrs.Requests
{
    public class LookupHallsRequest : IRequest<IEnumerable<LookupItemDto<Guid>>>
    {
        public Guid[] HallIds { get; set; } = Array.Empty<Guid>();
    }
}
