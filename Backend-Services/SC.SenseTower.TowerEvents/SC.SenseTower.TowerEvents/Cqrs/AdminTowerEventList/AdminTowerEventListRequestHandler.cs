using AutoMapper;
using MediatR;
using SC.SenseTower.Common.Cqrs.Handlers;
using SC.SenseTower.Common.Models;
using SC.SenseTower.TowerEvents.Dto.TowerEvents;
using SC.SenseTower.TowerEvents.Services;

namespace SC.SenseTower.TowerEvents.Cqrs.AdminTowerEventList
{
    public class AdminTowerEventListRequestHandler : BaseHandler, IRequestHandler<AdminTowerEventListRequest, PagedDataDto<TowerEventListItemDto>>
    {
        private readonly TowerEventsService towerEventsService;

        public AdminTowerEventListRequestHandler(
            ILogger<AdminTowerEventListRequestHandler> logger,
            IMapper mapper,
            TowerEventsService towerEventsService) : base(logger, mapper)
        {
            this.towerEventsService = towerEventsService;
        }

        public async Task<PagedDataDto<TowerEventListItemDto>> Handle(AdminTowerEventListRequest request, CancellationToken cancellationToken)
        {
            var data = await towerEventsService.Get(request.Filters, request.Sorting, request.Offset, request.PageSize, cancellationToken);
            var items = Mapper.Map<TowerEventListItemDto[]>(data);
            var result = new PagedDataDto<TowerEventListItemDto>
            {
                Items = items,
                TotalCount = await towerEventsService.Count(request.Filters, cancellationToken)
            };
            return result;

        }
    }
}
