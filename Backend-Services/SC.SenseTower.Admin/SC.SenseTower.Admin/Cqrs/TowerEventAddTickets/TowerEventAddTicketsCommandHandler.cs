using AutoMapper;
using MediatR;
using SC.SenseTower.Admin.Services;
using SC.SenseTower.Common.Cqrs.Handlers;

namespace SC.SenseTower.Admin.Cqrs.TowerEventAddTickets
{
    public class TowerEventAddTicketsCommandHandler : BaseHandler, IRequestHandler<TowerEventAddTicketsCommand, Unit>
    {
        private readonly TicketsService ticketsService;

        public TowerEventAddTicketsCommandHandler(
            ILogger<TowerEventAddTicketsCommandHandler> logger,
            IMapper mapper,
            TicketsService ticketsService) : base(logger, mapper)
        {
            this.ticketsService = ticketsService;
        }

        public async Task<Unit> Handle(TowerEventAddTicketsCommand request, CancellationToken cancellationToken)
        {
            await ticketsService.Create(request.AccessToken, request.RefreshToken, request.EventId, request.Quantity, cancellationToken);
            return Unit.Value;
        }
    }
}
