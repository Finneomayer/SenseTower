using MediatR;
using SC.SenseTower.Common.Models;
using SC.SenseTower.MyPlaces.Dto.Places;

namespace SC.SenseTower.MyPlaces.Cqrs.Requests
{
    public class AdminPlacesPageRequest : PagedDataRequestDto<AdminPlacesPageFilter>, IRequest<PagedDataDto<PlaceDto>>
    {
        public string AccessToken { get; set; } = null!;
    }
}
