using AutoMapper;
using MediatR;
using SC.SenseTower.Common.Cqrs.Handlers;
using SC.SenseTower.Halls.Data.Models;
using SC.SenseTower.Halls.Services;

namespace SC.SenseTower.Halls.RabbitMQ.SpaceUpdate
{
    public class SpaceUpdateCommandHandler : BaseHandler, IRequestHandler<SpaceUpdateCommand, Unit>
    {
        private readonly HallsService hallsService;

        public SpaceUpdateCommandHandler(
            ILogger<SpaceUpdateCommandHandler> logger,
            IMapper mapper,
            HallsService hallsService) : base(logger, mapper)
        {
            this.hallsService = hallsService;
        }

        public async Task<Unit> Handle(SpaceUpdateCommand request, CancellationToken cancellationToken)
        {
            var space = Mapper.Map<LocalSpace>(request);
            await hallsService.UpdateSpace(space, cancellationToken);
            return Unit.Value;
        }
    }
}
