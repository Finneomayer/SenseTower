using AutoMapper;
using MediatR;
using SC.SenseTower.Common.Cqrs.Handlers;
using SC.SenseTower.Halls.Cqrs.Commands;
using SC.SenseTower.Halls.Data.Models;
using SC.SenseTower.Halls.Services;

namespace SC.SenseTower.Halls.Cqrs.Handlers
{
    public class UpdateHallCommandHandler : BaseHandler, IRequestHandler<UpdateHallCommand, bool>
    {
        private readonly HallsService hallsService;
        private readonly SpacesService spacesService;

        public UpdateHallCommandHandler(
            ILogger<UpdateHallCommandHandler> logger,
            IMapper mapper,
            HallsService hallsService,
            SpacesService spacesService) : base(logger, mapper)
        {
            this.hallsService = hallsService;
            this.spacesService = spacesService;
        }

        public async Task<bool> Handle(UpdateHallCommand request, CancellationToken cancellationToken)
        {
            var hall = await hallsService.Get(request.Id, cancellationToken);
            hall.Name = request.Name;
            if (request.SpaceId != null)
            {
                var spaceInfo = await spacesService.GetSpace(request.AccessToken, request.SpaceId.Value, cancellationToken);
                var space = Mapper.Map<LocalSpace>(spaceInfo);
                hall.Space = space;
            }
            else
            {
                hall.Space = null;
            }
            return await hallsService.Update(hall, cancellationToken);
        }
    }
}
