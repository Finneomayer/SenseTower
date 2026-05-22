using AutoMapper;
using MediatR;
using SC.SenseTower.Common.Cqrs.Handlers;
using SC.SenseTower.Spaces.Cqrs.Commands;
using SC.SenseTower.Spaces.RabbitMQ;
using SC.SenseTower.Spaces.Services;

namespace SC.SenseTower.Spaces.Cqrs.Handlers
{
    public class DeleteSpaceCommandHandler : BaseHandler, IRequestHandler<DeleteSpaceCommand, Unit>
    {
        private readonly ISpacesService spacesService;
        private readonly IRabbitMQService rabbitMQService;

        public DeleteSpaceCommandHandler(
            ILogger<DeleteSpaceCommandHandler> logger,
            IMapper mapper,
            ISpacesService spacesService,
            IRabbitMQService rabbitMQService) : base(logger, mapper)
        {
            this.spacesService = spacesService;
            this.rabbitMQService = rabbitMQService;
        }

        public async Task<Unit> Handle(DeleteSpaceCommand request, CancellationToken cancellationToken)
        {
            await spacesService.Delete(request.Id, cancellationToken);
            await rabbitMQService.SendDeleteSpaceMessage(request.Id, cancellationToken);
            return Unit.Value;
        }
    }
}
