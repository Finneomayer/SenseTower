using AutoMapper;
using Microsoft.Extensions.Options;
using SC.SenseTower.Common.Models;
using SC.SenseTower.Common.Services;
using SC.SenseTower.Spaces.Settings;

namespace SC.SenseTower.Spaces.Services
{
    public class HallsService : BaseHttpService
    {
        private readonly ServiceEndpointsSettings settings;

        public HallsService(
            ILogger<HallsService> logger,
            IMapper mapper,
            HttpClient httpClient,
            IOptions<ServiceEndpointsSettings> options) : base(logger, mapper, httpClient)
        {
            settings = options.Value;
        }

        public async Task<IEnumerable<LookupItemDto<Guid>>?> Lookup(string? accessToken, Guid[] hallIds, CancellationToken cancellationToken)
        {
            var result = await Post<LookupItemDto<Guid>[]?>(accessToken, settings.LookupHallsUrl, new { hallIds }, cancellationToken);
            return result;
        }

        public async Task ClearSpace(string? accessToken, Guid? hallId, Guid spaceId, CancellationToken cancellationToken)
        {
            await Post<bool>(accessToken, settings.ClearSpaceUrl, new { hallId, spaceId }, cancellationToken);
            return;
        }
    }
}
