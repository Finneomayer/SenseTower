using MediatR;
using SC.SenseTower.Admin.Dto.TowerEvents;
using SC.SenseTower.Common.Models;

namespace SC.SenseTower.Admin.Cqrs.TowerEventsPage
{
    public class TowerEventsPageRequest : PagedDataRequest<TowerEventsPageFilter>, IRequest<PagedDataDto<TowerEventGridItemDto>>
    {
        public string AccessToken { get; set; } = null!;

        public string RefreshToken { get; set; } = null!;
    }
}
