using AutoMapper;
using MediatR;
using SC.SenseTower.Admin.Dto.TowerEvents;
using SC.SenseTower.Admin.Services;
using SC.SenseTower.Common.Cqrs.Handlers;

namespace SC.SenseTower.Admin.Cqrs.TowerEventUpdate
{
    public class TowerEventUpdateCommandHandler : BaseHandler, IRequestHandler<TowerEventUpdateCommand, Unit>
    {
        private readonly TowerEventsService towerEventsService;

        public TowerEventUpdateCommandHandler(
            ILogger<TowerEventUpdateCommandHandler> logger,
            IMapper mapper,
            TowerEventsService towerEventsService) : base(logger, mapper)
        {
            this.towerEventsService = towerEventsService;
        }

        public async Task<Unit> Handle(TowerEventUpdateCommand request, CancellationToken cancellationToken)
        {
            var towerEvent = Mapper.Map<TowerEventUpdateRequestDto>(request);
            await towerEventsService.Update(request.AccessToken, request.RefreshToken, towerEvent, cancellationToken);
            return Unit.Value;
        }
    }
}
