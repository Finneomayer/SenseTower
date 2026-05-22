using AutoMapper;
using MediatR;
using SC.SenseTower.Common.Cqrs.Handlers;
using SC.SenseTower.TowerEvents.Dto.TowerEvents;
using SC.SenseTower.TowerEvents.Services;

namespace SC.SenseTower.TowerEvents.Cqrs.TowerEventTopList
{
    public class TowerEventTopListRequestHandler : BaseHandler, IRequestHandler<TowerEventTopListRequest, IEnumerable<TowerEventListItemDto>>
    {
        private readonly TowerEventsService towerEventsService;

        public TowerEventTopListRequestHandler(
            ILogger<TowerEventTopListRequestHandler> logger,
            IMapper mapper,
            TowerEventsService towerEventsService) : base(logger, mapper)
        {
            this.towerEventsService = towerEventsService;
        }

        public async Task<IEnumerable<TowerEventListItemDto>> Handle(TowerEventTopListRequest request, CancellationToken cancellationToken)
        {
            var data = await towerEventsService.GetList(null, request.Limit, cancellationToken);
            var result = Mapper.Map<TowerEventListItemDto[]>(data);
            return result;
        }
    }
}
