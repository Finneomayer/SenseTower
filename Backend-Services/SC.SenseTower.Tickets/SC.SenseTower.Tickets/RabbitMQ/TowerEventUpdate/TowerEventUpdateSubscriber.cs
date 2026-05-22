using MediatR;
using Microsoft.Extensions.Options;
using SC.SenseTower.Common.Services.RabbitMQ;
using SC.SenseTower.Tickets.Settings.RabbitMQ;

namespace SC.SenseTower.Tickets.RabbitMQ.TowerEventUpdate
{
    public class TowerEventUpdateSubscriber : RabbitMQSubscriber<TowerEventUpdateCommand, Unit>
    {
        public TowerEventUpdateSubscriber(
            ILogger<TowerEventUpdateSubscriber> logger,
            IServiceProvider services,
            IOptions<TowerEventUpdateBindingSettings> options) : base(logger, services, options)
        {
        }
    }
}
