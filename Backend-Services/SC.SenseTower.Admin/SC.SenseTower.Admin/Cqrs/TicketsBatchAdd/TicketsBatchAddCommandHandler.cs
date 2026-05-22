using AutoMapper;
using MediatR;
using SC.SenseTower.Admin.Services;
using SC.SenseTower.Common.Cqrs.Handlers;

namespace SC.SenseTower.Admin.Cqrs.TicketsBatchAdd
{
    public class TicketsBatchAddCommandHandler : BaseHandler, IRequestHandler<TicketsBatchAddCommand, string[]>
    {
        private readonly GuestInvitesService ticketsService;

        public TicketsBatchAddCommandHandler(
            ILogger<TicketsBatchAddCommandHandler> logger,
            IMapper mapper,
            GuestInvitesService ticketsService) : base(logger, mapper)
        {
            this.ticketsService = ticketsService;
        }

        public async Task<string[]> Handle(TicketsBatchAddCommand request, CancellationToken cancellationToken)
        {
            return await ticketsService.CreateBatch(request.UserId, request.Quantity, cancellationToken);
        }
    }
}
