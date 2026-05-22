using MediatR;
using Microsoft.Extensions.Options;
using SC.SenseTower.Common.Services.RabbitMQ;
using SC.SenseTower.Halls.Settings.RabbitMQ;

namespace SC.SenseTower.Halls.RabbitMQ.UserDelete
{
    public class UserDeleteSubscriber : RabbitMQSubscriber<UserDeleteCommand, Unit>
    {
        public UserDeleteSubscriber(
            ILogger<UserDeleteSubscriber> logger,
            IServiceProvider services,
            IOptions<UserDeleteBindingSettings> options) : base(logger, services, options)
        {
        }
    }
}
