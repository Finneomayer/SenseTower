using AutoMapper;
using MediatR;
using SC.SenseTower.Common.Cqrs.Handlers;
using SC.SenseTower.Halls.Cqrs.Commands;
using SC.SenseTower.Halls.Data.Models;
using SC.SenseTower.Halls.Services;

namespace SC.SenseTower.Halls.Cqrs.Handlers
{
    public class AddSpaceCommandHandler : BaseHandler, IRequestHandler<AddSpaceCommand, Unit>
    {
        private readonly HallsService hallsService;
        private readonly SpacesService spacesService;

        public AddSpaceCommandHandler(
            ILogger<AddSpaceCommandHandler> logger,
            IMapper mapper,
            HallsService hallsService,
            SpacesService spacesService) : base(logger, mapper)
        {
            this.hallsService = hallsService;
            this.spacesService = spacesService;
        }

        public async Task<Unit> Handle(AddSpaceCommand request, CancellationToken cancellationToken)
        {
            var spaceDto = await spacesService.Get(request.AccessToken, request.SpaceId, cancellationToken);
            var space = Mapper.Map<LocalSpace>(spaceDto);
            await hallsService.AddSpace(request.HallId, space, cancellationToken);
            return Unit.Value;
        }
    }
}
