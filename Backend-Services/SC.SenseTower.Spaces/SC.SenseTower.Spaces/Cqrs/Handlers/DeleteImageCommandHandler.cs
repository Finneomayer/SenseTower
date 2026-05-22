using AutoMapper;
using MediatR;
using SC.SenseTower.Common.Cqrs.Handlers;
using SC.SenseTower.Spaces.Cqrs.Commands;
using SC.SenseTower.Spaces.RabbitMQ;
using SC.SenseTower.Spaces.Services;

namespace SC.SenseTower.Spaces.Cqrs.Handlers
{
    public class DeleteImageCommandHandler : BaseHandler, IRequestHandler<DeleteImageCommand, Unit>
    {
        private readonly ISpacesService spacesService;
        private readonly IRabbitMQService rabbitMQService;

        public DeleteImageCommandHandler(
            ILogger<DeleteImageCommandHandler> logger,
            IMapper mapper,
            ISpacesService spacesService,
            IRabbitMQService rabbitMQService) : base(logger, mapper)
        {
            this.spacesService = spacesService;
            this.rabbitMQService = rabbitMQService;
        }

        public async Task<Unit> Handle(DeleteImageCommand request, CancellationToken cancellationToken)
        {
            if (await spacesService.DeleteImage(request.SpaceId, request.ImageId, cancellationToken))
            {
                var space = await spacesService.Get(request.SpaceId, cancellationToken);
                await rabbitMQService.SendUpdateSpaceMessage(space, cancellationToken);
            }
            return Unit.Value;
        }
    }
}
