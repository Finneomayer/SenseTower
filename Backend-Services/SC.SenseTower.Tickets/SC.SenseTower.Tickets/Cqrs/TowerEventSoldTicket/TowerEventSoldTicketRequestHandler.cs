using AutoMapper;
using MediatR;
using SC.SenseTower.Common.Cqrs.Handlers;
using SC.SenseTower.Tickets.Dto.Tickets;
using SC.SenseTower.Tickets.Services;

namespace SC.SenseTower.Tickets.Cqrs.TowerEventSoldTicket
{
    public class TowerEventSoldTicketRequestHandler : BaseHandler, IRequestHandler<TowerEventSoldTicketRequest, IEnumerable<SoldTicketInfoDto>>
    {
        private readonly TicketsService ticketsService;
        private readonly AccountsService accountsService;

        public TowerEventSoldTicketRequestHandler(
            ILogger<TowerEventSoldTicketRequestHandler> logger,
            IMapper mapper,
            TicketsService ticketsService,
            AccountsService accountsService) : base(logger, mapper)
        {
            this.ticketsService = ticketsService;
            this.accountsService = accountsService;
        }

        public async Task<IEnumerable<SoldTicketInfoDto>> Handle(TowerEventSoldTicketRequest request, CancellationToken cancellationToken)
        {
            var data = await ticketsService.GetSold(request.EventId, cancellationToken);
            var result = Mapper.Map<SoldTicketInfoDto[]>(data);
            if (result.Any())
            {
                var userIds = result
                    .Select(x => x.UserId)
                    .Distinct()
                    .ToArray();
                var userInfos = await accountsService.GetInfoByIds(request.AccessToken, userIds, cancellationToken);
                if (userInfos.Any())
                {
                    foreach (var item in result)
                    {
                        item.UserName = userInfos.FirstOrDefault(x => x.UserId == item.UserId)?.Login;
                    }
                }
            }
            return result;
        }
    }
}
