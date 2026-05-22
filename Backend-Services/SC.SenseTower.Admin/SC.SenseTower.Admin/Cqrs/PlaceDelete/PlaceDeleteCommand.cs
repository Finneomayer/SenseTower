using MediatR;
using SC.SenseTower.Admin.Dto;

namespace SC.SenseTower.Admin.Cqrs.PlaceDelete
{
    public class PlaceDeleteCommand : ItemRequestDto, IRequest<Unit>
    {
    }
}
