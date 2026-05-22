using MediatR;
using Microsoft.Extensions.Options;
using SC.SenseTower.Accounts.Settings.RabbitMQ;
using SC.SenseTower.Common.Services.RabbitMQ;

namespace SC.SenseTower.Accounts.RabbitMQ.UserDelete
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
