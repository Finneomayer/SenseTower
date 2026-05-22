using SC.SenseTower.Spaces.Data.Models;

namespace SC.SenseTower.Spaces.RabbitMQ;

public interface IRabbitMQService
{
    Task SendUpdateSpaceMessage(Space space, CancellationToken cancellationToken);
    Task SendDeleteSpaceMessage(Guid spaceId, CancellationToken cancellationToken);
    Task Start(CancellationToken cancellationToken);
    Task Send(string exchangeName, string routingKey, object? data, CancellationToken cancellationToken);
    Task TryConnect(CancellationToken cancellationToken);
    Task OpenChannel(CancellationToken cancellationToken);
}