using MediatR;
using MongoDB.Driver;
using SC.Internship.Common.Exceptions;
using SenseWebApi1.Features.EventFeature;
using SenseWebApi1.MongoDB;

namespace SenseWebApi1.Features.TicketFeature.GetTicket;

// ReSharper disable once UnusedType.Global
public class GetTicketsHandler : IRequestHandler<GetTicketsCommand, List<Ticket>>
{
        
    private readonly IMongoDbContext _databaseContext;
    public GetTicketsHandler(IMongoDbContext databaseContext)
    {
                
        _databaseContext = databaseContext;
    }
    public async Task<List<Ticket>> Handle(GetTicketsCommand request, CancellationToken cancellationToken)
    {
        var mongoCollection = _databaseContext.GetMongoDatabase().GetCollection<Event>("Events");
            
        var eventObj = await mongoCollection.Find(p => p.EventId==request.EventId).FirstOrDefaultAsync(cancellationToken: cancellationToken);

        if (eventObj == null)
        {
            throw new ScException("Такого мероприятия нет");
        }
            
        var tickets =  eventObj.Tickets;
    
        return tickets!;
            
            
    }
}