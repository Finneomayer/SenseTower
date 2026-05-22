using Microsoft.Extensions.Options;
using SC.SenseTower.Common.Services.RabbitMQ;
using SC.SenseTower.Common.Services.RabbitMQ.Constants;
using SC.SenseTower.Common.Services.RabbitMQ.Settings;
using SC.SenseTower.MyPlaces.Dto.Places;

namespace SC.SenseTower.MyPlaces.RabbitMQ
{
    public class RabbitMQService : RabbitMQSender
    {
        public RabbitMQService(
            ILogger<RabbitMQService> logger,
            IOptions<RabbitMQConnectionSettings> options) : base(logger, options)
        {
        }

        public async Task SendUpdatePlaceMessage(PlaceDto place, CancellationToken cancellationToken)
        {
            await Send(RmqExchanges.DEFAULT_EXCHANGE, $"{RmqRoutes.RKEY_TOWER}.{RmqRoutes.RKEY_PLACES}.{RmqRoutes.RKEY_UPDATE}", place, cancellationToken);
        }

        public async Task SendDeletePlacesMessage(Guid[] placeIds, CancellationToken cancellationToken)
        {
            await Send(RmqExchanges.DEFAULT_EXCHANGE, $"{RmqRoutes.RKEY_TOWER}.{RmqRoutes.RKEY_PLACES}.{RmqRoutes.RKEY_DELETE}", new { placeIds }, cancellationToken);
        }
    }
}
