using AutoMapper;
using MediatR;
using SC.SenseTower.Common.Cqrs.Handlers;
using SC.SenseTower.Halls.Services;

namespace SC.SenseTower.Halls.RabbitMQ.SpaceDelete
{
    public class SpaceDeleteCommandHandler : BaseHandler, IRequestHandler<SpaceDeleteCommand, Unit>
    {
        private readonly HallsService hallsService;

        public SpaceDeleteCommandHandler(
            ILogger<SpaceDeleteCommandHandler> logger,
            IMapper mapper,
            HallsService hallsService) : base(logger, mapper)
        {
            this.hallsService = hallsService;
        }

        public async Task<Unit> Handle(SpaceDeleteCommand request, CancellationToken cancellationToken)
        {
            await hallsService.ClearSpace(request.SpaceId, cancellationToken);
            return Unit.Value;
        }
    }
}
