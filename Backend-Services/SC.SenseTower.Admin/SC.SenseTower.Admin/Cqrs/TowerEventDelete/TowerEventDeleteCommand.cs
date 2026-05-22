using MediatR;
using SC.SenseTower.Admin.Dto;

namespace SC.SenseTower.Admin.Cqrs.TowerEventDelete
{
    public class TowerEventDeleteCommand : ExternalRequestDto, IRequest<Unit>
    {
        public Guid EventId { get; set; }
    }
}
