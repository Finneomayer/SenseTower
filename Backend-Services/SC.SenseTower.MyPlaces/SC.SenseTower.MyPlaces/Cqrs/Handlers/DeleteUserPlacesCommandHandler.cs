using AutoMapper;
using MediatR;
using SC.SenseTower.Common.Cqrs.Handlers;
using SC.SenseTower.MyPlaces.Cqrs.Commands;
using SC.SenseTower.MyPlaces.RabbitMQ;
using SC.SenseTower.MyPlaces.Services;

namespace SC.SenseTower.MyPlaces.Cqrs.Handlers
{
    public class DeleteUserPlacesCommandHandler : BaseHandler, IRequestHandler<DeleteUserPlacesCommand, bool>
    {
        private readonly PlacesService placesService;
        private readonly RabbitMQService rabbitMQService;

        public DeleteUserPlacesCommandHandler(
            ILogger<DeleteUserPlacesCommandHandler> logger,
            IMapper mapper,
            PlacesService placesService,
            RabbitMQService rabbitMQService) : base(logger, mapper)
        {
            this.placesService = placesService;
            this.rabbitMQService = rabbitMQService;
        }

        public async Task<bool> Handle(DeleteUserPlacesCommand request, CancellationToken cancellationToken)
        {
            var places = await placesService.GetByOwnerId(request.OwnerId, cancellationToken);
            var result = await placesService.DeleteByOwner(request.AccessToken, request.OwnerId, cancellationToken);
            var placesIds = places.Select(x => x.Id).ToArray();
            await rabbitMQService.SendDeletePlacesMessage(placesIds, cancellationToken);
            return result;
        }
    }
}
