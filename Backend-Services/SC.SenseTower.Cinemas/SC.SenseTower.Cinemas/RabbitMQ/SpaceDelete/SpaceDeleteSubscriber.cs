using MediatR;
using Microsoft.Extensions.Options;
using SC.SenseTower.Cinemas.Settings.RabbitMQ;
using SC.SenseTower.Common.Services.RabbitMQ;

namespace SC.SenseTower.Cinemas.RabbitMQ.SpaceDelete
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
