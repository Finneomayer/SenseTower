using MediatR;

namespace SC.SenseTower.MyPlaces.Cqrs.Commands
{
    public class DeleteImageCommand : IRequest<Unit>
    {
        /// <summary>
        /// Идентификатор помещения.
        /// </summary>
        public Guid PlaceId { get; set; }

        /// <summary>
        /// Идентификатор изображения.
        /// </summary>
        public Guid ImageId { get; set; }

        /// <summary>
        /// Токен пользователя (заполняется сервером).
        /// </summary>
        public string AccessToken { get; set; } = null!;
    }
}
