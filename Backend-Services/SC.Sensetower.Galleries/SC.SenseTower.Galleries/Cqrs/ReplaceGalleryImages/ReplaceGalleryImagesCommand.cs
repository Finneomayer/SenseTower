using MediatR;
using SC.SenseTower.Galleries.Dto.Galleries;

namespace SC.SenseTower.Galleries.Cqrs.ReplaceGalleryImages
{
    public class ReplaceGalleryImagesCommand : IRequest<Unit>
    {
        /// <summary>
        /// Идентификатор галереи.
        /// </summary>
        public Guid GalleryId { get; set; }

        /// <summary>
        /// Коллекция изображений в галерее.
        /// </summary>
        public GalleryImageSaveDto[] Images { get; set; } = Array.Empty<GalleryImageSaveDto>();

        /// <summary>
        /// Токен пользователя (заполняется сервером).
        /// </summary>
        public string AccessToken { get; set; } = null!;
    }
}
