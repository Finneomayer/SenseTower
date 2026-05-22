using MediatR;
// ReSharper disable PropertyCanBeMadeInitOnly.Global

namespace SenseWebApi1.Features.TicketFeature.SellTicketForUser;

public class SellTicketForUserCommand:IRequest<Ticket>
{
    public Guid TicketId { get; set; }
    
    public Guid OwnerId { get; set; }
}