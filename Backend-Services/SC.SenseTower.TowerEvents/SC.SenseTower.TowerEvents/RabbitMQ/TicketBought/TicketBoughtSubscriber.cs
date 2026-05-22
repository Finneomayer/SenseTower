using MediatR;
using Microsoft.Extensions.Options;
using SC.SenseTower.Common.Services.RabbitMQ;
using SC.SenseTower.TowerEvents.Settings.RabbitMQ;

namespace SC.SenseTower.TowerEvents.RabbitMQ.TicketBought
{
    public class TicketBoughtSubscriber : RabbitMQSubscriber<TicketBoughtCommand, Unit>
    {
        public TicketBoughtSubscriber(
            ILogger<TicketBoughtSubscriber> logger,
            IServiceProvider services,
            IOptions<TicketBoughtBindingSettings> options) : base(logger, services, options)
        {
        }
    }
}
