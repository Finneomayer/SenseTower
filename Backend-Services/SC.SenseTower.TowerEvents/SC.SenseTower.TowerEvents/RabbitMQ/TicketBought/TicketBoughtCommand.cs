using MediatR;

namespace SC.SenseTower.TowerEvents.RabbitMQ.TicketBought
{
    public class TicketBoughtCommand : IRequest<Unit>
    {
        public Guid EventId { get; set; }

        public Guid TicketId { get; set; }

        public Guid UserId { get; set; }
    }
}
