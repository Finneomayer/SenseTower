using SC.SenseTower.Common.Services.RabbitMQ;
using SC.SenseTower.TowerEvents.RabbitMQ;
using SC.SenseTower.TowerEvents.RabbitMQ.SpaceDelete;
using SC.SenseTower.TowerEvents.RabbitMQ.SpaceUpdate;
using SC.SenseTower.TowerEvents.RabbitMQ.TicketBought;
using SC.SenseTower.TowerEvents.RabbitMQ.TotalTickets;
using SC.SenseTower.TowerEvents.RabbitMQ.UserDelete;

namespace SC.SenseTower.TowerEvents.Extensions
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
                    app.Services.GetRequiredService<TicketBoughtSubscriber>(),
                    app.Services.GetRequiredService<TotalTicketsSubscriber>(),
                    app.Services.GetRequiredService<SpaceUpdateSubscriber>(),
                    app.Services.GetRequiredService<SpaceDeleteSubscriber>(),
                    app.Services.GetRequiredService<UserDeleteSubscriber>()
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
