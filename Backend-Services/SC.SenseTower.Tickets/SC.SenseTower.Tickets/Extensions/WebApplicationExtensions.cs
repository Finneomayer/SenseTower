using SC.SenseTower.Common.Services.RabbitMQ;
using SC.SenseTower.Tickets.RabbitMQ.TicketsCreate;
using SC.SenseTower.Tickets.RabbitMQ.TowerEventDelete;
using SC.SenseTower.Tickets.RabbitMQ.TowerEventUpdate;
using SC.SenseTower.Tickets.RabbitMQ.UserDelete;

namespace SC.SenseTower.Tickets.Extensions
{
    public static class WebApplicationExtensions
    {
        public static WebApplication UseRabbitMQ(this WebApplication app)
        {
            var receiver = app.Services.GetRequiredService<RabbitMQReceiver>();
            receiver.Start(new IRabbitMQSubscriber[]
            {
                app.Services.GetRequiredService<UserDeleteSubscriber>(),
                app.Services.GetRequiredService<TicketsCreateSubscriber>(),
                app.Services.GetRequiredService<TowerEventUpdateSubscriber>(),
                app.Services.GetRequiredService<TowerEventDeleteSubscriber>()
            });
            return app;
        }
    }
}
