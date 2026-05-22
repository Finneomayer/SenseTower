using SC.SenseTower.Common.Extensions;
using SC.SenseTower.Common.Services.RabbitMQ;
using SC.SenseTower.Spaces.RabbitMQ;
using SC.SenseTower.Spaces.RabbitMQ.UserDelete;

namespace SC.SenseTower.Spaces.Extensions
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
                    app.Services.GetRequiredService<UserDeleteSubscriber>()
                });

                var sender = app.Services.GetRequiredService<IRabbitMQService>();
                sender.Start(default).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                var logger = app.Services.GetService<ILogger>();
                if (logger != null)
                    logger.LogError(ex.GetMessage());
            }

            return app;
        }
    }
}
