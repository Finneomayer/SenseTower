using AutoMapper;
using Microsoft.Extensions.Options;
using SC.SenseTower.Common.Services;
using SC.SenseTower.Halls.Dto.Spaces;
using SC.SenseTower.Halls.Settings;

namespace SC.SenseTower.Halls.Services
{
    public class SpacesService : BaseHttpService
    {
        private readonly ServiceEndpointsSettings endpointsSettings;

        public SpacesService(
            ILogger<SpacesService> logger,
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

        public async Task<SpaceResponseDto?> Get(string accessToken, Guid spaceId, CancellationToken cancellationToken)
        {
            var result = await Get<SpaceResponseDto>(accessToken, string.Format(endpointsSettings.GetSpaceUrl, spaceId), null, cancellationToken);
            return result;
        }
    }
}
