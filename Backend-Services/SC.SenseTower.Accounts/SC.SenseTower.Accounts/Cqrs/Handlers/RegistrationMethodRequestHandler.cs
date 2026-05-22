using AutoMapper;
using MediatR;
using SC.SenseTower.Accounts.Cqrs.Requests;
using SC.SenseTower.Accounts.Services;
using SC.SenseTower.Common.Cqrs.Handlers;

namespace SC.SenseTower.Accounts.Cqrs.Handlers
{
    public class RegistrationMethodRequestHandler : BaseHandler, IRequestHandler<RegistrationMethodRequest, string?>
    {
        private readonly InvitesService invitesService;
        private readonly TicketsService ticketsService;

        public RegistrationMethodRequestHandler(
            ILogger<RegistrationMethodRequestHandler> logger,
            IMapper mapper,
            InvitesService invitesService,
            TicketsService ticketsService) : base(logger, mapper)
        {
            this.invitesService = invitesService;
            this.ticketsService = ticketsService;
        }

        public async Task<string?> Handle(RegistrationMethodRequest request, CancellationToken cancellationToken)
        {
            var invite = await invitesService.Get(request.Code, cancellationToken);
            if (invite != null)
                return "invite";

            var ticket = await ticketsService.Get(request.Code, cancellationToken);
            if (ticket != null)
                return "ticket";

            return null;
        }
    }
}
