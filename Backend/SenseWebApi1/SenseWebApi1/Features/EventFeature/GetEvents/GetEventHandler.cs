using AutoMapper;
using MediatR;
using MongoDB.Driver;
using SenseWebApi1.MongoDB;

namespace SenseWebApi1.Features.EventFeature.GetEvents;

// ReSharper disable once UnusedType.Global
public class GetEventCommandHandler : IRequestHandler<GetEventCommand, IEnumerable<EventDto>>
{

    
    private readonly IMapper _mapper;
    private readonly IMongoDbContext _databaseContext;
    public GetEventCommandHandler(IMapper mapper,IMongoDbContext databaseContext)
    {
        _mapper = mapper;
        
        _databaseContext = databaseContext;
    }
    public async Task<IEnumerable<EventDto>> Handle(GetEventCommand request, CancellationToken cancellationToken)
    {
        var mongoCollection = _databaseContext.GetMongoDatabase().GetCollection<Event>("Events");
        var eventsCollection = await mongoCollection.Find(_ => true).ToListAsync(cancellationToken: cancellationToken);
        var result = eventsCollection.Select(entity => _mapper.Map<Event, EventDto>(entity)).ToList();
        return result;
    }
}