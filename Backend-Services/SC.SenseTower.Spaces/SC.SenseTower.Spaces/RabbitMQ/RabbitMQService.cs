using Microsoft.Extensions.Options;
using SC.SenseTower.Common.Services.RabbitMQ;
using SC.SenseTower.Common.Services.RabbitMQ.Constants;
using SC.SenseTower.Common.Services.RabbitMQ.Settings;
using SC.SenseTower.Spaces.Data.Models;

namespace SC.SenseTower.Spaces.RabbitMQ
{
    public class RabbitMQService : RabbitMQSender, IRabbitMQService
    {
        public RabbitMQService(
            ILogger<RabbitMQService> logger,
            IOptions<RabbitMQConnectionSettings> options) : base(logger, options)
        {
        }

        public async Task SendUpdateSpaceMessage(Space space, CancellationToken cancellationToken)
        {
            await Send(RmqExchanges.DEFAULT_EXCHANGE, $"{RmqRoutes.RKEY_TOWER}.{RmqRoutes.RKEY_SPACES}.{RmqRoutes.RKEY_UPDATE}", space, cancellationToken);
        }

        public async Task SendDeleteSpaceMessage(Guid spaceId, CancellationToken cancellationToken)
        {
            await Send(RmqExchanges.DEFAULT_EXCHANGE, $"{RmqRoutes.RKEY_TOWER}.{RmqRoutes.RKEY_SPACES}.{RmqRoutes.RKEY_DELETE}", new { spaceId }, cancellationToken);
        }
    }
}
