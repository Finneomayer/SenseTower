using AutoMapper;
using Microsoft.Extensions.Options;
using SC.SenseTower.Admin.Settings;
using SC.SenseTower.Common.Enums;
using SC.SenseTower.Common.Models;
using SC.SenseTower.Common.Services;

namespace SC.SenseTower.Admin.Services
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

        public async Task<IEnumerable<LookupItemDto<Guid>>> Lookup(string? accessToken, SpaceType? spaceType, CancellationToken cancellationToken)
        {
            var url = spaceType == null ? endpointsSettings.SpacesLookupUrl : (endpointsSettings.SpacesLookupUrl + ((int)spaceType).ToString());
            var result = await Get<LookupItemDto<Guid>[]>(accessToken, url, null, cancellationToken);
            return result ?? Array.Empty<LookupItemDto<Guid>>();
        }
    }
}
