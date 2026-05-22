using AutoMapper;
using MediatR;
using SC.SenseTower.Common.Cqrs.Handlers;
using SC.SenseTower.TowerEvents.Dto.TowerEvents;
using SC.SenseTower.TowerEvents.Services;

namespace SC.SenseTower.TowerEvents.Cqrs.TowerEvent
{
    public class TowerEventRequestHandler : BaseHandler, IRequestHandler<TowerEventRequest, TowerEventDto?>
    {
        private readonly TowerEventsService towerEventsService;

        public TowerEventRequestHandler(
            ILogger<TowerEventRequestHandler> logger,
            IMapper mapper,
            TowerEventsService towerEventsService) : base(logger, mapper)
        {
            this.towerEventsService = towerEventsService;
        }

        public async Task<TowerEventDto?> Handle(TowerEventRequest request, CancellationToken cancellationToken)
        {
            var towerEvent = await towerEventsService.Get(request.EventId, cancellationToken);
            var result = Mapper.Map<TowerEventDto>(towerEvent);
            return result;
        }
    }
}
