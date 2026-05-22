using SC.SenseTower.Cinemas.RabbitMQ.SpaceDelete;
using SC.SenseTower.Cinemas.RabbitMQ.SpaceUpdate;
using SC.SenseTower.Cinemas.RabbitMQ.UserDelete;
using SC.SenseTower.Common.Services.RabbitMQ;

namespace SC.SenseTower.Cinemas.Extensions
{
    public static class WebApplicationExtensions
    {
        public static WebApplication UseRabbitMQ(this WebApplication app)
        {
            var receiver = app.Services.GetRequiredService<RabbitMQReceiver>();
            receiver.Start(new IRabbitMQSubscriber[]
            {
                app.Services.GetRequiredService<UserDeleteSubscriber>(),
                app.Services.GetRequiredService<SpaceUpdateSubscriber>(),
                app.Services.GetRequiredService<SpaceDeleteSubscriber>()
            });
            return app;
        }
    }
}
