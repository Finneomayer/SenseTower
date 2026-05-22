using AutoMapper;
using MediatR;
using SC.SenseTower.Admin.Cqrs.TicketDetails;
using SC.SenseTower.Admin.Dto.Invites;
using SC.SenseTower.Admin.Dto.Tickets;
using SC.SenseTower.Admin.Services;
using SC.SenseTower.Common.Cqrs.Handlers;
using System.Linq;

namespace SC.SenseTower.Admin.Cqrs.TicketDetails
{
    public class TicketDetailsRequestHandler : BaseHandler, IRequestHandler<TicketDetailsRequest, TicketDetailsDto>
    {
        private readonly GuestInvitesService ticketsService;
        private readonly IdentityService identityService;

        public TicketDetailsRequestHandler(
            ILogger<TicketDetailsRequestHandler> logger,
            IMapper mapper,
            GuestInvitesService ticketsService,
            IdentityService identityService) : base(logger, mapper)
        {
            this.ticketsService = ticketsService;
            this.identityService = identityService;
        }

        public async Task<TicketDetailsDto> Handle(TicketDetailsRequest request, CancellationToken cancellationToken)
        {
            var ticket = await ticketsService.Get(request.TicketId, cancellationToken);
            var result = Mapper.Map<TicketDetailsDto>(ticket);

            var userIds = new List<Guid?>();
            if (result.IssuerId != null)
                userIds.Add(result.IssuerId.Value);
            if (result.UserId != null && result.UserId != result.IssuerId)
                userIds.Add(result.UserId.Value);
            if (userIds.Count > 0)
            {
                var users = await identityService.GetUserLookups(userIds, cancellationToken);
                if (users != null)
                {
                    if (result.IssuerId != null)
                        result.IssuerName = users.FirstOrDefault(r => r.Id == result.IssuerId.Value)?.Name;
                    if (result.UserId != null)
                        result.UserName = users.FirstOrDefault(r => r.Id == result.UserId.Value)?.Name;
                }
            }

            return result;
        }
    }
}
