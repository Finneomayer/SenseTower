using Microsoft.Extensions.Options;
using SC.SenseTower.Common.Services.RabbitMQ;
using SC.SenseTower.Common.Services.RabbitMQ.Constants;
using SC.SenseTower.Common.Services.RabbitMQ.Settings;

namespace SC.SenseTower.Galleries.RabbitMQ
{
    public class RabbitMQService : RabbitMQSender
    {
        public RabbitMQService(
            ILogger<RabbitMQService> logger,
            IOptions<RabbitMQConnectionSettings> options) : base(logger, options)
        {
        }

        public async Task SendImagesDeleteMessage(Guid[] imageIds, CancellationToken cancellationToken)
        {
            await Send(RmqExchanges.DEFAULT_EXCHANGE, $"{RmqRoutes.RKEY_TOWER}.{RmqRoutes.RKEY_IMAGES}.{RmqRoutes.RKEY_DELETE}", new { imageIds }, cancellationToken);
        }
    }
}
