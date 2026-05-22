using SC.SenseTower.Accounts.RabbitMQ.UserDelete;
using SC.SenseTower.Common.Services.RabbitMQ;

namespace SC.SenseTower.Accounts.Extensions
{
    public static class WebApplicationExtensions
    {
        public static WebApplication UseRabbitMQ(this WebApplication app, IServiceCollection services)
        {
            var receiver = app.Services.GetRequiredService<RabbitMQReceiver>();
            receiver.Start(new IRabbitMQSubscriber[]
            {
                app.Services.GetRequiredService<UserDeleteSubscriber>()
            });
            return app;
        }
    }
}
