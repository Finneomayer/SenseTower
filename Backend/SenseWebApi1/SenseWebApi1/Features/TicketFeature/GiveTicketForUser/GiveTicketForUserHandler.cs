using MediatR;
using MongoDB.Driver;
using SC.Internship.Common.Exceptions;
using SenseWebApi1.Features.EventFeature;
using SenseWebApi1.MongoDB;

namespace SenseWebApi1.Features.TicketFeature.GiveTicketForUser;

// ReSharper disable once UnusedType.Global
public class GiveTicketForUserHandler:IRequestHandler<GiveTicketForUserCommand,Guid>
{
        
    private readonly IMongoDbContext _databaseContext;
    public GiveTicketForUserHandler(IMongoDbContext databaseContext)
    {
            
        _databaseContext = databaseContext;
    }

    public async Task<Guid> Handle(GiveTicketForUserCommand request, CancellationToken cancellationToken)
    {
        var filterForSearch = Builders<Event>.Filter.ElemMatch(p => p.Tickets, t => t.TicketId == request.TicketId);
        var mongoCollection = _databaseContext.GetMongoDatabase().GetCollection<Event>("Events");
        var eventObj = await mongoCollection.Find(filterForSearch).FirstOrDefaultAsync(cancellationToken: cancellationToken);
#pragma warning disable CS8604
        var ticket = eventObj.Tickets.FirstOrDefault(p => p.TicketId==request.TicketId);
        if (ticket != null)
        {
                
            ticket.OwnerId = request.OwnerId;
                
#pragma warning restore CS8604
            await UpdateEvent(eventObj);
        }
        else
        {
            throw new ScException("Такой билет не найден");
        }
        return request.TicketId;
           
    }
        
    private  async Task UpdateEvent(Event @event)
    {
        var mongoCollection = _databaseContext.GetMongoDatabase().GetCollection<Event>("Events");
        var filter = Builders<Event>.Filter
            .Where(p=>p.EventId==@event.EventId);
            
        var updateWithTick = Builders<Event>.Update
                    
            .Set(p => p.Beginning, @event.Beginning)
            .Set(p => p.End, @event.End)
            .Set(p => p.AreaId, @event.AreaId)
            .Set(p => p.Description, @event.Description)
            .Set(p => p.ImageId, @event.ImageId)
            .Set(p => p.Tickets, @event.Tickets)
            .Set(p => p.IsHavePlaces, @event.IsHavePlaces)
            .Set(p=>p.Cost,@event.Cost);
        await mongoCollection.UpdateOneAsync(filter, updateWithTick);
            
            
           
    }
}