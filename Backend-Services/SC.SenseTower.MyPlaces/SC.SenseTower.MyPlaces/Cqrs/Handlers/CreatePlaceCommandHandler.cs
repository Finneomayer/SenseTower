using AutoMapper;
using MediatR;
using SC.SenseTower.Common.Cqrs.Handlers;
using SC.SenseTower.MyPlaces.Cqrs.Commands;
using SC.SenseTower.MyPlaces.Data.Models;
using SC.SenseTower.MyPlaces.Services;

namespace SC.SenseTower.MyPlaces.Cqrs.Handlers
{
    public class CreatePlaceCommandHandler : BaseHandler, IRequestHandler<CreatePlaceCommand, Guid>
    {
        public readonly PlacesService placesService;
        public readonly SpacesService spacesService;

        public CreatePlaceCommandHandler(
            ILogger<CreatePlaceCommandHandler> logger,
            IMapper mapper,
            PlacesService placesService,
            SpacesService spacesService) : base(logger, mapper)
        {
            this.placesService = placesService;
            this.spacesService = spacesService;
        }

        public async Task<Guid> Handle(CreatePlaceCommand request, CancellationToken cancellationToken)
        {
            var spaceDto = request.SpaceId == null
                ? null
                : await spacesService.GetSpace(request.AccessToken, request.SpaceId.Value, cancellationToken);
            var space = Mapper.Map<Space>(spaceDto);
            return await placesService.Create(
                request.Id,
                request.PlaceName,
                request.PlaceNumber,
                request.PublicAccessType,
                request.OwnerId,
                request.DoorImageId,
                space,
                cancellationToken);
        }
    }
}
