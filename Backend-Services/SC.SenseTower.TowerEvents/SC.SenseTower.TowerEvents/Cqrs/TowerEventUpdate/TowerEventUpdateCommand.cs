using MediatR;
using SC.SenseTower.Common.Enums;

namespace SC.SenseTower.TowerEvents.Cqrs.TowerEventUpdate
{
    public class TowerEventUpdateCommand : IRequest<Unit>
    {
        /// <summary>
        /// Идентификатор события.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Дата и время начала события.
        /// </summary>
        public DateTimeOffset Date { get; set; }

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
        /// Идентификатор пространства, к которому привязано событие.
        /// </summary>
        public Guid? SpaceId { get; set; }

        /// <summary>
        /// Состояние события.
        /// </summary>
        public TowerEventState State { get; set; }

        /// <summary>
        /// Токен пользователя (заполняеися сервером).
        /// </summary>
        public string AccessToken { get; set; } = null!;
    }
}
