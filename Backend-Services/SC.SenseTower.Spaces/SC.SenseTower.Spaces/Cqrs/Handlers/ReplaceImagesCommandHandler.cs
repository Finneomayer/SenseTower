using AutoMapper;
using MediatR;
using SC.SenseTower.Common.Cqrs.Handlers;
using SC.SenseTower.Spaces.Cqrs.Commands;
using SC.SenseTower.Spaces.Data.Models;
using SC.SenseTower.Spaces.RabbitMQ;
using SC.SenseTower.Spaces.Services;

namespace SC.SenseTower.Spaces.Cqrs.Handlers
{
    public class ReplaceImagesCommandHandler : BaseHandler, IRequestHandler<ReplaceImagesCommand, Unit>
    {
        private readonly ISpacesService spacesService;
        private readonly ImagesService imagesService;
        private readonly IRabbitMQService rabbitMQService;

        public ReplaceImagesCommandHandler(
            ILogger<ReplaceImagesCommandHandler> logger,
            IMapper mapper,
            ISpacesService spacesService,
            ImagesService imagesService,
            IRabbitMQService rabbitMQService) : base(logger, mapper)
        {
            this.spacesService = spacesService;
            this.imagesService = imagesService;
            this.rabbitMQService = rabbitMQService;
        }

        public async Task<Unit> Handle(ReplaceImagesCommand request, CancellationToken cancellationToken)
        {
            var imageIds = request.Images.Select(x => x.ImageId).Distinct().ToArray();
            var images = await imagesService.GetByIds(request.AccessToken, imageIds, cancellationToken);
            var pictures = request.Images
                .Select(x =>
                {
                    var image = images?.FirstOrDefault(i => i.Id == x.ImageId);
                    var item = new Picture
                    {
                        Location = x.Location,
                        Image = new ImageInfo
                        {
                            Id = x.ImageId,
                            FileUrl = image?.FileUrl ?? string.Empty,
                            Name = image?.Name ?? string.Empty,
                            PreviewUrl = image?.PreviewUrl ?? string.Empty
                        }
                    };
                    return item;
                })
                .ToArray();
            await spacesService.ReplaceImages(request.SpaceId, pictures, cancellationToken);

            var space = await spacesService.Get(request.SpaceId, cancellationToken);
            await rabbitMQService.SendUpdateSpaceMessage(space, cancellationToken);

            return Unit.Value;
        }
    }
}
