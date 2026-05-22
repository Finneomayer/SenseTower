using MediatR;
using MongoDB.Driver;
using SC.Internship.Common.Exceptions;
using SenseWebApi1.Features.EventFeature;
using SenseWebApi1.MongoDB;

namespace SenseWebApi1.Features.TicketFeature.AddFreeTickets;

// ReSharper disable once UnusedType.Global
public class AddFreeTicketsHandler : IRequestHandler<AddTicketsCommand, bool>
{
        
    private readonly IMongoDbContext _databaseContext;
    public AddFreeTicketsHandler(IMongoDbContext databaseContext)
    {
            
        _databaseContext = databaseContext;
    }
    public async Task<bool> Handle(AddTicketsCommand request, CancellationToken cancellationToken)
    {
            
        try
        {
            var mongoCollection = _databaseContext.GetMongoDatabase().GetCollection<Event>("Events");
            var eventObj = await mongoCollection.Find(p => p.EventId == request.EventId).FirstOrDefaultAsync(cancellationToken: cancellationToken);
            if (eventObj != null)
            {
                if (eventObj.IsHavePlaces)
                {
                    var tickets = (await mongoCollection.Find(p => p.EventId == request.EventId).FirstOrDefaultAsync(cancellationToken: cancellationToken)).Tickets;
#pragma warning disable CS8604
                    var maxPlace = tickets.Max(p => p.Place);
#pragma warning restore CS8604

                    for (var i = maxPlace; i < maxPlace + request.Countoftickets; i++)
                    {

                        tickets.Add(
                            new Ticket
                            {
                                TicketId = Guid.NewGuid(),
                                Place = i + 1
                            });
                    }

                    eventObj.Tickets = tickets;
                }
                else
                {
                    var tickets = (await mongoCollection.Find(p => p.EventId == request.EventId).FirstOrDefaultAsync(cancellationToken: cancellationToken)).Tickets;
#pragma warning disable CS8604
                    
                    var maxPlace = tickets.Max(p => p.Place);
#pragma warning restore CS8604

                    for (var i = maxPlace; i < maxPlace + request.Countoftickets; i++)
                    {

                        tickets.Add(
                            new Ticket
                            {
                                TicketId = Guid.NewGuid()
                            });
                    }

                    eventObj.Tickets = tickets;
                }
            

            }
            else
            {
                throw new ScException("Такого мерприятия нет");
            }


            await UpdateEvent(eventObj);
            return true;
        }
        catch
        {
            return false;
        }
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