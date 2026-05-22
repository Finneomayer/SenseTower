using AutoMapper;
using MediatR;
using SC.SenseTower.Common.Cqrs.Handlers;
using SC.SenseTower.MyPlaces.Data.Models;
using SC.SenseTower.MyPlaces.Dto.Places;
using SC.SenseTower.MyPlaces.Services;

namespace SC.SenseTower.MyPlaces.RabbitMQ.SpaceUpdate
{
    public class SpaceUpdateCommandHandler : BaseHandler, IRequestHandler<SpaceUpdateCommand, Unit>
    {
        private readonly PlacesService placesService;
        private readonly RabbitMQService rabbitMQService;

        public SpaceUpdateCommandHandler(
            ILogger<SpaceUpdateCommandHandler> logger,
            IMapper mapper,
            PlacesService placesService,
            RabbitMQService rabbitMQService) : base(logger, mapper)
        {
            this.placesService = placesService;
            this.rabbitMQService = rabbitMQService;
        }

        public async Task<Unit> Handle(SpaceUpdateCommand request, CancellationToken cancellationToken)
        {
            var space = Mapper.Map<Space>(request);
            var place = await placesService.GetBySpaceId(space.Id, cancellationToken);
            if (place != null)
            {
                place.Space = space;
                await placesService.Update(place, cancellationToken);
            }
            var placeDto = Mapper.Map<PlaceDto>(place);
            await rabbitMQService.SendUpdatePlaceMessage(placeDto, cancellationToken);
            return Unit.Value;
        }
    }
}
