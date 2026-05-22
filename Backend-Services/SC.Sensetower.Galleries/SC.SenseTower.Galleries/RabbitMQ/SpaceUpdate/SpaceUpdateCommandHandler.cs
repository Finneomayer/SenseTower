using AutoMapper;
using MediatR;
using SC.SenseTower.Common.Cqrs.Handlers;
using SC.SenseTower.Galleries.Data.Models;
using SC.SenseTower.Galleries.Services;

namespace SC.SenseTower.Galleries.RabbitMQ.SpaceUpdate
{
    public class SpaceUpdateCommandHandler : BaseHandler, IRequestHandler<SpaceUpdateCommand, Unit>
    {
        private readonly GalleriesService galleriesService;

        public SpaceUpdateCommandHandler(
            ILogger<SpaceUpdateCommandHandler> logger,
            IMapper mapper,
            GalleriesService galleriesService) : base(logger, mapper)
        {
            this.galleriesService = galleriesService;
        }

        public async Task<Unit> Handle(SpaceUpdateCommand request, CancellationToken cancellationToken)
        {
            var space = Mapper.Map<Space>(request);
            await galleriesService.UpdateSpace(space, cancellationToken);
            return Unit.Value;
        }
    }
}
