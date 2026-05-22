using AutoMapper;
using MediatR;
using SC.SenseTower.Common.Cqrs.Handlers;
using SC.SenseTower.Halls.Services;

namespace SC.SenseTower.Halls.RabbitMQ.UserDelete
{
    public class UserDeleteCommandHandler : BaseHandler, IRequestHandler<UserDeleteCommand, Unit>
    {
        private readonly HallsService hallsService;

        public UserDeleteCommandHandler(
            ILogger<UserDeleteCommandHandler> logger,
            IMapper mapper,
            HallsService hallsService) : base(logger, mapper)
        {
            this.hallsService = hallsService;
        }

        public async Task<Unit> Handle(UserDeleteCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await hallsService.ClearUser(request.UserId, cancellationToken);
            }
            catch (Exception ex)
            {
                //todo: сделать передачу предупреждений на фронт
                Logger.LogError(ex.Message);
            }
            return Unit.Value;
        }
    }
}
