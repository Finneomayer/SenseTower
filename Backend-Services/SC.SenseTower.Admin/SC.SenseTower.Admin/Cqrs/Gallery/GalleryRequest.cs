using MediatR;
using SC.SenseTower.Admin.Dto;
using SC.SenseTower.Admin.Dto.Galleries;

namespace SC.SenseTower.Admin.Cqrs.Gallery
{
    public class GalleryRequest : ItemRequestDto, IRequest<GalleryDto>
    {
    }
}
