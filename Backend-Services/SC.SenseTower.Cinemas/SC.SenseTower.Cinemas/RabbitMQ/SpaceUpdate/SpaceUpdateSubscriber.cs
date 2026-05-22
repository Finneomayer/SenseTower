using MediatR;
using Microsoft.Extensions.Options;
using SC.SenseTower.Cinemas.Settings.RabbitMQ;
using SC.SenseTower.Common.Services.RabbitMQ;

namespace SC.SenseTower.Cinemas.RabbitMQ.SpaceUpdate
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
