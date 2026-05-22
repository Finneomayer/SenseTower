using MediatR;
using SC.SenseTower.Admin.Dto;
using SC.SenseTower.Admin.Dto.Places;

namespace SC.SenseTower.Admin.Cqrs.Place
{
    public class PlaceRequest : ItemRequestDto, IRequest<PlaceDto>
    {
    }
}
