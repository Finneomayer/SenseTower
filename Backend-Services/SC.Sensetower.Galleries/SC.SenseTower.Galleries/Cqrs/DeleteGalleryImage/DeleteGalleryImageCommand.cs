using MediatR;

namespace SC.SenseTower.Galleries.Cqrs.DeleteGalleryImage
{
    public class DeleteGalleryImageCommand : IRequest<Unit>
    {
        /// <summary>
        /// Идентификатор галереи.
        /// </summary>
        public Guid GalleryId { get; set; }

        /// <summary>
        /// Позиция изображения в галерее, из которой изображение нужно убрать.
        /// </summary>
        public int Position { get; set; }
    }
}
