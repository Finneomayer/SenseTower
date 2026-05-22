using MediatR;
using Microsoft.Extensions.Options;
using SC.SenseTower.Common.Services.RabbitMQ;
using SC.SenseTower.Galleries.Settings.RabbitMQ;

namespace SC.SenseTower.Galleries.RabbitMQ.SpaceDelete
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
