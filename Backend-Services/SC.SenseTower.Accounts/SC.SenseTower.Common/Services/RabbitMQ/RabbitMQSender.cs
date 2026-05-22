using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using SC.SenseTower.Common.Services.RabbitMQ.Settings;
using System.Text;
using System.Text.Json;

namespace SC.SenseTower.Common.Services.RabbitMQ
{
    public class RabbitMQSender
    {
        private readonly ILogger<RabbitMQSender> logger;
        private readonly RabbitMQConnectionSettings settings;

        private IConnectionFactory? connectionFactory = null;
        private IConnection? connection = null;
        private Guid connectionId;
        private IModel? channel = null;

        private static readonly SemaphoreSlim connectionSemaphore = new(1, 1);
        private static readonly SemaphoreSlim channelSemaphore = new(1, 1);

        public RabbitMQSender(ILogger<RabbitMQSender> logger, IOptions<RabbitMQConnectionSettings> options)
        {
            this.logger = logger;
            settings = options.Value;
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
        }

        ~RabbitMQSender()
        {
            channel?.Close();
            connection?.Close();
        }

        public async Task Start(CancellationToken cancellationToken)
        {
            await TryConnect(cancellationToken);
            await OpenChannel(cancellationToken);
        }

        public async Task Send(string exchangeName, string routingKey, object? data, CancellationToken cancellationToken)
        {
            await TryConnect(cancellationToken);
            await OpenChannel(cancellationToken);

            var message = string.Empty;

            try
            {
                if (data != null)
                {
                    message = JsonSerializer.Serialize(data);
                }
                var messageBytes = Encoding.UTF8.GetBytes(message);
                channel.BasicPublish(exchange: exchangeName,
                    routingKey: routingKey,
                    basicProperties: channel?.CreateBasicProperties(),
                    body: messageBytes);
                logger.LogDebug($"Сообщение в RabbitMQ в канал {channel?.ChannelNumber} соединения {connectionId} (ExchangeName = {exchangeName}; RK = {routingKey}) успешно отправлено: {message}");
            }
            catch (Exception ex)
            {
                logger.LogCritical(
                    ex,
                    $"Непредвиденная ошибка при отправке сообщения в RabbitMQ в канал {channel?.ChannelNumber ?? -1} соединения {connectionId}: ExchangeName = {exchangeName}; RK = {routingKey}; Message = \"{message}\"");
            }
        }

        private async Task TryConnect(CancellationToken cancellationToken)
        {
            if (connectionFactory == null || (connection != null && connection.IsOpen))
                return;

            try
            {
                await connectionSemaphore.WaitAsync(cancellationToken);
                if (connection != null && connection.IsOpen)
                    return;

                if (channel != null)
                {
                    if (channel.IsOpen)
                    {
                        channel.Close();
                    }
                    channel.Dispose();
                    channel = null;
                }

                connection = connectionFactory.CreateConnection();
                connection.ConnectionShutdown += (_, args) =>
                {
                    logger.LogInformation($"Соединение с RabbitMQ с ид {connectionId} закрыто: {args.Initiator} - {args.ReplyCode} ({args.ReplyText})");
                };
                connectionId = Guid.NewGuid();
                logger.LogInformation($"Открыто соединение с RabbitMQ с ид {connectionId}");
            }
            catch (Exception ex)
            {
                logger.LogCritical(ex, "Ошибка подключения к RabbitMQ");
            }
            finally
            {
                connectionSemaphore.Release();
            }
        }

        private async Task OpenChannel(CancellationToken cancellationToken)
        {
            if (connection == null || (channel != null && channel.IsOpen))
                return;

            try
            {
                await channelSemaphore.WaitAsync(cancellationToken);
                if (channel != null && channel.IsOpen)
                    return;

                channel = connection.CreateModel();
                channel.ModelShutdown += (_, args) =>
                {
                    logger.LogInformation($"В соединении RabbitMQ с ид {connectionId} закрыт канал обмена сообщениями: {args.Initiator} - {args.ReplyCode} ({args.ReplyText})");
                };
                channel.CallbackException += (_, args) =>
                {
                    logger.LogError(args.Exception, $"Ошибка в канале обмена сообщениями в очереди RabbitMQ с ид {connectionId}: {string.Join("; ", args.Detail.Select(x => $"{x.Key}: {x.Value}"))}");
                };
                logger.LogInformation($"В соединении RabbitMQ с ид {connectionId} создан канал обмена сообщениями");

                foreach (var exchange in settings.Exchanges)
                {
                    channel.ExchangeDeclare(exchange, "topic", true, false, null);
                    logger.LogInformation($"В соединении RabbitMQ с ид {connectionId} в канале обмена сообщениями номер {channel.ChannelNumber} создан обменник {exchange}");
                }
            }
            catch (Exception ex)
            {
                logger.LogCritical(ex, $"Ошибка открытия канала обмена сообщениями RabbitMQ по соединению с ид {connectionId}");
            }
            finally
            {
                channelSemaphore.Release();
            }
        }
    }
}
