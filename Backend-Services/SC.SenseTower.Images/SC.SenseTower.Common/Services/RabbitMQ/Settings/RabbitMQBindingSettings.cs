namespace SC.SenseTower.Common.Services.RabbitMQ.Settings
{
    public class RabbitMQBindingSettings
    {
        public string Exchange { get; set; } = null!;
        public string RoutingKey { get; set; } = null!;
        public string QueueName { get; set; } = null!;
        public string Type { get; set; } = "topic";
        public bool AutoAck { get; set; } = true;
        public bool IsSynchronously { get; set; } = false;
    }
}
