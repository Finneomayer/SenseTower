using MediatR;
using SC.SenseTower.Spaces.Cqrs.Requests;
using SC.SenseTower.Spaces.Dto.Places;
using SC.SenseTower.Spaces.Services;

namespace SC.SenseTower.Spaces.Cqrs.Handlers
{
    public class PlacesRequestHandler : IRequestHandler<PlacesRequest, PlaceInfoDto[]?>
    {
        private readonly PlacesService placesService;

        public PlacesRequestHandler(PlacesService placesService)
        {
            this.placesService = placesService;
        }

#pragma warning disable CS8604 // Возможно, аргумент-ссылка, допускающий значение NULL.
        public async Task<PlaceInfoDto[]?> Handle(PlacesRequest request, CancellationToken cancellationToken)
        {
            var result = await placesService.GetPlaces(request.AccessToken, request.PlaceIds, cancellationToken).ConfigureAwait(false);
            return result;
        }
#pragma warning restore CS8604 // Возможно, аргумент-ссылка, допускающий значение NULL.
    }
}
