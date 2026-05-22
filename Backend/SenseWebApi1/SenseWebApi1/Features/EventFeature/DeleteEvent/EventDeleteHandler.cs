using MediatR;
using MongoDB.Driver;
using SenseWebApi1.MongoDB;
using SenseWebApi1.RMQ;

namespace SenseWebApi1.Features.EventFeature.DeleteEvent;

// ReSharper disable once UnusedType.Global
public class EventDeleteCommandHandler : IRequestHandler<EventDeleteCommand, Guid>
{
        
    private readonly IRmqSender _rmqSender;
    private readonly IMongoDbContext _databaseContext;
    public EventDeleteCommandHandler(IRmqSender rmqSender,IMongoDbContext databaseContext)
    {
            
        _rmqSender = rmqSender;
        _databaseContext = databaseContext;
    }
    public async Task<Guid> Handle(EventDeleteCommand request, CancellationToken cancellationToken)
    {
        var mongoCollection = _databaseContext.GetMongoDatabase().GetCollection<Event>("Events");
        var filter = Builders<Event>.Filter
            .Where(p=>p.EventId==request.EventId);
        var eventObj =  mongoCollection.Find(filter).FirstOrDefault();
        if (eventObj == null)
        {
            throw new Exception("Такого события нет");
        }
        await mongoCollection.DeleteOneAsync(Builders<Event>.Filter.Where(p=>p.EventId==request.EventId), cancellationToken);
        await _rmqSender.SendDeleteEvent(request.EventId);
        return request.EventId;
    }
}