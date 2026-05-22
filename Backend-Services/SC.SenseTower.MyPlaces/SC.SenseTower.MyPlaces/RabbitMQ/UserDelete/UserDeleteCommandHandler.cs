using AutoMapper;
using MediatR;
using SC.SenseTower.Common.Cqrs.Handlers;
using SC.SenseTower.MyPlaces.Services;

namespace SC.SenseTower.MyPlaces.RabbitMQ.UserDelete
{
    public class UserDeleteCommandHandler : BaseHandler, IRequestHandler<UserDeleteCommand, Unit>
    {
        private readonly PlacesService placesService;

        public UserDeleteCommandHandler(
            ILogger<UserDeleteCommandHandler> logger,
            IMapper mapper,
            PlacesService placesService) : base(logger, mapper)
        {
            this.placesService = placesService;
        }

        public async Task<Unit> Handle(UserDeleteCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await placesService.ClearUser(request.UserId, cancellationToken);
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
