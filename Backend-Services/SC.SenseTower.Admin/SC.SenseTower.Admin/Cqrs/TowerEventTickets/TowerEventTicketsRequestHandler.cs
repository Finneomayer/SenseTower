using AutoMapper;
using MediatR;
using SC.SenseTower.Admin.Dto.TowerEvents;
using SC.SenseTower.Admin.Services;
using SC.SenseTower.Common.Cqrs.Handlers;

namespace SC.SenseTower.Admin.Cqrs.TowerEventTickets
{
    public class TowerEventTicketsRequestHandler : BaseHandler, IRequestHandler<TowerEventTicketsRequest, IEnumerable<TowerEventTicketDto>>
    {
        private readonly TicketsService ticketsService;

        public TowerEventTicketsRequestHandler(
            ILogger<TowerEventTicketsRequestHandler> logger,
            IMapper mapper,
            TicketsService ticketsService) : base(logger, mapper)
        {
            this.ticketsService = ticketsService;
        }

        public async Task<IEnumerable<TowerEventTicketDto>> Handle(TowerEventTicketsRequest request, CancellationToken cancellationToken)
        {
            var result = await ticketsService.GetSold(request.AccessToken, request.RefreshToken, request.Id, cancellationToken);
            return result;
        }
    }
}
