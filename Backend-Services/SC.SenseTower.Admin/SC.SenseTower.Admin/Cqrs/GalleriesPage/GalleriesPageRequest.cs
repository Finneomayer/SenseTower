using MediatR;
using SC.SenseTower.Admin.Dto.Galleries;
using SC.SenseTower.Common.Models;

namespace SC.SenseTower.Admin.Cqrs.GalleriesPage
{
    public class GalleriesPageRequest : PagedDataRequest<GalleriesPageFilter>, IRequest<PagedDataDto<GalleryGridItemDto>>
    {
        public string AccessToken { get; set; } = null!;
    }
}
