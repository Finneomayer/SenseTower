using SC.SenseTower.Common.Enums;
using SC.SenseTower.TowerEvents.Dto.Spaces;

namespace SC.SenseTower.TowerEvents.Dto.TowerEvents
{
    public class TowerEventDto
    {
        /// <summary>
        /// Идентификатор события.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Дата проведения события.
        /// </summary>
        public DateTimeOffset Date { get; set; }

        /// <summary>
        /// Дата и время начала события.
        /// </summary>
        public DateTimeOffset From => Date;

        /// <summary>
        /// Дата и время окончания события.
        /// </summary>
        public DateTimeOffset UpTo { get; set; }

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
        /// Ссылка на афишу события.
        /// </summary>
        public string ImageUrl { get; set; } = null!;

        /// <summary>
        /// Пространство, к которому привязано событие.
        /// </summary>
        public LocalSpaceDto? Space { get; set; }

        /// <summary>
        /// Текущее состояние события.
        /// </summary>
        public TowerEventState State { get; set; }

        /// <summary>
        /// Массив проданных билетов.
        /// </summary>
        public TicketDto[] SoldTickets { get; set; } = Array.Empty<TicketDto>();

        /// <summary>
        /// Общее число билетов на событие.
        /// </summary>
        public int TotalTickets { get; set; }

        /// <summary>
        /// Число проданных билетов на событие.
        /// </summary>
        public int Sold { get; set; }
    }
}
