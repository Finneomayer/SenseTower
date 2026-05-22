using AutoMapper;
using Microsoft.Extensions.Options;
using SC.SenseTower.Common.Services;
using SC.SenseTower.Galleries.Dto.Spaces;
using SC.SenseTower.Galleries.Settings;

namespace SC.SenseTower.Galleries.Services
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

        public async Task<SpaceDto> GetSpace(string accessToken, Guid spaceId, CancellationToken cancellationToken)
        {
            var idStr = spaceId.ToString();
            if (!endpointsSettings.GetSpaceUrl.EndsWith('/'))
                idStr = '/' + idStr;
            var result = await Get<SpaceDto>(accessToken, endpointsSettings.GetSpaceUrl + idStr, null, cancellationToken);
            return result;
        }
    }
}
