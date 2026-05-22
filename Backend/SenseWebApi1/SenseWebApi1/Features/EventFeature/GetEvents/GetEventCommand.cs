using MediatR;
using SenseWebApi1.Features.TicketFeature;
// ReSharper disable UnusedMember.Global

namespace SenseWebApi1.Features.EventFeature.GetEvents;

public class GetEventCommand : IRequest<IEnumerable<EventDto>>
{
    public Guid EventId { get; set; }
        
    public DateTime Beginning { get; set; }

    public DateTime End { get; set; }

    public string? EventName { get; set; }

    public string? Description { get; set; }

    public Guid ImageId { get; set; }

    public Guid AreaId { get; set; }
        
    public List<Ticket>?  Tickets { get; set; }
}