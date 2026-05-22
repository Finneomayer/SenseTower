using MediatR;
using Microsoft.Extensions.Options;
using SC.SenseTower.Common.Services.RabbitMQ;
using SC.SenseTower.Halls.Settings.RabbitMQ;

namespace SC.SenseTower.Halls.RabbitMQ.UserPlaceUpdate
{
    public class UserPlaceUpdateSubscriber : RabbitMQSubscriber<UserPlaceUpdateCommand, Unit>
    {
        public UserPlaceUpdateSubscriber(
            ILogger<UserPlaceUpdateSubscriber> logger,
            IServiceProvider services,
            IOptions<UserPlaceUpdateBindingSettings> options) : base(logger, services, options)
        {
        }
    }
}
