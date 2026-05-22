using System.Text;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace SenseWebApi1.RMQ;

public class RmqSender:IRmqSender
{
    private readonly IModel _channel;
    // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
    private readonly IOptions<RmqOptions> _options;
    

    public RmqSender(IOptions<RmqOptions> options)
    {
        _options = options;
        
        
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

    public async Task SendDeleteEvent(Guid id)
    {
        var deletedImage = new
        {
            Id = id,
            Type = 3
        };
        var json = JsonConvert.SerializeObject(deletedImage);
        var sendBody = Encoding.UTF8.GetBytes(json);
        _channel.BasicPublish(exchange: "", routingKey: "deletedevents", body: sendBody);
        await Task.CompletedTask;
        

       

    }
}