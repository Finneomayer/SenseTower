using AutoMapper;
using MediatR;
using SC.SenseTower.Common.Cqrs.Handlers;
using SC.SenseTower.Tickets.Dto.Tickets;
using SC.SenseTower.Tickets.Services;

namespace SC.SenseTower.Tickets.Cqrs.EventTicketList
{
    public class EventTicketListRequestHandler : BaseHandler, IRequestHandler<EventTicketListRequest, IEnumerable<TicketDto>>
    {
        private readonly TicketsService ticketsService;

        public EventTicketListRequestHandler(
            ILogger<EventTicketListRequestHandler> logger,
            IMapper mapper,
            TicketsService ticketsService) : base(logger, mapper)
        {
            this.ticketsService = ticketsService;
        }

        public async Task<IEnumerable<TicketDto>> Handle(EventTicketListRequest request, CancellationToken cancellationToken)
        {
            var data = await ticketsService.GetByEventId(request.EventId, cancellationToken);
            var result = Mapper.Map<TicketDto[]>(data);
            return result;
        }
    }
}
