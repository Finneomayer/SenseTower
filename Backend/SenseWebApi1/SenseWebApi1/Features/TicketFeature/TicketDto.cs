namespace SenseWebApi1.Features.TicketFeature;

public class TicketDto
{
    public Guid TicketId { get; set; }

    public Guid? OwnerId { get; set; }

    public int? Place { get; set; }
}