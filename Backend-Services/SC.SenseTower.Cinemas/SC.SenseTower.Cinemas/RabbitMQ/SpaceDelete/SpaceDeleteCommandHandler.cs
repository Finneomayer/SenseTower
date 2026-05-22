using AutoMapper;
using MediatR;
using SC.SenseTower.Cinemas.Services;
using SC.SenseTower.Common.Cqrs.Handlers;

namespace SC.SenseTower.Cinemas.RabbitMQ.SpaceDelete
{
    public class SpaceDeleteCommandHandler : BaseHandler, IRequestHandler<SpaceDeleteCommand, Unit>
    {
        private readonly CinemasService cinemasService;

        public SpaceDeleteCommandHandler(
            ILogger<SpaceDeleteCommandHandler> logger,
            IMapper mapper,
            CinemasService cinemasService) : base(logger, mapper)
        {
            this.cinemasService = cinemasService;
        }

        public async Task<Unit> Handle(SpaceDeleteCommand request, CancellationToken cancellationToken)
        {
            await cinemasService.DeleteSpace(request.SpaceId, cancellationToken);
            return Unit.Value;
        }
    }
}
