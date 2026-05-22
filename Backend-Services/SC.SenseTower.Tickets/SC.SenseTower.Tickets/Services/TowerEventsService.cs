using AutoMapper;
using Microsoft.Extensions.Options;
using SC.SenseTower.Common.Services;
using SC.SenseTower.Tickets.Dto.TowerEvents;
using SC.SenseTower.Tickets.Settings;

namespace SC.SenseTower.Tickets.Services
{
    public class TowerEventsService : BaseHttpService
    {
        private readonly ServiceEndpointsSettings endpointsSettings;

        public TowerEventsService(
            ILogger<TowerEventsService> logger,
            IMapper mapper,
            HttpClient httpClient,
            IOptions<ServiceEndpointsSettings> options) : base(logger, mapper, httpClient)
        {
            endpointsSettings = options.Value;
        }

        public async Task<TowerEventResponseDto?> Get(string accessToken, Guid eventId, CancellationToken cancellationToken)
        {
            var result = await Get<TowerEventResponseDto>(accessToken, String.Format(endpointsSettings.GetTowerEventUrl, eventId), null, cancellationToken);
            return result;
        }
    }
}
