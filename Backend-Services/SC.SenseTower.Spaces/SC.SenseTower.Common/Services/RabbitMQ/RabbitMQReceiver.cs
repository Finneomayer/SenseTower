using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using SC.SenseTower.Common.Services.RabbitMQ.Settings;

namespace SC.SenseTower.Common.Services.RabbitMQ
{
    public class RabbitMQReceiver
    {
        private readonly RabbitMQConnectionSettings settings;
        private readonly IServiceProvider services;
        private readonly ILogger<RabbitMQReceiver> logger;

        private ConnectionFactory? connectionFactory = null;
        private List<IModel> channels = new();
        private IConnection? connection = null;

        private Task? task = null;
        private CancellationTokenSource tokenSource = new();
        private CancellationToken cancellationToken => tokenSource.Token;

        public RabbitMQReceiver(
            ILogger<RabbitMQReceiver> logger,
            IServiceProvider services,
            IOptions<RabbitMQConnectionSettings> options)
        {
            this.logger = logger;
            this.services = services;
            settings = options.Value;
        }

        ~RabbitMQReceiver()
        {
            if (task?.Status == TaskStatus.Running)
            {
                tokenSource.Cancel();
                task.Wait();
            }
        }

        public void Start(IEnumerable<IRabbitMQSubscriber> subscribers)
        {
            connectionFactory = new ConnectionFactory
            {
                HostName = settings.HostName,
                Port = settings.Port,
                VirtualHost = "/",
                AmqpUriSslProtocols = System.Security.Authentication.SslProtocols.Tls12,
                UserName = settings.UserName,
                Password = settings.Password,
                AutomaticRecoveryEnabled = true
            };
            task = Task.Run(() => Connect(cancellationToken)).ContinueWith((state) => Task.Run(() => Subscribe(subscribers)));
        }

        private void Connect(CancellationToken cancellationToken)
        {
            connection = null;
            do
            {
                try
                {
                    connection = connectionFactory.CreateConnection();
                }
                catch (BrokerUnreachableException)
                {
                    Task.Delay(5000);
                }
                catch (Exception ex)
                {
                    logger.LogCritical($"Соединение с RabbitMQ на прослушивание не установлено: {ex.Message}");
                    Task.Delay(60000);
                }
            } while (connection == null && !cancellationToken.IsCancellationRequested);
            if (connection != null)
            {
                logger.LogInformation("Установлено соединение с RabbitMQ на прослушивание");
            }
        }

        private void Subscribe(IEnumerable<IRabbitMQSubscriber> subscribers)
        {
            foreach (var subscriber in subscribers)
            {
                var settings = subscriber.Settings;

                if (string.IsNullOrEmpty(settings.Exchange) && string.IsNullOrEmpty(settings.QueueName) && string.IsNullOrEmpty(settings.RoutingKey))
                {
                    logger.LogCritical($"Подписчик RabbitMQ \"{subscriber.GetType()}\" проигнорирован из-за нехватки настроек");
                    continue;
                }

                try
                {
                    var channel = connection.CreateModel();
                    channel.ExchangeDeclare(settings.Exchange, settings.Type, true);
                    var queue = channel.QueueDeclare();
                    channel.QueueBind(queue.QueueName, settings.Exchange, settings.RoutingKey);
                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += subscriber.ProcessMessage;
                    channel.BasicConsume(consumer, queue.QueueName, subscriber.Settings.AutoAck);
                    channels.Add(channel);
                    logger.LogInformation($"Прослушивание RabbitMQ успешно запущено: Exchange = {settings.Exchange}; QueueName = {queue.QueueName}; RK = {settings.RoutingKey}; Type = {settings.Type}");
                }
                catch (Exception ex)
                {
                    logger.LogCritical(ex, $"Ошибка при запуске прослушивания RabbitMQ: Exchange = {settings.Exchange}; RK = {settings.RoutingKey}; Type = {settings.Type}");
                }
            }
        }

        public void Stop()
        {
            channels.ForEach(x => x.Close());
            if (connection?.IsOpen ?? false)
                connection.Close();
            logger.LogInformation("RabbitMQReceiver успешно остановлен");
        }
    }
}
