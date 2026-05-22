using AutoMapper;
using Microsoft.Extensions.Options;
using SC.SenseTower.Common.Models;
using SC.SenseTower.Common.Services;
using SC.SenseTower.Spaces.Dto.Places;
using SC.SenseTower.Spaces.Settings;

namespace SC.SenseTower.Spaces.Services
{
    public class PlacesService : BaseHttpService
    {
        private readonly ServiceEndpointsSettings endpointsSettings;

        public PlacesService(
            ILogger<PlacesService> logger,
            IMapper mapper,
            HttpClient httpClient,
            IOptions<ServiceEndpointsSettings> options) : base(logger, mapper, httpClient)
        {
            endpointsSettings = options.Value;
        }

        public async Task<PlaceInfoDto[]?> GetPlaces(string accessToken, Guid[] placeIds, CancellationToken cancellationToken)
        {
            var response = await Post<PlaceInfoResponseDto[]>(accessToken, endpointsSettings.PlacesListByIds, new { placeIds }, cancellationToken);
            var result = Mapper.Map<PlaceInfoDto[]>(response);
            return result;
        }

        public async Task<Guid?> Create(string accessToken, string name, CancellationToken cancellationToken)
        {
            var result = await Post<Guid?>(accessToken, endpointsSettings.CreatePlaceUrl, new { name }, cancellationToken);
            return result;
        }

        public async Task<IEnumerable<LookupItemDto<Guid>>?> Lookup(string? accessToken, Guid[] placeIds, CancellationToken cancellationToken)
        {
            var result = await Post<LookupItemDto<Guid>[]?>(accessToken, endpointsSettings.LookupPlacesUrl, new { placeIds }, cancellationToken);
            return result;
        }

        public async Task Delete(string? accessToken, Guid placeId, CancellationToken cancellationToken)
        {
            await Post<bool?>(accessToken, $"{endpointsSettings.DeletePlaceUrl}/{placeId}", null, cancellationToken);
        }

        public async Task<PlaceResponseDto?> GetBySpaceId(string accessToken, Guid spaceId, CancellationToken cancellationToken)
        {
            var result = await Get<PlaceResponseDto>(accessToken, string.Format(endpointsSettings.PlacesGetBySpaceUrl, spaceId), null, cancellationToken);
            return result;
        }
    }
}
