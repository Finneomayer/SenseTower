using MediatR;
using Microsoft.Extensions.Options;
using SC.SenseTower.Common.Services.RabbitMQ;
using SC.SenseTower.Images.Settings.RabbitMQ;

namespace SC.SenseTower.Images.RabbitMQ.ImagesDelete
{
    public class ImagesDeleteSubscriber : RabbitMQSubscriber<ImagesDeleteCommand, Unit>
    {
        public ImagesDeleteSubscriber(
            ILogger<ImagesDeleteSubscriber> logger,
            IServiceProvider services,
            IOptions<ImagesDeleteBindingSettings> options) : base(logger, services, options)
        {
        }
    }
}
