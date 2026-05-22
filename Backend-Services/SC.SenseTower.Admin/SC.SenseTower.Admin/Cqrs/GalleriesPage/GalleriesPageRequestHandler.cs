using AutoMapper;
using MediatR;
using SC.SenseTower.Admin.Dto.Galleries;
using SC.SenseTower.Admin.Services;
using SC.SenseTower.Common.Cqrs.Handlers;
using SC.SenseTower.Common.Models;

namespace SC.SenseTower.Admin.Cqrs.GalleriesPage
{
    public class GalleriesPageRequestHandler : BaseHandler, IRequestHandler<GalleriesPageRequest, PagedDataDto<GalleryGridItemDto>>
    {
        private readonly GalleriesService galleriesService;

        public GalleriesPageRequestHandler(
            ILogger<GalleriesPageRequestHandler> logger,
            IMapper mapper,
            GalleriesService galleriesService) : base(logger, mapper)
        {
            this.galleriesService = galleriesService;
        }

        public async Task<PagedDataDto<GalleryGridItemDto>> Handle(GalleriesPageRequest request, CancellationToken cancellationToken)
        {
            var data = await galleriesService.GetPagedList(request.AccessToken, request.Filters, request.Sorting, request.Page, request.PageSize, cancellationToken);
            var result = Mapper.Map<PagedDataDto<GalleryGridItemDto>>(data);
            return result;
        }
    }
}
