using MediatR;
using SC.SenseTower.Common.Models;
using SC.SenseTower.Galleries.Dto.Galleries;

namespace SC.SenseTower.Galleries.Cqrs.GalleryList
{
    public class GalleryListRequest : PagedDataRequestDto<GalleryListFilter>, IRequest<PagedDataDto<GalleryItemDto>>
    {
        /// <summary>
        /// Токен пользователя (заполняется сервером).
        /// </summary>
        public string AccessToken { get; set; } = null!;
    }
}
