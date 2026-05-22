using AutoMapper;
using MediatR;
using Microsoft.Extensions.Options;
using SC.SenseTower.Common.Cqrs.Handlers;
using SC.SenseTower.Images.Data.Models;
using SC.SenseTower.Images.Dto.Images;
using SC.SenseTower.Images.Services;
using SC.SenseTower.Images.Settings;

namespace SC.SenseTower.Images.Cqrs.Image
{
    public class ImageRequestHandler : BaseHandler, IRequestHandler<ImageRequest, ImageDto?>
    {
        private readonly ImageFilesService imageFilesService;
        private readonly PlacesService placesService;
        private readonly ImageSettings settings;

        public ImageRequestHandler(
            ILogger<ImageRequestHandler> logger,
            IMapper mapper,
            ImageFilesService imageFilesService,
            PlacesService placesService,
            IOptions<ImageSettings> options) : base(logger, mapper)
        {
            this.imageFilesService = imageFilesService;
            this.placesService = placesService;
            settings = options.Value;
        }

        public async Task<ImageDto?> Handle(ImageRequest request, CancellationToken cancellationToken)
        {
            var imageFile = await imageFilesService.Get(request.Id, cancellationToken);
            if (imageFile == null)
                return null;
            var result = Mapper.Map<ImageFile, ImageDto>(imageFile, o =>
            {
                o.AfterMap((s, d) =>
                {
                    d.FileUrl = settings.RootUrl + d.FileUrl;
                    d.PreviewUrl = settings.RootUrl + d.PreviewUrl;
                });
            });
            return result;
        }
    }
}
