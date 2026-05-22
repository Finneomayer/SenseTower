using MediatR;

namespace SC.SenseTower.Tickets.RabbitMQ.TowerEventDelete
{
    public class TowerEventDeleteCommand : IRequest<Unit>
    {
        public Guid EventId { get; set; }
    }
}
