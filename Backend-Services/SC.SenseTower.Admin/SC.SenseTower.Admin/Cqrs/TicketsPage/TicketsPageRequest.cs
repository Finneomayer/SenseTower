using MediatR;
using SC.SenseTower.Admin.Dto.Tickets;
using SC.SenseTower.Common.Data;
using SC.SenseTower.Common.Models;

namespace SC.SenseTower.Admin.Cqrs.TicketsPage
{
    public class TicketsPageRequest : BaseListRequest, IRequest<PagedDataDto<TicketGridItemDto>>
    {
        public TicketsPageFilter Filters { get; set; } = new();
    }
}
