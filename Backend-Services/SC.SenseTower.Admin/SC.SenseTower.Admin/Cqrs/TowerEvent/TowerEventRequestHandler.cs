using AutoMapper;
using MediatR;
using SC.SenseTower.Admin.Dto.TowerEvents;
using SC.SenseTower.Admin.Services;
using SC.SenseTower.Common.Cqrs.Handlers;

namespace SC.SenseTower.Admin.Cqrs.TowerEvent
{
    public class TowerEventRequestHandler : BaseHandler, IRequestHandler<TowerEventRequest, TowerEventDto>
    {
        private readonly TowerEventsService towerEventsService;
        private readonly SpacesService spacesService;
        private readonly ImagesService imagesService;

        public TowerEventRequestHandler(
            ILogger<TowerEventRequestHandler> logger,
            IMapper mapper,
            TowerEventsService towerEventsService,
            SpacesService spacesService,
            ImagesService imagesService) : base(logger, mapper)
        {
            this.towerEventsService = towerEventsService;
            this.spacesService = spacesService;
            this.imagesService = imagesService;
        }

        public async Task<TowerEventDto> Handle(TowerEventRequest request, CancellationToken cancellationToken)
        {
            var towerEvent = await towerEventsService.Get(request.AccessToken, request.Id, cancellationToken);
            var result = Mapper.Map<TowerEventDto>(towerEvent);
            result.AvailableImages = await imagesService.Lookup(request.AccessToken, null, cancellationToken);
            result.AvailableSpaces = await spacesService.Lookup(request.AccessToken, null, cancellationToken);
            return result;
        }
    }
}
