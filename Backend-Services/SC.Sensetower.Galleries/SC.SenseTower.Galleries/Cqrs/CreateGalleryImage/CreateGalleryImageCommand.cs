using MediatR;

namespace SC.SenseTower.Galleries.Cqrs.CreateGalleryImage
{
    public class CreateGalleryImageCommand : IRequest<Unit>
    {
        /// <summary>
        /// Идентификатор галереи.
        /// </summary>
        public Guid GalleryId { get; set; }

        /// <summary>
        /// Название изображения в галерее.
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// Описание изображения.
        /// </summary>
        public string Description { get; set; } = null!;

        /// <summary>
        /// Автор изображения.
        /// </summary>
        public string Author { get; set; } = null!;

        /// <summary>
        /// Идентификатор изображения.
        /// </summary>
        public Guid? ImageId { get; set; }

        /// <summary>
        /// Не использовать, будет удалено.
        /// </summary>
        public IFormFile? ImageFile { get; set; }

        /// <summary>
        /// Ширина изображения в метрах для Unity.
        /// </summary>
        public decimal PictureWidthInMeters { get; set; }

        /// <summary>
        /// Ширина паспарту в метрах для Unity.
        /// </summary>
        public decimal PassepartoutWidthInMeters { get; set; }

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
