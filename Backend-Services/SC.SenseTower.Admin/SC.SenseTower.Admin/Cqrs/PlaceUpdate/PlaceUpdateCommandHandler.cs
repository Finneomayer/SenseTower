using AutoMapper;
using MediatR;
using SC.SenseTower.Admin.Dto.Places;
using SC.SenseTower.Admin.Services;
using SC.SenseTower.Common.Cqrs.Handlers;

namespace SC.SenseTower.Admin.Cqrs.PlaceUpdate
{
    public class PlaceUpdateCommandHandler : BaseHandler, IRequestHandler<PlaceUpdateCommand, Unit>
    {
        private readonly PlacesService placesService;

        public PlaceUpdateCommandHandler(
            ILogger<PlaceUpdateCommandHandler> logger,
            IMapper mapper,
            PlacesService placesService) : base(logger, mapper)
        {
            this.placesService = placesService;
        }

        public async Task<Unit> Handle(PlaceUpdateCommand request, CancellationToken cancellationToken)
        {
            var placeDto = Mapper.Map<PlaceSaveDto>(request);
            await placesService.Update(request.AccessToken, request.RefreshToken, placeDto, cancellationToken);
            return Unit.Value;
        }
    }
}
