using AutoMapper;
using MediatR;
using SC.SenseTower.Cinemas.Services;
using SC.SenseTower.Common.Cqrs.Handlers;

namespace SC.SenseTower.Cinemas.RabbitMQ.UserDelete
{
    public class UserDeleteCommandHandler : BaseHandler, IRequestHandler<UserDeleteCommand, Unit>
    {
        private readonly CinemasService cinemasService;

        public UserDeleteCommandHandler(
            ILogger<UserDeleteCommandHandler> logger,
            IMapper mapper,
            CinemasService cinemasService) : base(logger, mapper)
        {
            this.cinemasService = cinemasService;
        }

        public async Task<Unit> Handle(UserDeleteCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await cinemasService.ClearAdministrator(request.UserId, cancellationToken);
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
