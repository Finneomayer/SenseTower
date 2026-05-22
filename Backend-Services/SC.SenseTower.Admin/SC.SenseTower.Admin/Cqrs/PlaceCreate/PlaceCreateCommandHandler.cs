using AutoMapper;
using MediatR;
using SC.SenseTower.Admin.Dto.Places;
using SC.SenseTower.Admin.Services;
using SC.SenseTower.Common.Cqrs.Handlers;

namespace SC.SenseTower.Admin.Cqrs.PlaceCreate
{
    public class PlaceCreateCommandHandler : BaseHandler, IRequestHandler<PlaceCreateCommand, Guid?>
    {
        private readonly PlacesService placesService;

        public PlaceCreateCommandHandler(
            ILogger<PlaceCreateCommandHandler> logger,
            IMapper mapper,
            PlacesService placesService) : base(logger, mapper)
        {
            this.placesService = placesService;
        }

        public async Task<Guid?> Handle(PlaceCreateCommand request, CancellationToken cancellationToken)
        {
            var placeDto = Mapper.Map<PlaceSaveDto>(request);
            var result = await placesService.Create(request.AccessToken, request.RefreshToken, placeDto, cancellationToken);
            return result;
        }
    }
}
