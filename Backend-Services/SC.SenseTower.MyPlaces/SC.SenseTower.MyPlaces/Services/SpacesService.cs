using AutoMapper;
using Microsoft.Extensions.Options;
using SC.SenseTower.Common.Services;
using SC.SenseTower.MyPlaces.Dto.Spaces;
using SC.SenseTower.MyPlaces.Settings;

namespace SC.SenseTower.MyPlaces.Services
{
    public class SpacesService : BaseHttpService
    {
        private readonly ServiceEndpointsSettings endpointsSettings;

        public SpacesService(
            ILogger<BaseHttpService> logger,
            IMapper mapper,
            HttpClient httpClient,
            IOptions<ServiceEndpointsSettings> options) : base(logger, mapper, httpClient)
        {
            endpointsSettings = options.Value;
        }

        public async Task<SpaceInfoDto?> GetSpace(string accessToken, Guid spaceId, CancellationToken cancellationToken)
        {
            var result = await Get<SpaceInfoDto>(accessToken, string.Format(endpointsSettings.GetSpaceUrl, spaceId), null, cancellationToken);
            return result;
        }
    }
}
