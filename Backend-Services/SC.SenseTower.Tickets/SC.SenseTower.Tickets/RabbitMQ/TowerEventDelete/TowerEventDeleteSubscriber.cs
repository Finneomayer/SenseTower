using MediatR;
using Microsoft.Extensions.Options;
using SC.SenseTower.Common.Services.RabbitMQ;
using SC.SenseTower.Tickets.Settings.RabbitMQ;

namespace SC.SenseTower.Tickets.RabbitMQ.TowerEventDelete
{
    public class TowerEventDeleteSubscriber : RabbitMQSubscriber<TowerEventDeleteCommand, Unit>
    {
        public TowerEventDeleteSubscriber(
            ILogger<TowerEventDeleteSubscriber> logger,
            IServiceProvider services,
            IOptions<TowerEventDeleteBindingSettings> options) : base(logger, services, options)
        {
        }
    }
}
