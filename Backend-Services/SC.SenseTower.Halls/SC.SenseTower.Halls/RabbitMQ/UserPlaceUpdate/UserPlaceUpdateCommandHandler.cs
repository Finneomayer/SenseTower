using AutoMapper;
using MediatR;
using SC.SenseTower.Common.Cqrs.Handlers;
using SC.SenseTower.Halls.Dto.Halls;
using SC.SenseTower.Halls.Services;

namespace SC.SenseTower.Halls.RabbitMQ.UserPlaceUpdate
{
    public class UserPlaceUpdateCommandHandler : BaseHandler, IRequestHandler<UserPlaceUpdateCommand, Unit>
    {
        private readonly HallsService hallsService;

        public UserPlaceUpdateCommandHandler(
            ILogger<UserPlaceUpdateCommandHandler> logger,
            IMapper mapper,
            HallsService hallsService) : base(logger, mapper)
        {
            this.hallsService = hallsService;
        }

        public async Task<Unit> Handle(UserPlaceUpdateCommand request, CancellationToken cancellationToken)
        {
            var userPlaceDto = Mapper.Map<UserPlaceDto>(request);
            await hallsService.UpdateUserPlace(userPlaceDto, cancellationToken);
            return Unit.Value;
        }
    }
}
