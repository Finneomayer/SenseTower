using AutoMapper;
using Microsoft.Extensions.Options;
using SC.SenseTower.Admin.Dto.TowerEvents;
using SC.SenseTower.Admin.Settings;
using SC.SenseTower.Common.Services;

namespace SC.SenseTower.Admin.Services
{
    public class TicketsService : BaseHttpService
    {
        private readonly ServiceEndpointsSettings endpointsSettings;

        public TicketsService(
            ILogger<TicketsService> logger,
            IMapper mapper,
            HttpClient httpClient,
            IOptions<ServiceEndpointsSettings> options) : base(logger, mapper, httpClient)
        {
            endpointsSettings = options.Value;
        }

        public async Task Create(string accessToken, string refreshToken, Guid eventId, int quantity, CancellationToken cancellationToken)
        {
            _ = await PostAsJson<object>(accessToken, endpointsSettings.TicketsAddUrl, new { eventId, quantity }, cancellationToken);
        }

        public async Task<IEnumerable<TowerEventTicketDto>> GetSold(string? accessToken, string? refreshToken, Guid id, CancellationToken cancellationToken)
        {
            var result = await Get<TowerEventTicketDto[]>(accessToken, string.Format(endpointsSettings.TicketsGetSoldUrl, id), null, cancellationToken);
            return result ?? Array.Empty<TowerEventTicketDto>();
        }
    }
}
