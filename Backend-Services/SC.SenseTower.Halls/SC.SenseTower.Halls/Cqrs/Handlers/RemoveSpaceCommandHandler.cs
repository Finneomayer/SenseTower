using AutoMapper;
using MediatR;
using SC.SenseTower.Common.Cqrs.Handlers;
using SC.SenseTower.Halls.Cqrs.Commands;
using SC.SenseTower.Halls.Services;

namespace SC.SenseTower.Halls.Cqrs.Handlers
{
    public class RemoveSpaceCommandHandler : BaseHandler, IRequestHandler<RemoveSpaceCommand, Unit>
    {
        private readonly HallsService hallsService;

        public RemoveSpaceCommandHandler(
            ILogger<RemoveSpaceCommandHandler> logger,
            IMapper mapper,
            HallsService hallsService) : base(logger, mapper)
        {
            this.hallsService = hallsService;
        }

        public async Task<Unit> Handle(RemoveSpaceCommand request, CancellationToken cancellationToken)
        {
            await hallsService.RemoveSpace(request.HallId, request.SpaceId, cancellationToken);
            return Unit.Value;
        }
    }
}
