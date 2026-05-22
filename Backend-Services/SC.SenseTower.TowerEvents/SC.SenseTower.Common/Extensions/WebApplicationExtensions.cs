using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using SC.SenseTower.Common.Services.RabbitMQ;

namespace SC.SenseTower.Common.Extensions
{
    public static class WebApplicationExtensions
    {
        public static WebApplication StopUsingRabbitMQ(this WebApplication app)
        {
            var receiver = app.Services.GetRequiredService<RabbitMQReceiver>();
            receiver?.Stop();
            return app;
        }
    }
}
