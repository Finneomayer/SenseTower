using AutoMapper;
using MediatR;
using SC.SenseTower.Common.Cqrs.Handlers;
using SC.SenseTower.TowerEvents.Dto.TowerEvents;
using SC.SenseTower.TowerEvents.Services;

namespace SC.SenseTower.TowerEvents.Cqrs.TowerEventList
{
    public class TowerEventListRequestHandler : BaseHandler, IRequestHandler<TowerEventListRequest, TowerEventListItemDto[]>
    {
        private readonly TowerEventsService towerEventsService;

        public TowerEventListRequestHandler(
            ILogger<TowerEventListRequestHandler> logger,
            IMapper mapper,
            TowerEventsService towerEventsService) : base(logger, mapper)
        {
            this.towerEventsService = towerEventsService;
        }

        public async Task<TowerEventListItemDto[]> Handle(TowerEventListRequest request, CancellationToken cancellationToken)
        {
            var data = await towerEventsService.GetList(request, null, cancellationToken);
            var result = Mapper.Map<TowerEventListItemDto[]>(data);
            return result;
        }
    }
}
