using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using SC.SenseTower.Auth.Constants;
using SC.SenseTower.Auth.Settings;
using System.Text;
using System.Text.Json;

namespace SC.SenseTower.Auth.Services.RabbitMQ
{
    public class RabbitMQService
    {
        private readonly ILogger<RabbitMQService> _logger;

        private readonly IConnectionFactory connectionFactory;
        private IConnection connection;
        private Guid connectionId;
        private IModel channel;

        private static readonly SemaphoreSlim connectionSemaphore = new(1, 1);
        private static readonly SemaphoreSlim channelSemaphore = new(1, 1);

        public RabbitMQService(ILogger<RabbitMQService> logger, IOptions<RabbitMQSettings> options)
        {
            _logger = logger;
            var settings = options.Value;
            connectionFactory = new ConnectionFactory
            {
                HostName = settings.HostName,
                Port = AmqpTcpEndpoint.UseDefaultPort,
                VirtualHost = "/",
                AmqpUriSslProtocols = System.Security.Authentication.SslProtocols.Tls12,
                UserName = settings.UserName,
                Password = settings.Password,
                AutomaticRecoveryEnabled = true
            };
        }

        ~RabbitMQService()
        {
            channel?.Close();
            connection?.Close();
        }

        public async Task Start(CancellationToken cancellationToken)
        {
            await TryConnect(cancellationToken);
            await OpenChannel(cancellationToken);
        }

        public async Task Send(string exchangeName, string routingKey, object data, CancellationToken cancellationToken)
        {
            await TryConnect(cancellationToken);
            await OpenChannel(cancellationToken);
            if (connection == null || !connection.IsOpen || channel == null || !channel.IsOpen)
                return;

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
                    basicProperties: channel.CreateBasicProperties(),
                    body: messageBytes);
                _logger.LogDebug($"Сообщение в RabbitMQ в канал {channel.ChannelNumber} соединения {connectionId} (ExchangeName = {exchangeName}; RK = {routingKey}) успешно отправлено: {message}");
            }
            catch (Exception ex)
            {
                _logger.LogCritical(
                    ex,
                    $"Непредвиденная ошибка при отправке сообщения в RabbitMQ в канал {channel?.ChannelNumber ?? -1} соединения {connectionId}: ExchangeName = {exchangeName}; RK = {routingKey}; Message = \"{message}\"");
            }
        }

        private async Task TryConnect(CancellationToken cancellationToken)
        {
            if (connection is { IsOpen: true })
                return;

            try
            {
                await connectionSemaphore.WaitAsync(cancellationToken);
                if (connection is { IsOpen: true })
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
                connection.ConnectionShutdown += (sender, args) =>
                {
                    _logger.LogInformation($"Соединение с RabbitMQ с ид {connectionId} закрыто: {args.Initiator} - {args.ReplyCode} ({args.ReplyText})");
                };
                connectionId = Guid.NewGuid();
                _logger.LogInformation($"Открыто соединение с RabbitMQ с ид {connectionId}");
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Ошибка подключения к RabbitMQ");
                throw;
            }
            finally
            {
                connectionSemaphore.Release();
            }
        }

        private async Task OpenChannel(CancellationToken cancellationToken)
        {
            if (channel is { IsOpen: true })
                return;

            try
            {
                await channelSemaphore.WaitAsync(cancellationToken);
                if (channel is { IsOpen: true })
                    return;

                channel = connection.CreateModel();
                channel.ModelShutdown += (sender, args) =>
                {
                    _logger.LogInformation($"В соединении RabbitMQ с ид {connectionId} закрыт канал обмена сообщениями: {args.Initiator} - {args.ReplyCode} ({args.ReplyText})");
                };
                channel.CallbackException += (sender, args) =>
                {
                    _logger.LogError(args.Exception, $"Ошибка в канале обмена сообщениями в очереди RabbitMQ с ид {connectionId}: {string.Join("; ", args.Detail.Select(x => $"{x.Key}: {x.Value}"))}");
                };
                _logger.LogInformation($"В соединении RabbitMQ с ид {connectionId} создан канал обмена сообщениями номер {channel.ChannelNumber}");

                channel.ExchangeDeclare(RabbitMQConstants.DEFAULT_EXCHANGE, "topic", true, false, null);
                _logger.LogInformation($"В соединении RabbitMQ с ид {connectionId} в канале обмена сообщениями номер {channel.ChannelNumber} создан обменник {RabbitMQConstants.DEFAULT_EXCHANGE}");
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, $"Ошибка открытия канала обмена сообщениями RabbitMQ по соединению с ид {connectionId}");
            }
            finally
            {
                channelSemaphore.Release();
            }
        }
    }
}
