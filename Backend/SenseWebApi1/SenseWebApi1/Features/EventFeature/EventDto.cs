
using SenseWebApi1.Features.TicketFeature;

namespace SenseWebApi1.Features.EventFeature;

public class EventDto
{
        
    public Guid EventId { get; set; }

    public DateTime Beginning { get; set; }

    public DateTime End { get; set; }

    public string? EventName { get; set; }

    public string? Description { get; set; }

    public Guid? ImageId { get; set; }

    public Guid AreaId { get; set; }

    // ReSharper disable once CollectionNeverQueried.Global
    public List<Ticket>? Tickets { get; set; }
        
    public decimal Cost { get; set; } 

    public bool IsHavePlaces { get; set; }
}