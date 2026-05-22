using MediatR;
// ReSharper disable UnusedMember.Global

namespace SenseWebApi1.Features.TicketFeature.GetTicket;

public class GetTicketsCommand : IRequest<List<Ticket>>
{
        
    public Guid TicketId { get; set; }

    public Guid? OwnerId { get; set; }

    public string? Place { get; set; }

    // ReSharper disable once PropertyCanBeMadeInitOnly.Global
    public Guid EventId { get; set; }
}