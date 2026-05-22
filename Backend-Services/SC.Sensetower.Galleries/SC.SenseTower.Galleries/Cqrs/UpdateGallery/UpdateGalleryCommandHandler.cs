using AutoMapper;
using MediatR;
using SC.SenseTower.Common.Cqrs.Handlers;
using SC.SenseTower.Common.Exceptions;
using SC.SenseTower.Galleries.Data.Models;
using SC.SenseTower.Galleries.Services;

namespace SC.SenseTower.Galleries.Cqrs.UpdateGallery
{
    public class UpdateGalleryCommandHandler : BaseHandler, IRequestHandler<UpdateGalleryCommand, Unit>
    {
        private readonly GalleriesService galleriesService;
        private readonly SpacesService spacesService;
        private readonly ImagesService imagesService;

        public UpdateGalleryCommandHandler(
            ILogger<UpdateGalleryCommandHandler> logger,
            IMapper mapper,
            GalleriesService galleriesService,
            SpacesService spacesService,
            ImagesService imagesService) : base(logger, mapper)
        {
            this.galleriesService = galleriesService;
            this.spacesService = spacesService;
            this.imagesService = imagesService;
        }

        public async Task<Unit> Handle(UpdateGalleryCommand request, CancellationToken cancellationToken)
        {
            var gallery = await galleriesService.Get(request.Id, cancellationToken);

            if (request.ImageFile != null)
            {
                var imageId = await imagesService.Create(request.AccessToken, request.UserId, request.ImageFile, cancellationToken);
                request.ImageId = imageId;
            }
            if (request.ImageId != gallery?.GalleryInfoTable.Image?.Id)
            {
                if (request.ImageId != null)
                {
                    var imageInfo = (await imagesService.GetByIds(request.AccessToken, new Guid[] { request.ImageId.Value }, cancellationToken))?.FirstOrDefault();
                    if (imageInfo == null)
                        throw new ScException("Не найдено изображение");
                    gallery.GalleryInfoTable.Image = new ImageInfo
                    {
                        Id = imageInfo.Id,
                        Name = imageInfo.Name,
                        FileUrl = imageInfo.FileUrl,
                        PreviewUrl = imageInfo.PreviewUrl
                    };
                }
                else
                {
                    gallery.GalleryInfoTable.Image = null;
                }
            }

            if (request.SpaceId != gallery?.Space.Id)
            {
                var space = await spacesService.GetSpace(request.AccessToken, request.SpaceId, cancellationToken);
                gallery.Space = Mapper.Map<Space>(space);
            }

            await galleriesService.Update(gallery, cancellationToken);
            return Unit.Value;
        }
    }
}
