using AutoMapper;
using MediatR;
using SC.SenseTower.Admin.Dto.TowerEvents;
using SC.SenseTower.Admin.Services;
using SC.SenseTower.Common.Cqrs.Handlers;
using SC.SenseTower.Common.Models;

namespace SC.SenseTower.Admin.Cqrs.TowerEventsPage
{
    public class TowerEventsPageRequestHandler : BaseHandler, IRequestHandler<TowerEventsPageRequest, PagedDataDto<TowerEventGridItemDto>>
    {
        private readonly TowerEventsService towerEventsService;

        public TowerEventsPageRequestHandler(
            ILogger<TowerEventsPageRequestHandler> logger,
            IMapper mapper,
            TowerEventsService towerEventsService) : base(logger, mapper)
        {
            this.towerEventsService = towerEventsService;
        }

        public async Task<PagedDataDto<TowerEventGridItemDto>> Handle(TowerEventsPageRequest request, CancellationToken cancellationToken)
        {
            var data = await towerEventsService.GetPagedList(request.AccessToken, request.Filters, request.Sorting, request.Page, request.PageSize, cancellationToken);
            var items = Mapper.Map<TowerEventGridItemDto[]>(data.Items);
            var result = new PagedDataDto<TowerEventGridItemDto>
            {
                Items = items,
                TotalCount = data.TotalCount
            };
            return await Task.FromResult(result);
        }
    }
}
