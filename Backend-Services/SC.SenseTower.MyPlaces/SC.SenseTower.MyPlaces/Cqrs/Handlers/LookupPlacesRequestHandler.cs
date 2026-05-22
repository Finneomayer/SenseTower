using AutoMapper;
using MediatR;
using SC.SenseTower.Common.Cqrs.Handlers;
using SC.SenseTower.Common.Models;
using SC.SenseTower.MyPlaces.Cqrs.Requests;
using SC.SenseTower.MyPlaces.Services;

namespace SC.SenseTower.MyPlaces.Cqrs.Handlers
{
    public class LookupPlacesRequestHandler : BaseHandler, IRequestHandler<LookupPlacesRequest, IEnumerable<LookupItemDto<Guid>>?>
    {
        private readonly PlacesService placesService;

        public LookupPlacesRequestHandler(
            ILogger<LookupPlacesRequestHandler> logger,
            IMapper mapper,
            PlacesService placesService) : base(logger, mapper)
        {
            this.placesService = placesService;
        }

        public async Task<IEnumerable<LookupItemDto<Guid>>?> Handle(LookupPlacesRequest request, CancellationToken cancellationToken)
        {
            var result = await placesService.Lookup(request.PlaceIds, cancellationToken);
            return result;
        }
    }
}
