using MediatR;

namespace SenseWebApi1.Features.TicketFeature.AddFreeTickets;

public class AddTicketsCommand : IRequest<bool>
{

    // ReSharper disable once PropertyCanBeMadeInitOnly.Global
    public Guid EventId { get; set; }

    // ReSharper disable once IdentifierTypo
    // ReSharper disable once PropertyCanBeMadeInitOnly.Global
    public int Countoftickets { get; set; }


}