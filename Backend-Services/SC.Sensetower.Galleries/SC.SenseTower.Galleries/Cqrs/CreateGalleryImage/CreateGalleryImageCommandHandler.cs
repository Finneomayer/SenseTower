using AutoMapper;
using MediatR;
using SC.SenseTower.Common.Cqrs.Handlers;
using SC.SenseTower.Common.Exceptions;
using SC.SenseTower.Galleries.Data.Models;
using SC.SenseTower.Galleries.Services;

namespace SC.SenseTower.Galleries.Cqrs.CreateGalleryImage
{
    public class CreateGalleryImageCommandHandler : BaseHandler, IRequestHandler<CreateGalleryImageCommand, Unit>
    {
        private readonly GalleriesService galleriesService;
        private readonly ImagesService imagesService;

        public CreateGalleryImageCommandHandler(
            ILogger<CreateGalleryImageCommandHandler> logger,
            IMapper mapper,
            GalleriesService galleriesService,
            ImagesService imagesService) : base(logger, mapper)
        {
            this.galleriesService = galleriesService;
            this.imagesService = imagesService;
        }

        public async Task<Unit> Handle(CreateGalleryImageCommand request, CancellationToken cancellationToken)
        {
            var picture = Mapper.Map<Picture>(request);
            var imageInfo = await imagesService.CopyImage(request.AccessToken, request.ImageId.Value, cancellationToken);
            if (imageInfo == null)
                throw new ScException("Изображение не найдено, его необходимо предварительно загрузить");
            picture.Image.Image = Mapper.Map<ImageInfo>(imageInfo);

            var gallery = await galleriesService.Get(request.GalleryId, cancellationToken);

            var pictures = gallery.Pictures.OrderBy(x => x.Position).ToList();
            var positionFound = false;
            for (var i = 0; i < pictures.Count; i++)
            {
                if (i != pictures[i].Position)
                {
                    positionFound = true;
                    picture.Position = i;
                    break;
                }
            }
            if (!positionFound)
                picture.Position = pictures.Count;
            pictures.Add(picture);

            gallery.Pictures = pictures.OrderBy(x => x.Position).ToArray();
            await galleriesService.Update(gallery, cancellationToken);
            return Unit.Value;
        }
    }
}
