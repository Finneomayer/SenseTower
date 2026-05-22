using AutoMapper;
using MediatR;
using SC.SenseTower.Common.Cqrs.Handlers;
using SC.SenseTower.MyPlaces.Cqrs.Requests;
using SC.SenseTower.MyPlaces.Services;

namespace SC.SenseTower.MyPlaces.Cqrs.Handlers
{
    public class AllocatedSpaceIdsRequestHandler : BaseHandler, IRequestHandler<AllocatedSpaceIdsRequest, IEnumerable<Guid>>
    {
        private readonly PlacesService placesService;

        public AllocatedSpaceIdsRequestHandler(
            ILogger<AllocatedSpaceIdsRequestHandler> logger,
            IMapper mapper,
            PlacesService placesService) : base(logger, mapper)
        {
            this.placesService = placesService;
        }

        public async Task<IEnumerable<Guid>> Handle(AllocatedSpaceIdsRequest request, CancellationToken cancellationToken)
        {
            var result = await placesService.GetAllocatedSpaceIds(cancellationToken);
            return result ?? Array.Empty<Guid>();
        }
    }
}
