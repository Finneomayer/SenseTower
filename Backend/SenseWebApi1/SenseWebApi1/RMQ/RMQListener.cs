using System.Text;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SC.Internship.Common.Exceptions;
using SenseWebApi1.Context;

namespace SenseWebApi1.RMQ;

// ReSharper disable once InconsistentNaming
public class RMQListener:BackgroundService
{
    private readonly IModel _channel;
    // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
    private readonly IOptions<RmqOptions> _options;
    private readonly IEventContext _eventContext;
    
    public RMQListener(IOptions<RmqOptions> options,IEventContext eventContext)
    {
        _options = options;
        _eventContext = eventContext;
        
        var factory = new ConnectionFactory 
            { 
                HostName = _options.Value.HostName, 
                Port = _options.Value.Port,
                UserName = _options.Value.UserName,
                Password = _options.Value.Password,
                VirtualHost = _options.Value.VirtualHost
            };
        var connection = factory.CreateConnection();
        _channel = connection.CreateModel();
    }
    
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new EventingBasicConsumer(_channel);
        // ReSharper disable once UnusedParameter.Local
        consumer.Received += (model, eventArgs) =>
        {
            var body = eventArgs.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var obj = JsonConvert.DeserializeObject<RmqDto>(message)!;
            switch (obj.Type)
            {
                case RmqType.ImageDeleteEvent:
                    _eventContext.UpdateEventForImage(obj.Id);
                    break;
                case RmqType.SpaceDeleteEvent:
                    _eventContext.DeleteEventForSpace(obj.Id);
                    break;
                default:
                    throw new ScException("Проблемы с RMQ");
            }
            
        };
        _channel.BasicConsume(queue: "deletedevents", autoAck: true, consumer: consumer);
        return Task.CompletedTask;
    }
    
}