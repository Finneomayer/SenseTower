using AutoMapper;
using MediatR;
using SC.SenseTower.Common.Cqrs.Handlers;
using SC.SenseTower.Halls.Cqrs.Commands;
using SC.SenseTower.Halls.Data.Models;
using SC.SenseTower.Halls.Services;

namespace SC.SenseTower.Halls.Cqrs.Handlers
{
    public class CreateHallCommandHandler : BaseHandler, IRequestHandler<CreateHallCommand, Guid>
    {
        private readonly HallsService hallsService;
        private readonly SpacesService spacesService;
        
        public CreateHallCommandHandler(
            ILogger<CreateHallCommandHandler> logger,
            IMapper mapper,
            HallsService hallsService,
            SpacesService spacesService) : base(logger, mapper)
        {
            this.hallsService = hallsService;
            this.spacesService = spacesService;
        }

        public async Task<Guid> Handle(CreateHallCommand request, CancellationToken cancellationToken)
        {
            var hall = Mapper.Map<Hall>(request);
            if (request.SpaceId != null)
            {
                var spaceInfo = await spacesService.GetSpace(request.AccessToken, request.SpaceId.Value, cancellationToken);
                var space = Mapper.Map<LocalSpace>(spaceInfo);
                hall.Space = space;
            }
            return await hallsService.Create(hall, cancellationToken);
        }
    }
}
