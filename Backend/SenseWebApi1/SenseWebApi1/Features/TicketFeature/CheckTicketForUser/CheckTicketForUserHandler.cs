using MediatR;
using MongoDB.Driver;
using SenseWebApi1.Features.EventFeature;
using SenseWebApi1.MongoDB;

namespace SenseWebApi1.Features.TicketFeature.CheckTicketForUser;

// ReSharper disable once UnusedType.Global
public class CheckTicketForUserHandler : IRequestHandler<CheckTicketForUserCommand, bool>
{
       
    private readonly IMongoDbContext _databaseContext;
    public CheckTicketForUserHandler(IMongoDbContext databaseContext)
    {
            
        _databaseContext = databaseContext;
    }

    public async Task<bool> Handle(CheckTicketForUserCommand request, CancellationToken cancellationToken)
    {
        var mongoCollection = _databaseContext.GetMongoDatabase().GetCollection<Event>("Events");
        var eventObj = await mongoCollection.Find(p=>p.EventId==request.EventId).FirstOrDefaultAsync(cancellationToken: cancellationToken);
#pragma warning disable CS8604
            
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        return (eventObj?.Tickets).FirstOrDefault(p => p.OwnerId == request.OwnerId) != null;
            
            
    }
}