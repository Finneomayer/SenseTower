using AutoMapper;
using MediatR;
using SC.SenseTower.Common.Cqrs.Handlers;
using SC.SenseTower.TowerEvents.Data.Models;
using SC.SenseTower.TowerEvents.Services;

namespace SC.SenseTower.TowerEvents.RabbitMQ.SpaceUpdate
{
    public class SpaceUpdateCommandHandler : BaseHandler, IRequestHandler<SpaceUpdateCommand, Unit>
    {
        private readonly TowerEventsService towerEventsService;

        public SpaceUpdateCommandHandler(
            ILogger<SpaceUpdateCommandHandler> logger,
            IMapper mapper,
            TowerEventsService towerEventsService) : base(logger, mapper)
        {
            this.towerEventsService = towerEventsService;
        }

        public async Task<Unit> Handle(SpaceUpdateCommand request, CancellationToken cancellationToken)
        {
            var space = Mapper.Map<Space>(request);
            await towerEventsService.UpdateSpace(space, cancellationToken);
            return Unit.Value;
        }
    }
}
