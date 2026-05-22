using Microsoft.Extensions.Options;
using SC.SenseTower.Common.Services.RabbitMQ;
using SC.SenseTower.Common.Services.RabbitMQ.Constants;
using SC.SenseTower.Common.Services.RabbitMQ.Settings;

namespace SC.SenseTower.Tickets.RabbitMQ
{
    public class RabbitMQService : RabbitMQSender
    {
        public RabbitMQService(
            ILogger<RabbitMQService> logger,
            IOptions<RabbitMQConnectionSettings> options) : base(logger, options)
        {
        }

        public async Task SendTicketBoughtMessage(Guid eventId, Guid ticketId, Guid userId, CancellationToken cancellationToken)
        {
            await Send(RmqExchanges.DEFAULT_EXCHANGE, $"{RmqRoutes.RKEY_TOWER}.{RmqRoutes.RKEY_TICKETS}.{RmqRoutes.RKEY_BOUGHT}", new { eventId, ticketId, userId }, cancellationToken);
        }

        public async Task SendTotalTicketsMessage(Guid eventId, int totalTickets, CancellationToken cancellationToken)
        {
            await Send(RmqExchanges.DEFAULT_EXCHANGE, $"{RmqRoutes.RKEY_TOWER}.{RmqRoutes.RKEY_EVENTS}.{RmqRoutes.RKEY_TOTAL_TICKETS}", new { eventId, totalTickets }, cancellationToken);
        }
    }
}
