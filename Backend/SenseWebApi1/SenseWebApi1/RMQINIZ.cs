using RabbitMQ.Client;

namespace SenseWebApi1;

// ReSharper disable once InconsistentNaming
// ReSharper disable once IdentifierTypo
public static class RMQINIZ
{
    public static void CreateListener()
    {
        var factory = new ConnectionFactory
        {
            HostName = "rabbitmq",
            Port = 5672,
            UserName = "user",
            Password = "password",
            VirtualHost = "/"
        };
        IConnection connection;
        while (true)
        {
            Thread.Sleep(5000);
            try
            {
                connection = factory.CreateConnection();
                break;
            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch
            {
                
            }
        }
        
            
        
        
        var channel = connection.CreateModel();
        channel.QueueDeclare("deletedevents",
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);
    }
}