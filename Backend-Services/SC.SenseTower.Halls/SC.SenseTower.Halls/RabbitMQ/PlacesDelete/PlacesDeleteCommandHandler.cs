using AutoMapper;
using MediatR;
using SC.SenseTower.Common.Cqrs.Handlers;
using SC.SenseTower.Halls.Services;

namespace SC.SenseTower.Halls.RabbitMQ.PlacesDelete
{
    public class PlacesDeleteCommandHandler : BaseHandler, IRequestHandler<PlacesDeleteCommand, Unit>
    {
        private readonly HallsService hallsService;

        public PlacesDeleteCommandHandler(
            ILogger<PlacesDeleteCommandHandler> logger,
            IMapper mapper,
            HallsService hallsService) : base(logger, mapper)
        {
            this.hallsService = hallsService;
        }

        public async Task<Unit> Handle(PlacesDeleteCommand request, CancellationToken cancellationToken)
        {
            await hallsService.DeletePlaces(request.PlaceIds, cancellationToken);
            return Unit.Value;
        }
    }
}
