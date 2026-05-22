using MediatR;
using Microsoft.Extensions.Options;
using SC.SenseTower.Common.Services.RabbitMQ;
using SC.SenseTower.TowerEvents.Settings.RabbitMQ;

namespace SC.SenseTower.TowerEvents.RabbitMQ.TotalTickets
{
    public class TotalTicketsSubscriber : RabbitMQSubscriber<TotalTicketsCommand, Unit>
    {
        public TotalTicketsSubscriber(
            ILogger<TotalTicketsSubscriber> logger,
            IServiceProvider services,
            IOptions<TotalTicketsBindingSettings> options) : base(logger, services, options)
        {
        }
    }
}
