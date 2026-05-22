using MediatR;

namespace SenseWebApi1.Features.TicketFeature.GiveTicketForUser;

public class GiveTicketForUserCommand : IRequest<Guid>
{
    // ReSharper disable once PropertyCanBeMadeInitOnly.Global
    public Guid TicketId { get; set; }

    // ReSharper disable once PropertyCanBeMadeInitOnly.Global
    public Guid OwnerId { get; set; }
        
        
}