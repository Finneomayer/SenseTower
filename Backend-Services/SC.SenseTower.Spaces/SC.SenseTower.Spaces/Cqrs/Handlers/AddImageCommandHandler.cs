using AutoMapper;
using MediatR;
using SC.SenseTower.Common.Cqrs.Handlers;
using SC.SenseTower.Spaces.Cqrs.Commands;
using SC.SenseTower.Spaces.Data.Models;
using SC.SenseTower.Spaces.RabbitMQ;
using SC.SenseTower.Spaces.Services;

namespace SC.SenseTower.Spaces.Cqrs.Handlers
{
    public class AddImageCommandHandler : BaseHandler, IRequestHandler<AddImageCommand, Unit>
    {
        private readonly ImagesService imagesService;
        private readonly ISpacesService spacesService;
        private readonly IRabbitMQService rabbitMQService;

        public AddImageCommandHandler(
            ILogger<AddImageCommandHandler> logger,
            IMapper mapper,
            ImagesService imagesService,
            ISpacesService spacesService,
            IRabbitMQService rabbitMQService) : base(logger, mapper)
        {
            this.imagesService = imagesService;
            this.spacesService = spacesService;
            this.rabbitMQService = rabbitMQService;
        }

        public async Task<Unit> Handle(AddImageCommand request, CancellationToken cancellationToken)
        {
            var space = await spacesService.Get(request.SpaceId, cancellationToken);
            var imageInfo = await imagesService.GetInfo(request.AccessToken, request.ImageId, cancellationToken);
            var image = Mapper.Map<ImageInfo>(imageInfo);

            var images = space.Images?.OrderBy(x => x.Location).ToList() ?? new List<Picture>();
            var emptySlot = 0;
            var slotFound = false;
            for (var i = 0; i < images.Count; i++)
            {
                if (i != images[i].Location)
                {
                    emptySlot = i;
                    slotFound = true;
                    break;
                }
            }
            if (!slotFound)
            {
                emptySlot = images.Count;
            }

            var picture = new Picture
            {
                Location = emptySlot,
                Image = image
            };
            images.Add(picture);

            await spacesService.ReplaceImages(request.SpaceId, images.OrderBy(x => x.Location).ToArray(), cancellationToken);

            space = await spacesService.Get(request.SpaceId, cancellationToken);
            await rabbitMQService.SendUpdateSpaceMessage(space, cancellationToken);

            return Unit.Value;
        }
    }
}
