using AutoMapper;
using MediatR;
using SC.SenseTower.Common.Cqrs.Handlers;
using SC.SenseTower.Tickets.Services;

namespace SC.SenseTower.Tickets.RabbitMQ.UserDelete
{
    public class UserDeleteCommandHandler : BaseHandler, IRequestHandler<UserDeleteCommand, Unit>
    {
        private readonly TicketsService ticketsService;

        public UserDeleteCommandHandler(
            ILogger<UserDeleteCommandHandler> logger,
            IMapper mapper,
            TicketsService ticketsService) : base(logger, mapper)
        {
            this.ticketsService = ticketsService;
        }

        public async Task<Unit> Handle(UserDeleteCommand request, CancellationToken cancellationToken)
        {
            await ticketsService.ClearUser(request.UserId, cancellationToken);
            return Unit.Value;
        }
    }
}
