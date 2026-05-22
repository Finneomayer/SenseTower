using AutoMapper;
using MediatR;
using SC.SenseTower.Admin.Dto.Galleries;
using SC.SenseTower.Admin.Services;
using SC.SenseTower.Common.Cqrs.Handlers;

namespace SC.SenseTower.Admin.Cqrs.GalleryImageAdd
{
    public class GalleryImageAddCommandHandler : BaseHandler, IRequestHandler<GalleryImageAddCommand, Unit>
    {
        private readonly GalleriesService galleriesService;

        public GalleryImageAddCommandHandler(
            ILogger<GalleryImageAddCommandHandler> logger,
            IMapper mapper,
            GalleriesService galleriesService) : base(logger, mapper)
        {
            this.galleriesService = galleriesService;
        }

        public async Task<Unit> Handle(GalleryImageAddCommand request, CancellationToken cancellationToken)
        {
            var galleryImage = Mapper.Map<GalleryAddImageDto>(request);
            await galleriesService.AddImage(request.AccessToken, request.GalleryId, galleryImage, cancellationToken);
            return Unit.Value;
        }
    }
}
