using AutoMapper;
using MediatR;
using SC.SenseTower.Common.Cqrs.Handlers;
using SC.SenseTower.Spaces.Cqrs.Commands;
using SC.SenseTower.Spaces.Data.Models;
using SC.SenseTower.Spaces.RabbitMQ;
using SC.SenseTower.Spaces.Services;

namespace SC.SenseTower.Spaces.Cqrs.Handlers
{
    public class SetDoorImageCommandHandler : BaseHandler, IRequestHandler<SetDoorImageCommand, Unit>
    {
        private readonly ISpacesService spacesService;
        private readonly ImagesService imagesService;
        private readonly IRabbitMQService rabbitMQService;

        public SetDoorImageCommandHandler(
            ILogger<SetDoorImageCommandHandler> logger,
            IMapper mapper,
            ISpacesService spacesService,
            ImagesService imagesService,
            IRabbitMQService rabbitMQService) : base(logger, mapper)
        {
            this.spacesService = spacesService;
            this.imagesService = imagesService;
            this.rabbitMQService = rabbitMQService;
        }

        public async Task<Unit> Handle(SetDoorImageCommand request, CancellationToken cancellationToken)
        {
            var space = await spacesService.Get(request.SpaceId, cancellationToken);
            if (request.ImageId == null)
            {
                space.DoorImage = null;
            }
            else
            {
                var imageInfoDto = await imagesService.GetInfo(request.AccessToken, request.ImageId.Value, cancellationToken);
                space.DoorImage = Mapper.Map<ImageInfo>(imageInfoDto);
            }

            await spacesService.Update(space, cancellationToken);
            await rabbitMQService.SendUpdateSpaceMessage(space, cancellationToken);

            return Unit.Value;
        }
    }
}
