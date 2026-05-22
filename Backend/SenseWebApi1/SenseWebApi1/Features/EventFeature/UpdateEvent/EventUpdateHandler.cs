using AutoMapper;
using MediatR;
using MongoDB.Driver;
using SenseWebApi1.MongoDB;


namespace SenseWebApi1.Features.EventFeature.UpdateEvent;

// ReSharper disable once UnusedType.Global
public class EventUpdateHandler : IRequestHandler<EventUpdateCommand, EventDto>
{
       
    private readonly IMapper _mapper;
    private readonly IMongoDbContext _databaseContext;
    public EventUpdateHandler( IMapper mapper,IMongoDbContext databaseContext)
    {
            
        _mapper = mapper;
        _databaseContext = databaseContext;

    }
    public async Task<EventDto> Handle(EventUpdateCommand request, CancellationToken cancellationToken)
    {
        var mongoCollection = _databaseContext.GetMongoDatabase().GetCollection<Event>("Events");
        var filter = Builders<Event>.Filter
            .Where(p=>p.EventId==request.EventId);
        var eventObj =  mongoCollection.Find(filter).FirstOrDefault();
        if (eventObj == null)
        {
            throw new Exception("Такого события нет");
        }
        var update = Builders<Event>.Update
                    
            .Set(p => p.Beginning, request.Beginning)
            .Set(p => p.End, request.End)
            .Set(p => p.AreaId, request.AreaId)
            .Set(p => p.Description, request.Description)
            .Set(p => p.ImageId, request.ImageId)
            .Set(p => p.IsHavePlaces, request.IsHavePlaces)
            .Set(p=>p.Cost,request.Cost);
        await mongoCollection.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);
            
        var updateEvent = _mapper.Map<EventUpdateCommand, Event>(request);
        var result = _mapper.Map<Event, EventDto>(updateEvent);
        return result;
            
    }
}