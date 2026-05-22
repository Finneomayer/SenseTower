using AutoMapper;
using MediatR;
using SC.SenseTower.Galleries.Constants;
using SC.SenseTower.Galleries.Dto.Galleries;
using SC.SenseTower.Galleries.Services;
using SC.SenseTower.Common.Cqrs.Handlers;
using SC.SenseTower.Common.Extensions;
using SC.SenseTower.Common.Models;

namespace SC.SenseTower.Galleries.Cqrs.GalleryList
{
    public class GalleryListRequestHandler : BaseHandler, IRequestHandler<GalleryListRequest, PagedDataDto<GalleryItemDto>>
    {
        private readonly GalleriesService galleriesService;

        public GalleryListRequestHandler(
            ILogger<GalleryListRequestHandler> logger,
            IMapper mapper,
            GalleriesService galleriesService) : base(logger, mapper)
        {
            this.galleriesService = galleriesService;
        }

        public async Task<PagedDataDto<GalleryItemDto>> Handle(GalleryListRequest request, CancellationToken cancellationToken)
        {
            var data = await galleriesService.Get(request.Filters, request.Sorting, request.Offset, request.PageSize, cancellationToken);
            var items = Mapper.Map<GalleryItemDto[]>(data);

            //var data = FakeData.Galleries
            //    .Select(x => new GalleryItemDto
            //    {
            //        Id = x.Id,
            //        InfoTable = x.GalleryInfoTable,
            //        Name = x.Name,
            //        PicturesCounter = x.PicturesLocation.Count,
            //        Space = x.Space
            //    })
            //    .ToArray();

            var result = new PagedDataDto<GalleryItemDto>
            {
                Items = items,
                TotalCount = items.Length
            };

            return result;
        }
    }
}
