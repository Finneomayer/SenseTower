using MediatR;

namespace SC.SenseTower.Images.Cqrs.DeleteImage
{
    public class DeleteImageCommand : BaseRequest, IRequest<Unit>
    {
    }
}
