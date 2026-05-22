using AutoMapper;
using MediatR;
using SC.SenseTower.Admin.Dto.TowerEvents;
using SC.SenseTower.Admin.Services;
using SC.SenseTower.Common.Cqrs.Handlers;

namespace SC.SenseTower.Admin.Cqrs.TowerEventCreate
{
    public class TowerEventCreateCommandHandler : BaseHandler, IRequestHandler<TowerEventCreateCommand, Guid>
    {
        private readonly TowerEventsService towerEventsService;

        public TowerEventCreateCommandHandler(
            ILogger<TowerEventCreateCommandHandler> logger,
            IMapper mapper,
            TowerEventsService towerEventsService) : base(logger, mapper)
        {
            this.towerEventsService = towerEventsService;
        }

        public async Task<Guid> Handle(TowerEventCreateCommand request, CancellationToken cancellationToken)
        {
            var towerEvent = Mapper.Map<TowerEventCreateRequestDto>(request);
            var result = await towerEventsService.Create(request.AccessToken, request.RefreshToken, towerEvent, cancellationToken);
            return result;
        }
    }
}
