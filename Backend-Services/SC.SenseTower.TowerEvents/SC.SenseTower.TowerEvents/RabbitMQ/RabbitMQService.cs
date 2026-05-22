using Microsoft.Extensions.Options;
using SC.SenseTower.Common.Services.RabbitMQ;
using SC.SenseTower.Common.Services.RabbitMQ.Constants;
using SC.SenseTower.Common.Services.RabbitMQ.Settings;
using SC.SenseTower.TowerEvents.Data.Models;

namespace SC.SenseTower.TowerEvents.RabbitMQ
{
    public class RabbitMQService : RabbitMQSender
    {
        public RabbitMQService(
            ILogger<RabbitMQService> logger,
            IOptions<RabbitMQConnectionSettings> options) : base(logger, options)
        {
        }

        public async Task SendCreateTicketsMessage(TowerEvent towerEvent, int? ticketQuantity, CancellationToken cancellationToken)
        {
            if ((ticketQuantity ?? 0) == 0)
                return;
            await Send(RmqExchanges.DEFAULT_EXCHANGE, $"{RmqRoutes.RKEY_TOWER}.{RmqRoutes.RKEY_TICKETS}.{RmqRoutes.RKEY_CREATE}", new { TowerEvent = towerEvent, Quantity = ticketQuantity }, cancellationToken);
        }

        public async Task SendTowerEventUpdateMessage(TowerEvent? towerEvent, CancellationToken cancellationToken)
        {
            await Send(RmqExchanges.DEFAULT_EXCHANGE, $"{RmqRoutes.RKEY_TOWER}.{RmqRoutes.RKEY_EVENTS}.{RmqRoutes.RKEY_UPDATE}", new { towerEvent }, cancellationToken);
        }

        public async Task SendTowerEventDeleteMessage(Guid eventId, CancellationToken cancellationToken)
        {
            await Send(RmqExchanges.DEFAULT_EXCHANGE, $"{RmqRoutes.RKEY_TOWER}.{RmqRoutes.RKEY_EVENTS}.{RmqRoutes.RKEY_DELETE}", new { eventId }, cancellationToken);
        }
    }
}
