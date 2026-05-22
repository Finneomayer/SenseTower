namespace SC.SenseTower.Common.Services.RabbitMQ.Settings
{
    public class RabbitMQConnectionSettings
    {
        public string HostName { get; set; } = null!;

        public int Port { get; set; }

        public string UserName { get; set; } = null!;

        public string Password { get; set; } = null!;

        public string[] Exchanges { get; set; } = Array.Empty<string>();
    }
}
