using AutoMapper;
using MediatR;
using SC.SenseTower.Common.Cqrs.Handlers;
using SC.SenseTower.Galleries.Dto.Galleries;
using SC.SenseTower.Galleries.Services;

namespace SC.SenseTower.Galleries.Cqrs.GalleryImages
{
    public class GalleryImagesRequestHandler : BaseHandler, IRequestHandler<GalleryImagesRequest, Dictionary<int, GalleryImageDto>>
    {
        private readonly GalleriesService galleriesService;

        public GalleryImagesRequestHandler(
            ILogger<GalleryImagesRequestHandler> logger,
            IMapper mapper,
            GalleriesService galleriesService) : base(logger, mapper)
        {
            this.galleriesService = galleriesService;
        }

        public async Task<Dictionary<int, GalleryImageDto>> Handle(GalleryImagesRequest request, CancellationToken cancellationToken)
        {
            var gallery = await galleriesService.Get(request.Id, cancellationToken);
            var result = gallery.Pictures
                .Select(x => new KeyValuePair<int, GalleryImageDto>(x.Position, Mapper.Map<GalleryImageDto>(x.Image)))
                .ToDictionary(x => x.Key, x => x.Value);
            return result;
        }
    }
}
