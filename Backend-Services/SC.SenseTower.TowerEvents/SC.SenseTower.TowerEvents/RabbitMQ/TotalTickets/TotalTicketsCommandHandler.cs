using AutoMapper;
using MediatR;
using SC.SenseTower.Common.Cqrs.Handlers;
using SC.SenseTower.TowerEvents.Services;

namespace SC.SenseTower.TowerEvents.RabbitMQ.TotalTickets
{
    public class TotalTicketsCommandHandler : BaseHandler, IRequestHandler<TotalTicketsCommand, Unit>
    {
        private readonly TowerEventsService towerEventsService;

        public TotalTicketsCommandHandler(
            ILogger<TotalTicketsCommandHandler> logger,
            IMapper mapper,
            TowerEventsService towerEventsService) : base(logger, mapper)
        {
            this.towerEventsService = towerEventsService;
        }

        public async Task<Unit> Handle(TotalTicketsCommand request, CancellationToken cancellationToken)
        {
            await towerEventsService.SetTotalTickets(request.EventId, request.TotalTickets, cancellationToken);
            return Unit.Value;
        }
    }
}
