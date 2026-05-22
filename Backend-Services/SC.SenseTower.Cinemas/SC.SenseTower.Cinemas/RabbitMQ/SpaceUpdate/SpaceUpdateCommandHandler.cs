using AutoMapper;
using MediatR;
using SC.SenseTower.Cinemas.Data.Models;
using SC.SenseTower.Cinemas.Services;
using SC.SenseTower.Common.Cqrs.Handlers;

namespace SC.SenseTower.Cinemas.RabbitMQ.SpaceUpdate
{
    public class SpaceUpdateCommandHandler : BaseHandler, IRequestHandler<SpaceUpdateCommand, Unit>
    {
        private readonly CinemasService cinemasService;

        public SpaceUpdateCommandHandler(
            ILogger<SpaceUpdateCommandHandler> logger,
            IMapper mapper,
            CinemasService cinemasService) : base(logger, mapper)
        {
            this.cinemasService = cinemasService;
        }

        public async Task<Unit> Handle(SpaceUpdateCommand request, CancellationToken cancellationToken)
        {
            var space = Mapper.Map<Space>(request);
            await cinemasService.UpdateSpace(space, cancellationToken);
            return Unit.Value;
        }
    }
}
