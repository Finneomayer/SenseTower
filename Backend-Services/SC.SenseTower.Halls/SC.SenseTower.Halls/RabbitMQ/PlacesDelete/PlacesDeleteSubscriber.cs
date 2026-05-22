using MediatR;
using Microsoft.Extensions.Options;
using SC.SenseTower.Common.Services.RabbitMQ;
using SC.SenseTower.Halls.Settings.RabbitMQ;

namespace SC.SenseTower.Halls.RabbitMQ.PlacesDelete
{
    public class PlacesDeleteSubscriber : RabbitMQSubscriber<PlacesDeleteCommand, Unit>
    {
        public PlacesDeleteSubscriber(
            ILogger<PlacesDeleteSubscriber> logger,
            IServiceProvider services,
            IOptions<PlacesDeleteBindingSettings> options) : base(logger, services, options)
        {
        }
    }
}
