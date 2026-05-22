using AutoMapper;
using MediatR;
using SC.SenseTower.Common.Cqrs.Handlers;
using SC.SenseTower.Halls.Cqrs.Commands;
using SC.SenseTower.Halls.Data.Models;
using SC.SenseTower.Halls.Services;

namespace SC.SenseTower.Halls.Cqrs.Handlers
{
    public class AddPublicPlaceCommandHandler : BaseHandler, IRequestHandler<AddPublicPlaceCommand, Unit>
    {
        private readonly HallsService hallsService;
        private readonly SpacesService spacesService;

        public AddPublicPlaceCommandHandler(
            ILogger<AddPublicPlaceCommandHandler> logger,
            IMapper mapper,
            HallsService hallsService,
            SpacesService spacesService) : base(logger, mapper)
        {
            this.hallsService = hallsService;
            this.spacesService = spacesService;
        }

        public async Task<Unit> Handle(AddPublicPlaceCommand request, CancellationToken cancellationToken)
        {
            var spaceInfo = await spacesService.GetSpace(request.AccessToken, request.SpaceId, cancellationToken);
            var space = Mapper.Map<Space>(spaceInfo);
            var hall = await hallsService.Get(request.HallId, cancellationToken);
            hall.PublicPlaces = hall.PublicPlaces
                .Union(new[] { space })
                .ToArray();
            await hallsService.Update(hall, cancellationToken);
            return Unit.Value;
        }
    }
}
