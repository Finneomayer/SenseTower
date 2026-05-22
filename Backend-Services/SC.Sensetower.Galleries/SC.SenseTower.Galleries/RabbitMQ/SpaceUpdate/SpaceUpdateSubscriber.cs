using MediatR;
using Microsoft.Extensions.Options;
using SC.SenseTower.Common.Services.RabbitMQ;
using SC.SenseTower.Galleries.Settings.RabbitMQ;

namespace SC.SenseTower.Galleries.RabbitMQ.SpaceUpdate
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
