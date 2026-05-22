using MediatR;

namespace SC.SenseTower.Tickets.Cqrs.TicketsCreate
{
    public class TicketsCreateCommand : IRequest<Unit>
    {
        /// <summary>
        /// Идентификатор события.
        /// </summary>
        public Guid EventId { get; set; }

        /// <summary>
        /// Количество билетов (по умолчанию 1).
        /// </summary>
        public int? Quantity { get; set; }

        /// <summary>
        /// Токен пользователя (заполняется сервером).
        /// </summary>
        public string AccessToken { get; set; } = null!;
    }
}
