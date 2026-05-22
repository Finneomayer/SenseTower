using AutoMapper;
using MediatR;
using SC.SenseTower.Admin.Dto.Galleries;
using SC.SenseTower.Admin.Services;
using SC.SenseTower.Common.Cqrs.Handlers;
using SC.SenseTower.Common.Enums;
using SC.SenseTower.Common.Exceptions;

namespace SC.SenseTower.Admin.Cqrs.Gallery
{
    public class GalleryRequestHandler : BaseHandler, IRequestHandler<GalleryRequest, GalleryDto>
    {
        private readonly GalleriesService galleriesService;
        private readonly SpacesService spacesService;
        private readonly ImagesService imagesService;

        public GalleryRequestHandler(
            ILogger<GalleryRequestHandler> logger,
            IMapper mapper,
            GalleriesService galleriesService,
            SpacesService spacesService,
            ImagesService imagesService) : base(logger, mapper)
        {
            this.galleriesService = galleriesService;
            this.spacesService = spacesService;
            this.imagesService = imagesService;
        }

        public async Task<GalleryDto> Handle(GalleryRequest request, CancellationToken cancellationToken)
        {
            var gallery = await galleriesService.GetGallery(request.AccessToken, request.Id, cancellationToken);
            if (gallery == null)
                throw new ScException("Галерея не найдена");
            var result = Mapper.Map<GalleryDto>(gallery);
            result.AvailableSpaces = await spacesService.Lookup(request.AccessToken, SpaceType.MyGallery, cancellationToken);
            result.AvailableImages = await imagesService.Lookup(request.AccessToken, null, cancellationToken);
            return result;
        }
    }
}
