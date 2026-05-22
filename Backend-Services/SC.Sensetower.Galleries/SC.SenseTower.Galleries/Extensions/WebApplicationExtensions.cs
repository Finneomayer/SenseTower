using SC.SenseTower.Common.Services.RabbitMQ;
using SC.SenseTower.Galleries.RabbitMQ;
using SC.SenseTower.Galleries.RabbitMQ.SpaceDelete;
using SC.SenseTower.Galleries.RabbitMQ.SpaceUpdate;

namespace SC.SenseTower.Galleries.Extensions
{
    public static class WebApplicationExtensions
    {
        public static WebApplication UseRabbitMQ(this WebApplication app)
        {
            try
            {
                var receiver = app.Services.GetRequiredService<RabbitMQReceiver>();
                receiver.Start(new IRabbitMQSubscriber[]
                {
                    //app.Services.GetRequiredService<UserDeleteSubscriber>(),
                    app.Services.GetRequiredService<SpaceDeleteSubscriber>(),
                    app.Services.GetRequiredService<SpaceUpdateSubscriber>()
                });

                var sender = app.Services.GetRequiredService<RabbitMQService>();
                sender.Start(default).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                var logger = app.Services.GetService<ILogger>();
                if (logger != null)
                    logger.LogError(ex.Message);
            }

            return app;
        }
    }
}
