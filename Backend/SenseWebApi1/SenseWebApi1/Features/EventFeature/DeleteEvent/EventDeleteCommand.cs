using MediatR;

namespace SenseWebApi1.Features.EventFeature.DeleteEvent;

public class EventDeleteCommand : IRequest<Guid>
{
    // ReSharper disable once PropertyCanBeMadeInitOnly.Global
    public Guid EventId { get; set; }
}