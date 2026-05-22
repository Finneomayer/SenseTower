using MediatR;

namespace SenseWebApi1.Features.TicketFeature.CheckTicketForUser;

public class CheckTicketForUserCommand : IRequest<bool>
{
    // ReSharper disable once PropertyCanBeMadeInitOnly.Global
    public Guid OwnerId { get; set; }

    // ReSharper disable once PropertyCanBeMadeInitOnly.Global
    public Guid EventId { get; set; }
}