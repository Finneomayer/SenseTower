using MediatR;

namespace SC.SenseTower.Tickets.RabbitMQ.TicketsCreate
{
    public class TicketsCreateCommand : IRequest<Unit>
    {
        public TowerEventDto TowerEvent { get; set; } = new();

        public int Quantity { get; set; }
    }
}
