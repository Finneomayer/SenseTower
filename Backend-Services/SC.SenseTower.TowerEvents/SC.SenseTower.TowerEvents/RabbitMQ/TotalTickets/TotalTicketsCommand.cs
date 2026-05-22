using MediatR;

namespace SC.SenseTower.TowerEvents.RabbitMQ.TotalTickets
{
    public class TotalTicketsCommand : IRequest<Unit>
    {
        public Guid EventId { get; set; }

        public int TotalTickets { get; set; }
    }
}
