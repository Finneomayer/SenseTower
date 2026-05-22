using AutoMapper;
using Microsoft.Extensions.Options;
using SC.SenseTower.Cinemas.Dto.Spaces;
using SC.SenseTower.Cinemas.Settings;
using SC.SenseTower.Common.Services;

namespace SC.SenseTower.Cinemas.Services
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

        public async Task<SpaceDto?> GetSpace(string accessToken, Guid spaceId, CancellationToken cancellationToken)
        {
            var result = await Get<SpaceDto>(accessToken, string.Format(endpointsSettings.GetSpaceUrl, spaceId), null, cancellationToken);
            return result;
        }
    }
}
