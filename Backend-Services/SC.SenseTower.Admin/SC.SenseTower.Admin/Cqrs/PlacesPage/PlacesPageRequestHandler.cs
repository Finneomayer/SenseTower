using AutoMapper;
using MediatR;
using SC.SenseTower.Admin.Dto.Places;
using SC.SenseTower.Admin.Services;
using SC.SenseTower.Common.Cqrs.Handlers;
using SC.SenseTower.Common.Models;

namespace SC.SenseTower.Admin.Cqrs.PlacesPage
{
    public class PlacesPageRequestHandler : BaseHandler, IRequestHandler<PlacesPageRequest, PagedDataDto<PlacesGridItemDto>>
    {
        private readonly PlacesService placesService;

        public PlacesPageRequestHandler(
            ILogger<PlacesPageRequestHandler> logger,
            IMapper mapper,
            PlacesService placesService) : base(logger, mapper)
        {
            this.placesService = placesService;
        }

        public async Task<PagedDataDto<PlacesGridItemDto>> Handle(PlacesPageRequest request, CancellationToken cancellationToken)
        {
            var data = await placesService.GetPage(request.AccessToken, request.RefreshToken, request.Filters, request.Sorting, request.Page, request.PageSize, cancellationToken);
            var result = new PagedDataDto<PlacesGridItemDto>
            {
                Items = Mapper.Map<PlacesGridItemDto[]>(data.Items),
                TotalCount = data.TotalCount
            };
            return result;
        }
    }
}
