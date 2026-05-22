using MediatR;
using Microsoft.Extensions.Options;
using SC.SenseTower.Common.Services.RabbitMQ;
using SC.SenseTower.MyPlaces.Settings.RabbitMQ;

namespace SC.SenseTower.MyPlaces.RabbitMQ.SpaceUpdate
{
    public class SpaceUpdateSubscriber : RabbitMQSubscriber<SpaceUpdateCommand, Unit>
    {
        public SpaceUpdateSubscriber(
            ILogger<SpaceUpdateSubscriber> logger,
            IServiceProvider services,
            IOptions<SpaceUpdateBindingSettings> options) : base(logger, services, options)
        {
        }
    }
}
