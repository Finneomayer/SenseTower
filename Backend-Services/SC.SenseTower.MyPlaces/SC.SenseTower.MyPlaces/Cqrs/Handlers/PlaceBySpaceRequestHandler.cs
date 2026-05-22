using AutoMapper;
using MediatR;
using SC.SenseTower.Common.Cqrs.Handlers;
using SC.SenseTower.MyPlaces.Cqrs.Requests;
using SC.SenseTower.MyPlaces.Dto.Places;
using SC.SenseTower.MyPlaces.Services;

namespace SC.SenseTower.MyPlaces.Cqrs.Handlers
{
    public class PlaceBySpaceRequestHandler : BaseHandler, IRequestHandler<PlaceBySpaceRequest, PlaceDto?>
    {
        private readonly PlacesService placesService;

        public PlaceBySpaceRequestHandler(
            ILogger<PlaceBySpaceRequestHandler> logger,
            IMapper mapper,
            PlacesService placesService) : base(logger, mapper)
        {
            this.placesService = placesService;
        }

        public async Task<PlaceDto?> Handle(PlaceBySpaceRequest request, CancellationToken cancellationToken)
        {
            var data = await placesService.GetBySpaceId(request.SpaceId, cancellationToken);
            var result = Mapper.Map<PlaceDto?>(data);
            return result;
        }
    }
}
