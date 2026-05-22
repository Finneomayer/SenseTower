using MediatR;

namespace SC.SenseTower.Images.Cqrs.UpdateImage
{
    public class UpdateImageCommand : BaseRequest, IRequest<Unit>
    {
        /// <summary>
        /// Название изображения.
        /// </summary>
        public string Name { get; set; } = null!;
    }
}
