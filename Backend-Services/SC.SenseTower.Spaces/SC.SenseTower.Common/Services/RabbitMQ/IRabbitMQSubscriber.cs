using RabbitMQ.Client.Events;
using SC.SenseTower.Common.Services.RabbitMQ.Settings;

namespace SC.SenseTower.Common.Services.RabbitMQ
{
    public interface IRabbitMQSubscriber
    {
        RabbitMQBindingSettings Settings { get; }

        void ProcessMessage(object sender, BasicDeliverEventArgs args);
    }
}
