using AutoMapper;
using Microsoft.Extensions.Options;
using SC.SenseTower.Common.Services;
using SC.SenseTower.Halls.Dto.Places;
using SC.SenseTower.Halls.Settings;

namespace SC.SenseTower.Halls.Services
{
    public class MyPlacesService : BaseHttpService
    {
        private readonly ServiceEndpointsSettings endpointsSettings;

        public MyPlacesService(
            ILogger<MyPlacesService> logger,
            IMapper mapper,
            HttpClient httpClient,
            IOptions<ServiceEndpointsSettings> options) : base(logger, mapper, httpClient)
        {
            endpointsSettings = options.Value;
        }

        public async Task<PlaceInfoDto[]> GetPlacesByIds(string? accessToken, Guid[] placeIds, CancellationToken cancellationToken)
        {
            return await Post<PlaceInfoDto[]>(accessToken, endpointsSettings.MyPlacesByIdsUrl, new { placeIds }, cancellationToken) ?? Array.Empty<PlaceInfoDto>();
        }
    }
}
