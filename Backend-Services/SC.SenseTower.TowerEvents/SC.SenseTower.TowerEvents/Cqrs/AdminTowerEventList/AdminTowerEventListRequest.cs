using MediatR;
using SC.SenseTower.Common.Models;
using SC.SenseTower.TowerEvents.Dto.TowerEvents;

namespace SC.SenseTower.TowerEvents.Cqrs.AdminTowerEventList
{
    public class AdminTowerEventListRequest : PagedDataRequestDto<AdminTowerEventListFilter>, IRequest<PagedDataDto<TowerEventListItemDto>>
    {
    }
}
