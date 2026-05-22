using AutoMapper;
using MediatR;
using SC.SenseTower.Common.Cqrs.Handlers;
using SC.SenseTower.TowerEvents.Services;

namespace SC.SenseTower.TowerEvents.Cqrs.TowerEventExists
{
    public class TowerEventExistsRequestHandler : BaseHandler, IRequestHandler<TowerEventExistsRequest, bool>
    {
        private readonly TowerEventsService towerEventsService;

        public TowerEventExistsRequestHandler(
            ILogger<TowerEventExistsRequestHandler> logger,
            IMapper mapper,
            TowerEventsService towerEventsService) : base(logger, mapper)
        {
            this.towerEventsService = towerEventsService;
        }

        public async Task<bool> Handle(TowerEventExistsRequest request, CancellationToken cancellationToken)
        {
            var result = await towerEventsService.Exists(request.EventId, cancellationToken);
            return result;
        }
    }
}
