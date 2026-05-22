using MediatR;

namespace SC.SenseTower.Images.Cqrs.AddImageFile
{
    public class AddImageFileCommand : IRequest<Guid>
    {
        /// <summary>
        /// Идентификатор пользователя.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Название изображения.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Файл изображения.
        /// </summary>
        public IFormFile File { get; set; } = null!;

        /// <summary>
        /// Токен пользователя (заполняется сервером).
        /// </summary>
        public string? AccessToken { get; set; }

    }
}
