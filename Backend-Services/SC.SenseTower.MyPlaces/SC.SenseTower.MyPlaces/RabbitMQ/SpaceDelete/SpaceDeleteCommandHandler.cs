using AutoMapper;
using MediatR;
using SC.SenseTower.Common.Cqrs.Handlers;
using SC.SenseTower.MyPlaces.Dto.Places;
using SC.SenseTower.MyPlaces.Services;

namespace SC.SenseTower.MyPlaces.RabbitMQ.SpaceDelete
{
    public class SpaceDeleteCommandHandler : BaseHandler, IRequestHandler<SpaceDeleteCommand, Unit>
    {
        private readonly PlacesService placesService;
        private readonly RabbitMQService rabbitMQService;

        public SpaceDeleteCommandHandler(
            ILogger<SpaceDeleteCommandHandler> logger,
            IMapper mapper,
            PlacesService placesService,
            RabbitMQService rabbitMQService) : base(logger, mapper)
        {
            this.placesService = placesService;
            this.rabbitMQService = rabbitMQService;
        }

        public async Task<Unit> Handle(SpaceDeleteCommand request, CancellationToken cancellationToken)
        {
            var place = await placesService.GetBySpaceId(request.SpaceId, cancellationToken);
            if (place != null)
            {
                place.Space = null;
                await placesService.Update(place, cancellationToken);
                var placeDto = Mapper.Map<PlaceDto>(place);
                await rabbitMQService.SendUpdatePlaceMessage(placeDto, cancellationToken);
            }
            return Unit.Value;
        }
    }
}
