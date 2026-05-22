using AutoMapper;
using MediatR;
using SC.SenseTower.Common.Cqrs.Handlers;
using SC.SenseTower.Galleries.Data.Models;
using SC.SenseTower.Galleries.Services;

namespace SC.SenseTower.Galleries.Cqrs.ReplaceGalleryImages
{
    public class ReplaceGalleryImagesCommandHandler : BaseHandler, IRequestHandler<ReplaceGalleryImagesCommand, Unit>
    {
        private readonly GalleriesService galleriesService;
        private readonly ImagesService imagesService;

        public ReplaceGalleryImagesCommandHandler(
            ILogger<ReplaceGalleryImagesCommandHandler> logger,
            IMapper mapper,
            GalleriesService galleriesService,
            ImagesService imagesService) : base(logger, mapper)
        {
            this.galleriesService = galleriesService;
            this.imagesService = imagesService;
        }

        public async Task<Unit> Handle(ReplaceGalleryImagesCommand request, CancellationToken cancellationToken)
        {
            var pictures = request.Images
                .Select(x => new Picture
                {
                    Position = x.Position,
                    Image = new GalleryImage
                    {
                        Author = x.Author,
                        Description = x.Description,
                        Image = new ImageInfo
                        {
                            Id = x.ImageId
                        },
                        Name = x.Name,
                        PassepartoutWidthInMeters = x.PassepartoutWidthInMeters,
                        PictureWidthInMeters = x.PictureWidthInMeters
                    }
                })
                .ToArray();

            var imageIds = request.Images
                .Select(x => x.ImageId)
                .Distinct()
                .ToArray();
            if (imageIds.Length > 0)
            {
                var imageInfos = await imagesService.GetByIds(request.AccessToken, imageIds, cancellationToken);
                if (imageInfos != null && imageInfos.Any())
                {
                    foreach (var item in pictures)
                    {
                        var image = imageInfos.FirstOrDefault(x => x.Id == item.Image.Image.Id);
                        if (image != null)
                        {
                            item.Image.Image.FileUrl = image.FileUrl;
                            item.Image.Image.Name = image.Name;
                            item.Image.Image.PreviewUrl = image.PreviewUrl;
                        }
                    }
                }
            }

            await galleriesService.ReplacePictures(request.GalleryId, pictures, cancellationToken);
            return Unit.Value;
        }
    }
}
