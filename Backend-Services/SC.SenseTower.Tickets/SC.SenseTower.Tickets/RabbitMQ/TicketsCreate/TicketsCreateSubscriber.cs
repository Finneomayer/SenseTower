using MediatR;
using Microsoft.Extensions.Options;
using SC.SenseTower.Common.Services.RabbitMQ;
using SC.SenseTower.Tickets.Settings.RabbitMQ;

namespace SC.SenseTower.Tickets.RabbitMQ.TicketsCreate
{
    public class TicketsCreateSubscriber : RabbitMQSubscriber<TicketsCreateCommand, Unit>
    {
        public TicketsCreateSubscriber(
            ILogger<TicketsCreateSubscriber> logger,
            IServiceProvider services,
            IOptions<TicketsCreateBindingSettings> options) : base(logger, services, options)
        {
        }
    }
}
