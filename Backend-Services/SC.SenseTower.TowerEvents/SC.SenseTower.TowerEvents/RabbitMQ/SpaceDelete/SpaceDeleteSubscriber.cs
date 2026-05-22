using MediatR;
using Microsoft.Extensions.Options;
using SC.SenseTower.Common.Services.RabbitMQ;
using SC.SenseTower.TowerEvents.Settings.RabbitMQ;

namespace SC.SenseTower.TowerEvents.RabbitMQ.SpaceDelete
{
    public class SpaceDeleteSubscriber : RabbitMQSubscriber<SpaceDeleteCommand, Unit>
    {
        public SpaceDeleteSubscriber(
            ILogger<SpaceDeleteSubscriber> logger,
            IServiceProvider services,
            IOptions<SpaceDeleteBindingSettings> options) : base(logger, services, options)
        {
        }
    }
}
