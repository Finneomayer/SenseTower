using AutoMapper;
using MediatR;
using SC.SenseTower.Common.Cqrs.Handlers;
using SC.SenseTower.Common.Exceptions;
using SC.SenseTower.Galleries.Data.Models;
using SC.SenseTower.Galleries.Services;

namespace SC.SenseTower.Galleries.Cqrs.CreateGallery
{
    public class CreateGalleryCommandHandler : BaseHandler, IRequestHandler<CreateGalleryCommand, Guid>
    {
        private readonly GalleriesService galleriesService;
        private readonly SpacesService spacesService;
        private readonly ImagesService imagesService;

        public CreateGalleryCommandHandler(
            ILogger<CreateGalleryCommandHandler> logger,
            IMapper mapper,
            GalleriesService galleriesService,
            SpacesService spacesService,
            ImagesService imagesService) : base(logger, mapper)
        {
            this.galleriesService = galleriesService;
            this.spacesService = spacesService;
            this.imagesService = imagesService;
        }

        public async Task<Guid> Handle(CreateGalleryCommand request, CancellationToken cancellationToken)
        {
            if (request.Id == null)
                request.Id = Guid.NewGuid();
            var gallery = Mapper.Map<Data.Models.Gallery>(request);

            if (request.ImageFile != null)
            {
                var imageId = await imagesService.Create(request.AccessToken, request.UserId, request.ImageFile, cancellationToken);
                request.ImageId = imageId;
            }
            if (request.ImageId != null)
            {
                var imageInfo = await imagesService.CopyImage(request.AccessToken, request.ImageId.Value, cancellationToken);
                if (imageInfo == null)
                    throw new ScException("Изображение не найдено, его необходимо предварительно загрузить");
                gallery.GalleryInfoTable.Image = new ImageInfo
                {
                    Id = imageInfo.Id,
                    Name = imageInfo.Name,
                    FileUrl = imageInfo.FileUrl,
                    PreviewUrl = imageInfo.PreviewUrl
                };
            }

            var space = await spacesService.GetSpace(request.AccessToken, request.SpaceId, cancellationToken);
            gallery.Space = Mapper.Map<Space>(space);

            var result = await galleriesService.Create(gallery, cancellationToken);
            return result;
        }
    }
}
