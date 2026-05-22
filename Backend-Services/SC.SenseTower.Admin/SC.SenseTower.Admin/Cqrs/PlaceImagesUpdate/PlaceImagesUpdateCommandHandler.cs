using AutoMapper;
using MediatR;
using SC.SenseTower.Admin.Services;
using SC.SenseTower.Common.Cqrs.Handlers;

namespace SC.SenseTower.Admin.Cqrs.PlaceImagesUpdate
{
    public class PlaceImagesUpdateCommandHandler : BaseHandler, IRequestHandler<PlaceImagesUpdateCommand, Unit>
    {
        private readonly PlacesService placesService;

        public PlaceImagesUpdateCommandHandler(
            ILogger<PlaceImagesUpdateCommandHandler> logger,
            IMapper mapper,
            PlacesService placesService) : base(logger, mapper)
        {
            this.placesService = placesService;
        }

        public async Task<Unit> Handle(PlaceImagesUpdateCommand request, CancellationToken cancellationToken)
        {
            await placesService.UpdatePlaceImages(request.AccessToken, request.RefreshToken, request.PlaceId, request.Images, cancellationToken);
            return Unit.Value;
        }
    }
}
