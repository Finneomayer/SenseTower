using MediatR;

namespace SC.SenseTower.Galleries.Cqrs.UpdateGallery
{
    public class UpdateGalleryCommand : IRequest<Unit>
    {
        /// <summary>
        /// Идентификатор галереи.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Название галереи.
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// Описание галереи.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Признак показа информационного табло.
        /// </summary>
        public bool IsVisible { get; set; }

        /// <summary>
        /// Идентификатор изображения для информационного табло.
        /// </summary>
        public Guid? ImageId { get; set; }

        /// <summary>
        /// Не использовать, будет удалено.
        /// </summary>
        public IFormFile? ImageFile { get; set; }

        /// <summary>
        /// Идентификатор пространства, к которому привязана галерея.
        /// </summary>
        public Guid SpaceId { get; set; }

        /// <summary>
        /// Токен пользователя (заполняется сервером).
        /// </summary>
        public string AccessToken { get; set; } = null!;

        /// <summary>
        /// Идентификатор текущего пользователя (заполняется сервером).
        /// </summary>
        public Guid UserId { get; set; }
    }
}
