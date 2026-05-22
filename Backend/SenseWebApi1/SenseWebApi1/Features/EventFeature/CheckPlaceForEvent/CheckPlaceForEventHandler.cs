using MediatR;
using MongoDB.Driver;
using SenseWebApi1.MongoDB;

namespace SenseWebApi1.Features.EventFeature.CheckPlaceForEvent;

// ReSharper disable once UnusedType.Global
public class CheckPlaceForEventHandler : IRequestHandler<CheckPlaceForEventCommand, bool>
{
        
    private readonly IMongoDbContext _databaseContext;
    public CheckPlaceForEventHandler(IMongoDbContext databaseContext) 
    {
            
        _databaseContext = databaseContext;
    }
    public async Task<bool> Handle(CheckPlaceForEventCommand request, CancellationToken cancellationToken)
    {
        var mongoCollection = _databaseContext.GetMongoDatabase().GetCollection<Event>("Events");
        var eventObj = await mongoCollection.Find(p => p.EventId == request.eventId).FirstOrDefaultAsync(cancellationToken: cancellationToken);
#pragma warning disable CS8604
        var placeObj = eventObj.Tickets.FirstOrDefault(p=>p.Place==request.place);
#pragma warning restore CS8604
        return placeObj != null;
            
    }
}