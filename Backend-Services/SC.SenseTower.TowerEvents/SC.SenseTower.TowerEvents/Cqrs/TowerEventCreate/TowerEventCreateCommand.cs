using MediatR;

namespace SC.SenseTower.TowerEvents.Cqrs.TowerEventCreate
{
    public class TowerEventCreateCommand : IRequest<Guid>
    {
        /// <summary>
        /// Идентификатор события (необязательно).
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Дата и время начала события.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Дата и время окончания события.
        /// </summary>
        public DateTime UpTo { get; set; }

        /// <summary>
        /// Название события.
        /// </summary>
        public string Title { get; set; } = null!;

        /// <summary>
        /// Описание события.
        /// </summary>
        public string Description { get; set; } = null!;

        /// <summary>
        /// Идентификатор изображения, афиши события.
        /// </summary>
        public Guid? ImageId { get; set; }

        /// <summary>
        /// Идентификатор пространства, к которому привязано событие.
        /// </summary>
        public Guid? SpaceId { get; set; }

        /// <summary>
        /// Количество билетов, требуемое для проведения события.
        /// </summary>
        public int? TicketQuantity { get; set; }

        /// <summary>
        /// Токен пользователя (заполняеися сервером).
        /// </summary>
        public string AccessToken { get; set; } = null!;
    }
}
