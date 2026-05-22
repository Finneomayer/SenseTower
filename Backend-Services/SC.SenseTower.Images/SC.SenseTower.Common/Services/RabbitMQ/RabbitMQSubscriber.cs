using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client.Events;
using SC.SenseTower.Common.Exceptions;
using SC.SenseTower.Common.Services.RabbitMQ.Settings;
using System.Text;
using System.Text.Json;

namespace SC.SenseTower.Common.Services.RabbitMQ
{
    public class RabbitMQSubscriber<TData, TReturnValue> : IRabbitMQSubscriber where TData : IRequest<TReturnValue>
    {
        public RabbitMQBindingSettings Settings { get; private set; }

        private readonly ILogger logger;
        private readonly IServiceProvider services;

        public RabbitMQSubscriber(
            ILogger logger,
            IServiceProvider services,
            IOptions<RabbitMQBindingSettings> options)
        {
            this.logger = logger;
            this.services = services;
            Settings = options.Value;
        }

        public async void ProcessMessage(object sender, BasicDeliverEventArgs args)
        {
            try
            {
                var message = Encoding.UTF8.GetString(args.Body.ToArray());
                var command = JsonSerializer.Deserialize<TData>(message, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                using var scope = services.CreateScope();
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                if (Settings.IsSynchronously)
                {
                    mediator.Send(command).GetAwaiter().GetResult();
                }
                else
                {
                    await mediator.Send(command);
                }
            }
            catch (ScException ex)
            {
                logger.LogError(ex, "Непредвиденная ошибка при обработке сообщения RabbitMQ " +
                    $"(Exchange = {Settings.Exchange}; QueueName = {Settings.QueueName}; " +
                    $"RK = {Settings.RoutingKey}; Type = {Settings.Type}), сообщение удалено из очереди");

                if (!Settings.AutoAck)
                    ((EventingBasicConsumer)sender).Model.BasicAck(args.DeliveryTag, false);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Непредвиденная ошибка при обработке сообщения RabbitMQ " +
                    $"(Exchange = {Settings.Exchange}; QueueName = {Settings.QueueName}; " +
                    $"RK = {Settings.RoutingKey}; Type = {Settings.Type}), сообщение осталось в очереди");
            }
        }
    }
}
