using MediatR;

namespace SC.SenseTower.Admin.Cqrs.TicketsBatchAdd
{
    public class TicketsBatchAddCommand : IRequest<string[]>
    {
        public Guid UserId { get; set; }

        public int Quantity { get; set; }
    }
}
