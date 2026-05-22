using MediatR;
using SC.SenseTower.Common.Models;

namespace SC.SenseTower.MyPlaces.Cqrs.Requests
{
    public class LookupByOwnerRequest : IRequest<IEnumerable<LookupItemDto<Guid>>>
    {
        public Guid OwnerId { get; set; }
    }
}
