using MediatR;
using SC.SenseTower.Tickets.RabbitMQ.TicketsCreate;

namespace SC.SenseTower.Tickets.RabbitMQ.TowerEventUpdate
{
    public class TowerEventUpdateCommand : IRequest<Unit>
    {
        public TowerEventDto TowerEvent { get; set; } = new();
    }
}
