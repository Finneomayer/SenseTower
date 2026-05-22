using AutoMapper;
using MediatR;
using SC.SenseTower.Common.Cqrs.Handlers;
using SC.SenseTower.TowerEvents.Services;

namespace SC.SenseTower.TowerEvents.RabbitMQ.UserDelete
{
    public class UserDeleteCommandHandler : BaseHandler, IRequestHandler<UserDeleteCommand, Unit>
    {
        private readonly TowerEventsService towerEventsService;

        public UserDeleteCommandHandler(
            ILogger<UserDeleteCommandHandler> logger,
            IMapper mapper,
            TowerEventsService towerEventsService) : base(logger, mapper)
        {
            this.towerEventsService = towerEventsService;
        }

        public async Task<Unit> Handle(UserDeleteCommand request, CancellationToken cancellationToken)
        {
            await towerEventsService.DeleteUser(request.UserId, cancellationToken);
            return Unit.Value;
        }
    }
}
