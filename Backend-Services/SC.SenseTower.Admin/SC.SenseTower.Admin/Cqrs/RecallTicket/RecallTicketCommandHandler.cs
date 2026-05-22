using AutoMapper;
using MediatR;
using SC.SenseTower.Admin.Services;
using SC.SenseTower.Common.Cqrs.Handlers;

namespace SC.SenseTower.Admin.Cqrs.RecallTicket
{
    public class RecallTicketCommandHandler : BaseHandler, IRequestHandler<RecallTicketCommand, Unit>
    {
        private readonly GuestInvitesService ticketsService;

        public RecallTicketCommandHandler(
            ILogger<RecallTicketCommandHandler> logger,
            IMapper mapper,
            GuestInvitesService ticketsService) : base(logger, mapper)
        {
            this.ticketsService = ticketsService;
        }

        public async Task<Unit> Handle(RecallTicketCommand request, CancellationToken cancellationToken)
        {
            await ticketsService.Recall(request.Id, request.Reason, cancellationToken);
            return Unit.Value;
        }
    }
}
