using MediatR;
using SC.SenseTower.Admin.Dto.Places;
using SC.SenseTower.Common.Models;

namespace SC.SenseTower.Admin.Cqrs.PlacesPage
{
    public class PlacesPageRequest : PagedDataRequest<PlacesPageFilter>, IRequest<PagedDataDto<PlacesGridItemDto>>
    {
        public string AccessToken { get; set; } = null!;

        public string RefreshToken { get; set; } = null!;
    }
}
