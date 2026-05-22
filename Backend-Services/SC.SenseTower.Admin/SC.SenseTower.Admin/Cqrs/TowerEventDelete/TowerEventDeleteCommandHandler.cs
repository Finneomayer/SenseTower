using AutoMapper;
using MediatR;
using SC.SenseTower.Admin.Services;
using SC.SenseTower.Common.Cqrs.Handlers;

namespace SC.SenseTower.Admin.Cqrs.TowerEventDelete
{
    public class TowerEventDeleteCommandHandler : BaseHandler, IRequestHandler<TowerEventDeleteCommand, Unit>
    {
        private readonly TowerEventsService towerEventsService;

        public TowerEventDeleteCommandHandler(
            ILogger<TowerEventDeleteCommandHandler> logger,
            IMapper mapper,
            TowerEventsService towerEventsService) : base(logger, mapper)
        {
            this.towerEventsService = towerEventsService;
        }

        public async Task<Unit> Handle(TowerEventDeleteCommand request, CancellationToken cancellationToken)
        {
            await towerEventsService.Delete(request.AccessToken, request.RefreshToken, request.EventId, cancellationToken);
            return Unit.Value;
        }
    }
}
