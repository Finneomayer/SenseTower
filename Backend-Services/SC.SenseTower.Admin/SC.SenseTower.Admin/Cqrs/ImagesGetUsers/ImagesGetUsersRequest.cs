using MediatR;
using SC.SenseTower.Admin.Dto;
using SC.SenseTower.Admin.Dto.Images;

namespace SC.SenseTower.Admin.Cqrs.ImagesGetUsers
{
    public class ImagesGetUsersRequest : ExternalRequestDto, IRequest<IEnumerable<ImageInfoDto>>
    {
    }
}
