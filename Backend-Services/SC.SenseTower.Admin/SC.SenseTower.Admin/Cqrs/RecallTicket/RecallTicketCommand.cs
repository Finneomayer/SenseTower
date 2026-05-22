using MediatR;

namespace SC.SenseTower.Admin.Cqrs.RecallTicket
{
    public class RecallTicketCommand : IRequest<Unit>
    {
        public string Id { get; set; } = null!;

        public string? Reason { get; set; }
    }
}
