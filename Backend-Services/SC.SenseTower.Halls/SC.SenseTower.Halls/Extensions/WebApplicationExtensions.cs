using SC.SenseTower.Common.Services.RabbitMQ;
using SC.SenseTower.Halls.RabbitMQ.PlacesDelete;
using SC.SenseTower.Halls.RabbitMQ.SpaceDelete;
using SC.SenseTower.Halls.RabbitMQ.SpaceUpdate;
using SC.SenseTower.Halls.RabbitMQ.UserDelete;
using SC.SenseTower.Halls.RabbitMQ.UserPlaceUpdate;

namespace SC.SenseTower.Halls.Extensions
{
    public static class WebApplicationExtensions
    {
        public static WebApplication UseRabbitMQ(this WebApplication app)
        {
            var receiver = app.Services.GetRequiredService<RabbitMQReceiver>();
            receiver.Start(new IRabbitMQSubscriber[]
            {
                app.Services.GetRequiredService<UserDeleteSubscriber>(),
                app.Services.GetRequiredService<UserPlaceUpdateSubscriber>(),
                app.Services.GetRequiredService<SpaceUpdateSubscriber>(),
                app.Services.GetRequiredService<SpaceDeleteSubscriber>(),
                app.Services.GetRequiredService<PlacesDeleteSubscriber>()
            });
            return app;
        }
    }
}
