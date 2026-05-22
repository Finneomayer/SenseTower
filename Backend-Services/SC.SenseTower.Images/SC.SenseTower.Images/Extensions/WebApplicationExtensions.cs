using SC.SenseTower.Common.Services.RabbitMQ;
using SC.SenseTower.Images.RabbitMQ.ImagesDelete;
using SC.SenseTower.Images.RabbitMQ.UserDelete;

namespace SC.SenseTower.Images.Extensions
{
    public static class WebApplicationExtensions
    {
        public static WebApplication UseRabbitMQ(this WebApplication app)
        {
            var receiver = app.Services.GetRequiredService<RabbitMQReceiver>();
            receiver.Start(new IRabbitMQSubscriber[]
            {
                app.Services.GetRequiredService<UserDeleteSubscriber>(),
                app.Services.GetRequiredService<ImagesDeleteSubscriber>()
            });
            return app;
        }
    }
}
