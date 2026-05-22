using MediatR;
using SC.Internship.Common.Exceptions;
using SenseWebApi1.MongoDB;
using SenseWebApi1.Services;

namespace SenseWebApi1.Features.EventFeature.CreateEvent;

// ReSharper disable once UnusedType.Global
public class EventCreateCommandHandler : IRequestHandler<EventCreateCommand, Guid>
{
    private readonly IHttpService _httpService;
    private readonly IMongoDbContext _databaseContext;

    public EventCreateCommandHandler(IMongoDbContext databaseContext,IHttpService httpService)
    {
           
        _databaseContext = databaseContext;
        _httpService = httpService;

    }
    public async Task<Guid> Handle(EventCreateCommand command, CancellationToken cancellationToken)
    {
        var @event = new Event
        {
            EventId = Guid.NewGuid(),
            AreaId = command.AreaId,
            ImageId = command.ImageId,
            End = command.End,
            Beginning = command.Beginning,
            Description = command.Description,
            EventName = command.EventName,
            IsHavePlaces = command.IsHavePlaces,
            Cost = command.Cost
        };
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (_httpService.GetImages(command.ImageId).Result == null) throw new ScException("Изображение не найдено");
        if (_httpService.GetSpaces(command.AreaId).Result == null) throw new ScException("Пространство не найдено");
        var mongoCollection = _databaseContext.GetMongoDatabase().GetCollection<Event>("Events");
        await mongoCollection.InsertOneAsync(@event, cancellationToken: cancellationToken);
        return @event.EventId;

    }
}