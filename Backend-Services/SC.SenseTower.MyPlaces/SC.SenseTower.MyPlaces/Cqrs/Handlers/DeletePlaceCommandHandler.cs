using AutoMapper;
using MediatR;
using SC.SenseTower.Common.Cqrs.Handlers;
using SC.SenseTower.MyPlaces.Cqrs.Commands;
using SC.SenseTower.MyPlaces.RabbitMQ;
using SC.SenseTower.MyPlaces.Services;

namespace SC.SenseTower.MyPlaces.Cqrs.Handlers
{
    public class DeletePlaceCommandHandler : BaseHandler, IRequestHandler<DeletePlaceCommand, Unit>
    {
        private readonly PlacesService placesService;
        private readonly RabbitMQService rabbitMQService;

        public DeletePlaceCommandHandler(
            ILogger<DeletePlaceCommandHandler> logger,
            IMapper mapper,
            PlacesService placesService,
            RabbitMQService rabbitMQService) : base(logger, mapper)
        {
            this.placesService = placesService;
            this.rabbitMQService = rabbitMQService;
        }

        public async Task<Unit> Handle(DeletePlaceCommand request, CancellationToken cancellationToken)
        {
            await placesService.Delete(request.AccessToken, request.PlaceId, cancellationToken);
            await rabbitMQService.SendDeletePlacesMessage(new Guid[] { request.PlaceId }, cancellationToken);
            return Unit.Value;
        }
    }
}
