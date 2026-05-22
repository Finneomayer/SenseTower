using MongoDB.Driver;
using SenseWebApi1.Features.EventFeature;
using SenseWebApi1.MongoDB;


namespace SenseWebApi1.Context;

public class EventContext : IEventContext
{
        
    private readonly IMongoDbContext _databaseContext;

    public EventContext(IMongoDbContext databaseContext)
    {
        _databaseContext = databaseContext;

        databaseContext.EventIniz();
            
    }
        

    public async Task UpdateEventForImage(Guid imageId)
    {
        var filterForSearch = Builders<Event>.Filter.Where(p => p.ImageId == imageId);
        var mongoCollection = _databaseContext.GetMongoDatabase().GetCollection<Event>("Events");
        var events = await mongoCollection.Find(filterForSearch).ToListAsync();
        foreach (var eventObj in events)
        {
            eventObj.ImageId = null;
        }

        var builder = Builders<Event>.Update;
        await mongoCollection.UpdateManyAsync(x => x.ImageId ==imageId, builder.Set(p=>p.ImageId,null));
            
    }

    public async Task DeleteEventForSpace(Guid spaceId)
    {
        var filterForSearch = Builders<Event>.Filter.Where(p => p.AreaId == spaceId);
        var mongoCollection = _databaseContext.GetMongoDatabase().GetCollection<Event>("Events");
        await mongoCollection.DeleteManyAsync(filterForSearch);
    }
}