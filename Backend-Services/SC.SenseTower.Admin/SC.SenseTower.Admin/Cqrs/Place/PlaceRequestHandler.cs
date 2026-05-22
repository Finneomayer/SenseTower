using AutoMapper;
using MediatR;
using SC.SenseTower.Admin.Constants;
using SC.SenseTower.Admin.Dto.Places;
using SC.SenseTower.Admin.Services;
using SC.SenseTower.Common.Cqrs.Handlers;
using SC.SenseTower.Common.Exceptions;

namespace SC.SenseTower.Admin.Cqrs.Place
{
    public class PlaceRequestHandler : BaseHandler, IRequestHandler<PlaceRequest, PlaceDto>
    {
        private readonly PlacesService placesService;
        private readonly SpacesService spacesService;
        private readonly ImagesService imagesService;
        private readonly AccountsHttpService accountsHttpService;

        public PlaceRequestHandler(
            ILogger<PlaceRequestHandler> logger,
            IMapper mapper,
            PlacesService placesService,
            SpacesService spacesService,
            ImagesService imagesService,
            AccountsHttpService accountsHttpService) : base(logger, mapper)
        {
            this.placesService = placesService;
            this.spacesService = spacesService;
            this.imagesService = imagesService;
            this.accountsHttpService = accountsHttpService;
        }

        public async Task<PlaceDto> Handle(PlaceRequest request, CancellationToken cancellationToken)
        {
            var place = await placesService.GetPlace(request.AccessToken, request.RefreshToken, request.Id, cancellationToken);
            if (place == null)
                throw new ScException("Помещение не найдено");

            var result = Mapper.Map<PlaceDto>(place);
            result.AvailableImages = await imagesService.Lookup(request.AccessToken, result.OwnerId, cancellationToken);
            result.AvailableUsers = await accountsHttpService.Lookup(request.AccessToken, request.RefreshToken, null, RoleNames.VR_USER, cancellationToken);
            var spaces = await spacesService.Lookup(request.AccessToken, null, cancellationToken);
            var allocatedSpaceIds = await placesService.AllocatedSpaceIds(request.AccessToken, request.RefreshToken, cancellationToken);
            result.AvailableSpaces = spaces.Where(x => !allocatedSpaceIds.Contains(x.Id) || x.Id == result.SpaceId).ToArray();

            return result;
        }
    }
}
